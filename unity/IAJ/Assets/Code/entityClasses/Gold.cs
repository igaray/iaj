using UnityEngine;
using System.Collections;

public class Gold : EObject {
	
	public override void Start(){
		base.Start();
		this._type   = "gold";
		this.weigth  = 2;
		this._engine = SimulationState.getInstance();
		if (!createdByCode){
			_engine.addGold(this);
		}
	}	
		
	public static Gold Create(Vector3 position) {
		
		Object  prefab = SimulationState.getInstance().goldPrefab;			
		GameObject gameObj = Instantiate(prefab, position, Quaternion.identity) as GameObject;
		Gold       gold    = gameObj.GetComponent<Gold>();
		gold.createdByCode = true;
		gold._type 		   = "gold";
		gold._engine	   = SimulationState.getInstance();		
		gold._engine.addGold(gold);
		return gold;
	}
	
	public override string toProlog(){
		string aux = base.toProlog();
		return aux + "])";
	}
}
