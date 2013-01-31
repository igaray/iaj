using UnityEngine;
using System.Collections;

public abstract class Building : Entity {

	public Vector3 _position;
	public Bounds  _bounds;
	
	void Start () {
		_bounds   = gameObject.collider.bounds;
		gameObject.collider.enabled = false;
		_position = transform.position;
	}
	
	public bool isInside(Vector3 p){
		return _bounds.Contains(p);
	}
}
