using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Pathfinding.Nodes;
using Pathfinding;

[RequireComponent (typeof (RigidBodyController))]

public class Agent : Entity {
	
//	public static Object prefab = Resources.Load("Prefabs/Capsule");
	
	public  int   lifeTotal = 100;
	private float _nodeSize; 		// size of the node in the world measure
	private float _delta;   			// time between ticks of "simulation"
	public  int   life;
	public  int   _depthOfSight;
	public  int   velocity = 5;
	
	public  List<EObject> backpack = new List<EObject>();
	
//	private Vector3 target = new Vector3(12, 0, 18);
//	private bool    moving = false;
//	private Vector3 velocityVector;
	
	private RigidBodyController _controller;
	private Engine _engine; 
	private List<GridNode> nodeList;
	
	public static Agent Create(	Object prefab, 
								Vector3 position, 
								Engine engine,
								string description, 
								string name, 
								int lifeTotal) 
	{
		GameObject gameObj = Instantiate(prefab, position, Quaternion.identity) as GameObject;
		Agent      agent   = gameObj.GetComponent<Agent>();
		
		agent.life         = lifeTotal;
		agent.description  = description;
		agent.name         = name;
		agent._engine	   = engine;		
		agent._nodeSize     = 2; //hardcodeado. Traté de hacerlo andar dinámicamente, pero no pude.
						  		 //Es el mismo número que el node size definido en el objeto A*, por IDE	
		return agent;
	}
	
	void Start(){
		this._delta      = _engine.delta;
		this._controller = this.GetComponent<RigidBodyController>();
		
		InvokeRepeating("execute", 0, _delta);
	}
	
	// esto se ejecuta en cada ciclo de "simulación"
	void execute(){		
		nodeList = this.perceptNodes();
		
		List<Agent> agents = this.perceptObjects<Agent>("agent");
		
		// TEST
		if (!_controller.moving && nodeList.Count > 1)
			_controller.move((Vector3)(nodeList[Random.Range(0, nodeList.Count)].position));	
		// TEST
		
	}
	
		
	public void moveToNode(int node){
		_controller.move((Vector3)(nodeList[node].position));	
	}
	
	public void moveToNode(Vector3 target){
		_controller.move(target);	
	}
	
	public void subLife(int dif) {
		if(this.life - dif <= 0){
			this.life = 0;	
			//TODO: DIEEEEEEE
		} else {
			this.life = life - dif;	
		}
	}
	
	public void addLife(int dif) {
		if(this.life + dif >= this.lifeTotal){
			this.life = this.lifeTotal;
		} else {
			this.life += dif;	
		}
	}
	
	public void pickup(EObject obj) {
		//TODO: 
		// - check the distance between agent and object
		// - remove object from the game
		this.backpack.Add(obj);
	}
	
	public void drop(EObject obj) {

		this.backpack.Remove(obj);
	}
	
	public override Hashtable perception(){
		Hashtable p = base.perception();
		p["life"]       = life;
		p["lifeTotal"]  = lifeTotal;
		p["backpack"]   = backpack;		
		p["agentsSeen"] = perceptObjects<Agent>("agent");
		p["goldSeen"]   = perceptObjects<Gold> ("gold");

		return p;
	}
	
	private List<ObjectType> perceptObjects<ObjectType>(string type, string layer = "perception") 
		where ObjectType : Component // cuánta magia
									// acá restrinjo el tipo pasado por 
									// parámetro
		{
		Collider[] colliders = 
			Physics.OverlapSphere(this.transform.position, _depthOfSight,
				1 << LayerMask.NameToLayer(layer)); // usa mascaras, con el << agregas con BITWISE-OR
		List<ObjectType> aux = new List<ObjectType>();
		
    	foreach (Collider hit in colliders) {		
			if (hit.tag == type)
				aux.Add(hit.gameObject.GetComponent<ObjectType>());
    	}
//		if (aux.Count > 0)
//			Debug.Log(aux.Count);
		return aux;
	}

	
	private List<GridNode> perceptNodes(){
		GridGraph graph = AstarPath.active.astarData.gridGraph;
		GridNode  node  = AstarPath.active.GetNearest(transform.position).node as GridNode;
		Node[]    nodes = graph.nodes;
		int       index = node.GetIndex();
		
		int[] neighbourOffsets = graph.neighbourOffsets;
		
		List   <GridNode> connections = new List   <GridNode>();
		Queue  <BFNode>   q           = new Queue  <BFNode>  ();
		HashSet<GridNode> visited     = new HashSet<GridNode>();
				
		//BFS
		BFNode t = new BFNode(0, node);
		q.Enqueue(t);
		Node aux;
		
		while (q.Count > 0){
			t = q.Dequeue();
			connections.Add(t.node);
			if (t.depth < _depthOfSight){ //si no está en el límite, agrego nodos
				for (int i = 0; i < 8; i++){ //las 8 conexiones posibles de cada nodo
					index = t.node.GetIndex();

					if(t.node.GetConnection(i)){
						aux = nodes[index + neighbourOffsets[i]];
						if (!(visited.Contains((GridNode)(aux))) && //famoso if de reglón de ancho, warpeado
							isVisibleNode(aux) &&
							aux.walkable)
						{	
							visited.Add((GridNode)aux);
							q.Enqueue(new BFNode(t.depth + 1, (GridNode)aux));
						}
					}
				}
			}
		}
		return connections;		
	}
	
	//check if the node is in a visible distance
	private bool isVisibleNode(Node node){
		return (new Int3(transform.position) -
				node.position).worldMagnitude < _depthOfSight * _nodeSize;
	}
}

//Breadth first nodes
class BFNode{
	public GridNode node;
	public int depth;
	
	public BFNode(int depth, GridNode node){
		this.node  = node;
		this.depth = depth;
	}
}
