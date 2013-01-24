using UnityEngine;
using System.Collections;

public class EObject : Entity {
	
	public int weigth;
	
	public EObject(int weigth, string description, string type, string name) 
		: base(description, type, name, true) 
	{
		this.weigth = weigth;
	}
	
	public override Hashtable perception(){
		Hashtable p = base.perception();
		p["weigth"] = this.weigth;
		return p;
	}
}
