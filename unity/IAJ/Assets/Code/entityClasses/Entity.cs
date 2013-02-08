using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding.Nodes;
using Pathfinding;

public abstract class Entity : MonoBehaviour, IPerceivableEntity {

	public string    _description;
	public string    _type;
	//public Engine    _engine;
	public GridGraph _graph;
	public string    _name;
	public Vector3   position;
	
	private Transform _transform;
	
	public virtual void Start(){
		_transform = this.transform;
		_graph     = AstarPath.active.astarData.gridGraph;
		//_name      = this.gameObject.name;
		position   = this.transform.position;
	}
	
	
	
	public virtual Dictionary<string, System.Object> perception(){
		Dictionary<string, System.Object> p = new Dictionary<string, System.Object>();
		p["name"]        = _name; 					// this is the same name from the monoBehaviour
		//p["type"]        = _type;					// I don't know if this is necessary
		//p["description"] = _description; 			// nor this
		p["position"] 	 = Vector3ToProlog(position);
		p["nearestNode"] = (AstarPath.active.GetNearest(position).node as GridNode).GetIndex();
		
		return p;
	}
	
	static public string Vector3ToProlog(Vector3 v){
		return String.Format("{0}, {1}, {2}", v.x, v.y, v.z);
	}
	
}
