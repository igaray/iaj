using UnityEngine;
using System.Collections;

public class Engine : MonoBehaviour {
	
	public GameObject prefab;
	
	void Start () {
		Instantiate(prefab, new Vector3(20, 23, 1), Quaternion.identity);
		Instantiate(prefab, new Vector3(30, 10, 1), Quaternion.identity);
		Instantiate(prefab, new Vector3(10, 30, 1), Quaternion.identity);
	}
	
	// Update is called once per frame
//	void Update () {
//	
//	}
}
