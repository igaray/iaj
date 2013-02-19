using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

using Pathfinding.Nodes;
using Pathfinding;

[RequireComponent (typeof (RigidBodyController))]

public class Agent : Entity {
	
	public  int   lifeTotal = 100;
	private float _nodeSize = 2; 		// size of the node in the world measure
										// Hardcodeado. Traté de hacerlo andar dinámicamente, pero no pude.
										// Es el mismo número que el node size definido en el objeto A*, por IDE
	private float _delta;   			// time between ticks of "simulation"
	public  int   life;
	public  int   _depthOfSight;
		
	private RigidBodyController   _controller;	
	public  List<EObject>         backpack = new List<EObject>();
	private List<PerceivableNode> nodeList;							// TODO: Borrar. Es para test nomás
	
	public static Agent Create(	GameObject prefab, 
								Vector3 position, 
								SimulationState ss,
								string description, 
								string name, 
								int lifeTotal) 
	{
		GameObject gameObj = Instantiate(prefab, position, Quaternion.identity) as GameObject;
		Agent      agent   = gameObj.GetComponent<Agent>();
		
		agent.life         = lifeTotal;
		agent._description = description;
		agent._name        = name;
		agent._delta	   = ss.delta;
		agent._type		   = "agent";

						  			
		return agent;
	}
	
	public override void Start(){
		base.Start();		
		this._controller = this.GetComponent<RigidBodyController>();		
		InvokeRepeating("execute", 0, _delta);
	}
	
	// esto se ejecuta en cada ciclo de "simulación"
	void execute(){		
		nodeList = this.perceptNodes();
		
		// TEST
		if (!_controller.moving && nodeList.Count > 1)
			_controller.move((Vector3)(nodeList[UnityEngine.Random.Range(0, nodeList.Count)]._node.position));	
		//position = transform.position;
		// TEST
		
	}
	
		
	public void moveToNode(int node){
		_controller.move((Vector3)(nodeList[node]._node.position));	
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
	
	public override Dictionary<string, System.Object> perception(){
		Dictionary<string, System.Object> p = base.perception();
		
		p["life"]       = life;
		p["lifeTotal"]  = lifeTotal;
//		p["backpack"]   = backpack;		
//		p["agentsSeen"] = perceptObjects<Agent>("agent");
//		p["goldSeen"]   = perceptObjects<Gold> ("gold");
		
		return p;
	}
	
	private List<ObjectType> perceptObjects<ObjectType>(string type, string layer = "perception") 
		where ObjectType : Component // cuánta magia
									 // acá restrinjo el tipo pasado por parámetro
		{
		Collider[] colliders = 
			Physics.OverlapSphere(this.transform.position, _depthOfSight,
				1 << LayerMask.NameToLayer(layer)); // usa mascaras, con el << agregas con BITWISE-OR
		List<ObjectType> aux = new List<ObjectType>();
		
    	foreach (Collider hit in colliders) {		
			if (hit.tag == type)
				aux.Add(hit.gameObject.GetComponent<ObjectType>());
    	}
		return aux;
	}
	
	private List<PerceivableNode> perceptNodes(){
		//GridGraph graph = AstarPath.active.astarData.gridGraph;
		GridNode  node  = AstarPath.active.GetNearest(transform.position).node as GridNode;
		Node[]    nodes = _graph.nodes;
		int       index = node.GetIndex();
		
		int[] neighbourOffsets = _graph.neighbourOffsets;
		
		List   <PerceivableNode> connections = new List   <PerceivableNode>();
		Queue  <BFNode>          q           = new Queue  <BFNode>         ();
		HashSet<GridNode>        visited     = new HashSet<GridNode>       ();
				
		//Breadth First Search
		BFNode t = new BFNode(0, node);
		q.Enqueue(t);
		Node aux;
		
		while (q.Count > 0){
			t = q.Dequeue();
			connections.Add(new PerceivableNode(t.node));
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
	
	public void perceive(Percept p){
		
		p.addEntitiesRange(perceptObjects<Agent>("agent").Cast<IPerceivableEntity>().ToList());
		p.addEntitiesRange(perceptObjects<Gold> ("gold") .Cast<IPerceivableEntity>().ToList());
		p.addEntitiesRange(perceptNodes()				 .Cast<IPerceivableEntity>().ToList());
}
	
	public override string toProlog(){
		string aux = base.toProlog();		
		return aux + String.Format("[[life, {0}], [lifeTotal, {1}]])", this.life, this.lifeTotal);
	}
	
	public string selfProperties(bool inProlog = true){
		Building building = inBuilding();
		string   inside   = building != null? building._name : "no";
		string   aux;
		if (inProlog){
			aux = string.Format("selfProperties({0}, {1}, {2}, {3}, {4}, {5}, {6})",
				this._name,
	//			this.lastAction,
	//			this.lastActionResult,
				"todo",
				"todo",
				this.life,
				this.lifeTotal,
				new PrologList(this.backpack).toProlog(),
				inside);
		}
		else{
			throw new NotImplementedException();
		}
	}
	
	public Building inBuilding(){
		return null; //TODO: implementar
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
