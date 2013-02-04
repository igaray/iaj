using System;
using System.Collections;
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
	
	public Hashtable perception ()
	{
		Hashtable d = new Hashtable();
		
		d["name"]     = _node.GetIndex(); 
		d["position"] = (Vector3)_node.position;
		// TODO: agregar conexiones?
		return d;
	}
}


