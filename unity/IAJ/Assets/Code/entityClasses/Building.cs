using UnityEngine;
using System.Collections;
using Pathfinding.Nodes;
using Pathfinding;

public abstract class Building : Entity {

	public Bounds _bounds;
	private Node node;
	
	public override void Start () {
		base.Start();
		node = AstarPath.active.GetNearest(this.transform.position).node;
		_bounds = new Bounds(transform.position, new Vector3(1.5f, 3f, 1.5f));
		//_bounds.extents = new Vector3(1.5f, 3f, 1.5f);
		//gameObject.collider.enabled = false;
	}
	
	public bool isInside(Vector3 p){
		Node pNode = AstarPath.active.GetNearest(p).node;		
		return pNode == node;
	}
	
	public Node getNode() {
		return node;
	}
	
}
