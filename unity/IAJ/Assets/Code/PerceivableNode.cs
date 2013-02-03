using System;
using System.Collections;
using Pathfinding;
using Pathfinding.Nodes;
using UnityEngine;

public class PerceivableNode : IPerceivableEntity
{
	public GridNode node;
	
	public PerceivableNode (GridNode node)
	{
		this.node = node;
	}
	
	public Hashtable perception ()
	{
		Hashtable d = new Hashtable();
		
		d["name"]     = node.GetIndex(); 
		d["position"] = (Vector3)node.position;
		// TODO: agregar conexiones?
		return d;
	}
}


