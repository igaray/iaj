using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Pathfinding.Nodes;
using Pathfinding;

public class Agent : Entity {
	
//	public static Object prefab = Resources.Load("Prefabs/Capsule");
	
	private int   lifeTotal = 100;
	private float nodeSize; //
	public  int   life;
	public  int   depthOfSight;
	
	public List<EObject> backpack = new List<EObject>();
	
	public static Agent Create(	Object prefab, 
								Vector3 position, 
								string description, 
								string name, 
								int lifeTotal) 
	{
		GameObject gameObj = Instantiate(prefab, position, Quaternion.identity) as GameObject;
		Agent      agent   = gameObj.GetComponent<Agent>();
		
		agent.life         = lifeTotal;
		agent.description  = description;
		agent.name         = name;
		
		agent.nodeSize     = 2; //hardcodeado. Traté de hacerlo andar dinámicamente, pero no pude.
						   //Es el mismo número que el node size definido en el objeto A*, por IDE	
		return agent;
	}
	
	void Update(){
		
		//this.perceptNodes();
		perceptObjects("gold");
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
		//TODO: 
		// - add object to the game
		this.backpack.Remove(obj);
	}
	
	public override Hashtable perception(){
		Hashtable p = base.perception();
		p["life"]      = life;
		p["lifeTotal"] = lifeTotal;
		p["backpack"]  = backpack;
		
		p["agentsSeen"] = perceptObjects("agent");
		p["goldSeen"]   = perceptObjects("gold");
		//TODO:
		// - cast spherecast
		return p;
	}
	
	private List<GameObject> perceptObjects(string type){
		Collider[] colliders = 
			Physics.OverlapSphere(this.transform.position, depthOfSight,
				1 << LayerMask.NameToLayer("perception")); // usa mascaras, con el << agregas con BITWISE-OR
		List<GameObject> aux = new List<GameObject>();
		
    	foreach (Collider hit in colliders) {		
			if (hit.tag == type)
				aux.Add(hit.gameObject);
    	}
		if (aux.Count > 0)
			Debug.Log(aux.Count);
		return aux;
	}
	
	public void moveToNode(int node){
		
	}
	
	private List<GridNode> perceptNodes(){
		GridGraph graph = AstarPath.active.astarData.gridGraph;
		GridNode  node  = AstarPath.active.GetNearest(transform.position).node as GridNode;
		Node[]    nodes = graph.nodes;
		int       index = node.GetIndex();
		
		int[] neighbourOffsets = graph.neighbourOffsets;
		
		List<GridNode>    connections = new List<GridNode>();
		Queue<BFNode>     q           = new Queue<BFNode>();
		HashSet<GridNode> visited     = new HashSet<GridNode>();
				
		//BFS
		BFNode t = new BFNode(0, node);
		q.Enqueue(t);
		Node aux;
		
		while (q.Count > 0){
			t = q.Dequeue();
			connections.Add(t.node);
			if (t.depth < depthOfSight){ //si no está en el límite, agrego nodos
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
				node.position).worldMagnitude < depthOfSight * nodeSize;
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
