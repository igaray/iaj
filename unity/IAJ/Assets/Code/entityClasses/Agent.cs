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
	private float _delta = 0.1f;   			// time between ticks of "simulation"
	public  int   life;
	public  int   _depthOfSight;
		
	private RigidBodyController   _controller;	
	private List<PerceivableNode> nodeList;							// TODO: Borrar. Es para test nomás
	public  List<EObject>         backpack = new List<EObject>();
	public  delegate void ActionFinished(ActionResult result);
	private ActionFinished actionFinished;
	
	public static Agent Create(	GameObject prefab, 
								Vector3 position, 
								IEngine se,
								string description, 
								string name, 
								int lifeTotal) 
	{
		GameObject gameObj = Instantiate(prefab, position, Quaternion.identity) as GameObject;
		Agent      agent   = gameObj.GetComponent<Agent>();
		
		agent.life         = lifeTotal;
		agent._description = description;
		agent._name        = name;
		agent._delta	   = se._delta;
		agent._type		   = "agent";
						  			
		return agent;
	}
	
	public override void Start(){
		base.Start();		
		this._controller = this.GetComponent<RigidBodyController>();	
		InvokeRepeating("execute", 0, _delta);
	}
	
	public void setCallback(ActionFinished s){
		actionFinished = s;
	}
	
	// esto se ejecuta en cada ciclo de "simulación"
	void execute(){		
		nodeList = this.perceptNodes();
		
		// TEST
//		if (!_controller.moving && nodeList.Count > 1){
//			GridNode node = nodeList[UnityEngine.Random.Range(0, nodeList.Count)]._node;
//			_controller.move((Vector3)node.position);	
//			Debug.Log(movePreConf(node.GetIndex()));
//		}
		//position = transform.position;
		// TEST
		
	}	
		
	public bool movePreConf(int node){
		Node actualNode = AstarPath.active.GetNearest(transform.position).node as GridNode;
//		return PerceivableNode.connections(actualNode as GridNode).Contains(node);
		return true;
	}
	
	public void movePosCond(int node){
		_controller.move((Vector3)(AstarPath.active.graphs[0].nodes[node].position));	
	}
	
	// posiblemente innecesario
	public void moveToPosition(Vector3 target){
		_controller.move(target);	
	}
	
	public void stoppedMoving(){
		actionFinished(ActionResult.failure);
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
		string   inside   = building != null ? building._name : "no";
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
		return aux;
	}
	
	public Building inBuilding(){
		return null; //TODO: implementar
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
		GridNode gridNode = AstarPath.active.GetNearest(transform.position).node as GridNode;
		List   <PerceivableNode> connections = new List   <PerceivableNode>();
		Queue  <BFNode>          q           = new Queue  <BFNode>         ();
		HashSet<Node>            visited     = new HashSet<Node>           ();
		//Breadth First Search
		BFNode t = new BFNode(0, gridNode);
		q.Enqueue(t);
		while (q.Count > 0){
			t = q.Dequeue();
			connections.Add(t.node);
			if (t.depth < _depthOfSight){ //si no está en el límite, agr
				foreach(Node node in t.node){
					if (!(visited.Contains(node)) && //famoso if de reglón de ancho, warpeado
						isVisibleNode(node) &&
						node.walkable)
					{	
						visited.Add(node);
						q.Enqueue(new BFNode(t.depth + 1, node as GridNode));
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
	public PerceivableNode node;
	public int             depth;
	
	public BFNode(int depth, PerceivableNode node){
		this.node  = node;
		this.depth = depth;
	}
	
	public BFNode(int depth, GridNode node){
		this.node  = new PerceivableNode(node);
		this.depth = depth;
	}
}
