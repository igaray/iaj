using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class EObject : Entity {
	
	public int weigth;	// the children of this class must assign this
						// however, it's not final.
	
	public override Dictionary<string, System.Object> perception(){
		Dictionary<string, System.Object> p = base.perception();
		p["weigth"] = this.weigth;
		return p;
	}
}
