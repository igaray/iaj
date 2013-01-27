using UnityEngine;
using System.Collections;

public class Gold : EObject {
	
	// creates an instance of the prefab Gold, that has this script attached, 
	// and returns the instance of this class that is attached to the object,
	// initialized
	public static Gold Create(	Object  prefab, 
								Vector3 position, 
								string  description, 
								string  name, 
								int 	weigth) {
		
		GameObject gameObj = Instantiate(prefab, position, Quaternion.identity) as GameObject;
		Gold       gold    = gameObj.GetComponent<Gold>();
		
		gold.weigth       = weigth;
		gold.description  = description;
		gold.name         = name;
		
		return gold;
	}
}
