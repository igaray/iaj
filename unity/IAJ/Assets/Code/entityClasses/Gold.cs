using UnityEngine;
using System.Collections;

public class Gold : EObject {
	
	public override void Start(){
		this._type = "gold";
	}
	
	public static Gold Create(	Object  prefab, 
								Vector3 position, 
								Engine  engine,			//this may not be necessary
								string  description, 
								string  name, 
								int 	weigth) {
		
		GameObject gameObj = Instantiate(prefab, position, Quaternion.identity) as GameObject;
		Gold       gold    = gameObj.GetComponent<Gold>();
		
		gold.weigth        = weigth;
		gold._description  = description;
		gold._name         = name;
		
		return gold;
	}
	
	public override string toProlog(){
		string aux = base.toProlog();
		return aux + "])";
	}
}
