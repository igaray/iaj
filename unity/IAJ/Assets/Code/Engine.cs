using UnityEngine;
using System.Collections;

public class Engine : MonoBehaviour {
	
	public GameObject agent;
	public GameObject gold;
	
	void Start () {
//		Instantiate(prefab, new Vector3(20, 23, 1), Quaternion.identity);
//		Instantiate(prefab, new Vector3(30, 10, 1), Quaternion.identity);
//		Instantiate(prefab, new Vector3(10, 30, 1), Quaternion.identity);
		
		Agent.Create(agent, new Vector3(20, 23, 1), "", "agent1", 100);
		Agent.Create(agent, new Vector3(30, 10, 1), "", "agent2", 100);
		Agent.Create(agent, new Vector3(10, 30, 1), "", "agent3", 100);
		
		Gold.Create (gold,  new Vector3(6,  0, 15), "", "gold1",  2);
		Gold.Create (gold,  new Vector3(22, 0, 4 ), "", "gold2",  2);
		Gold.Create (gold,  new Vector3(27, 0, 15), "", "gold3",  2);
		Gold.Create (gold,  new Vector3(2,  0, 4 ), "", "gold4",  2);
		Gold.Create (gold,  new Vector3(5,  0, 22), "", "gold5",  2);
		Gold.Create (gold,  new Vector3(26, 0, 10), "", "gold6",  2);
		Gold.Create (gold,  new Vector3(12, 0, 18), "", "gold7",  2);
		Gold.Create (gold,  new Vector3(12, 0, 18), "", "gold8",  2);
		Gold.Create (gold,  new Vector3(26, 0, 6 ), "", "gold9",  2);
		Gold.Create (gold,  new Vector3(11, 0, 4 ), "", "gold10", 2);
	}
	
	// Update is called once per frame
//	void Update () {
//	
//	}
}
