using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using Pathfinding.Nodes;
using UnityEngine;

public class PerceivableNode : IPerceivableEntity
{
	public GridNode _node;
	
	public PerceivableNode (GridNode node)
	{
		this._node = node;
	}
	
	public Dictionary<string, System.Object> perception ()
	{
		Dictionary<string, System.Object> d = new Dictionary<string, System.Object>();
		
		d["name"]     = _node.GetIndex(); 
		d["position"] = Entity.Vector3ToProlog((Vector3)_node.position);
		// TODO: agregar conexiones?
		return d;
	}
}


