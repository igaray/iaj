using UnityEngine;
using System.Collections;

public class Gold : EObject {
	public static Gold Create(	Object  prefab, 
								Vector3 position, 
								Engine  engine,
								string  description, 
								string  name, 
								int 	weigth) {
		
		GameObject gameObj = Instantiate(prefab, position, Quaternion.identity) as GameObject;
		Gold       gold    = gameObj.GetComponent<Gold>();
		
		gold.weigth       = weigth;
		gold.description  = description;
		gold.name         = name;
		gold.engine		  = engine;
		
		return gold;
	}
}
