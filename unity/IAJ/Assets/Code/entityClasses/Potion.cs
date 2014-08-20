using UnityEngine;
using System.Collections;

public class Potion : EObject {
	
	public override void Start(){
		base.Start();
		this._type   = "potion";
		this.weigth  = 2;
		this._engine = SimulationState.getInstance();
		if (!createdByCode){
			_engine.addPotion(this);
		}
	}		
	
	public static Potion Create(Vector3 position) {
		
		Object  prefab = SimulationState.getInstance().potionPrefab;			
		GameObject gameObj = Instantiate(prefab, position, Quaternion.identity) as GameObject;	
		Potion       potion    = gameObj.GetComponent<Potion>();
		potion.createdByCode = true;
		potion._type 		   = "potion";
		potion._engine	   = SimulationState.getInstance();		
		potion._engine.addPotion(potion);
		return potion;
	}
	
	public override string toProlog(){		
		string aux = base.toProlog();
		return aux + "])";
	}
}
