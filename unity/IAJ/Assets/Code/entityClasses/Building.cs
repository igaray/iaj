using UnityEngine;
using System.Collections;

public abstract class Building : Entity {

	public Bounds _bounds;
	
	public override void Start () {
		base.Start();		
		_bounds = gameObject.collider.bounds;		
		gameObject.collider.enabled = false;
	}
	
	public bool isInside(Vector3 p){
		return _bounds.Contains(p);
	}
}
