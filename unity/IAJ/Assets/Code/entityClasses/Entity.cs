using UnityEngine;
using System.Collections;
using Pathfinding.Nodes;
using Pathfinding;

public abstract class Entity : MonoBehaviour, IPerceivableEntity {

	public string    description;
	public string    type;
	public Engine    _engine;
	public GridGraph _graph;
	
	private Transform _transform;
	
	public virtual void Start(){
		_transform = this.transform;
		graph = AstarPath.active.astarData.gridGraph;
	}
	
	public virtual Hashtable perception(){
		Hashtable p = new Hashtable();
		
		p["name"]        = name; 					// this is the same name from the monoBehaviour
		p["type"]        = type;					// I don't know if this is necessary
		p["description"] = description; 			// nor this
		p["positionX"] 	 = transform.position.x;
		p["positionY"] 	 = transform.position.y;
		p["positionZ"] 	 = transform.position.z;
		p["positionX"] 	 = transform.position.x;
		p["nearestNode"] = (AstarPath.active.GetNearest(transform.position).node as GridNode).GetIndex();
		
		return p;
	}
	
}
