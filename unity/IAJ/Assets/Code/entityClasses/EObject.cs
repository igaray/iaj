using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class EObject : Entity {
	
	public int  weigth = 0;				// the children of this class must assign this
			 							// however, it's not final.
	public bool createdByCode = false; 	// if the object was created by GUI, this should stay false.
										// otherwise, the Create method ticks it up

	public Texture2D icon;
	
//	public void Start(){
//		_name = _type + 
//	}
	
	public override Dictionary<string, System.Object> perception(){
		Dictionary<string, System.Object> p = base.perception();
		p["weigth"] = this.weigth;
		return p;
	}
	
	public bool isAtGround() {
		return gameObject.activeSelf;
	}
	
	public override string toProlog(){
		string aux = base.toProlog();		
		return aux + string.Format("[[weight, {0}]", this.weigth);
	}

	public Texture getIcon() {
		return icon;
	}
}
