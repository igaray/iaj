using UnityEngine;
using System.Collections;

public class Gold : EObject {
	
	public override void Start(){
		base.Start();
		this._type   = "gold";
		this.weigth  = 2;
		this._engine = (GameObject.FindGameObjectWithTag("GameController").GetComponent(typeof(IEngineComponent))
			as IEngineComponent).engine;
		if (!createdByCode){
			_engine.addGold(this);
		}
	}
	
	public static Gold Create(	Object  prefab, 
								Vector3 position, 
								IEngine engine,
								string  description, 
								string  name, 
								int 	weigth) {
		
		GameObject gameObj = Instantiate(prefab, position, Quaternion.identity) as GameObject;
		Gold       gold    = gameObj.GetComponent<Gold>();
		
		gold.weigth        = weigth;
		gold._description  = description;
		gold._name         = name;
		gold.createdByCode = true;
		gold._engine	   = engine;
		
		return gold;
	}
	
	public override string toProlog(){
		string aux = base.toProlog();
		return aux + "])";
	}
}
