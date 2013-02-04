using UnityEngine;
using System.Collections;

public abstract class EObject : Entity {
	
	public int weigth;	// the children of this class must assign this
						// however, it's not final.
	
	public override Hashtable perception(){
		Hashtable p = base.perception();
		p["weigth"] = this.weigth;
		return p;
	}
}
