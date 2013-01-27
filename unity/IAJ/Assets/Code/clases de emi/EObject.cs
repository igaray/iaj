using UnityEngine;
using System.Collections;

public abstract class EObject : Entity {
	
	public int weigth;

	
	public override Hashtable perception(){
		Hashtable p = base.perception();
		p["weigth"] = this.weigth;
		return p;
	}
}
