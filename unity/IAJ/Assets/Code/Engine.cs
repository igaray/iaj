using UnityEngine;
using System.Collections;

public class Engine : MonoBehaviour {
	
	public GameObject agent;
	public GameObject gold;
	public float delta = 0.1f;
	
	void Start () {
//		Instantiate(prefab, new Vector3(20, 23, 1), Quaternion.identity);
//		Instantiate(prefab, new Vector3(30, 10, 1), Quaternion.identity);
//		Instantiate(prefab, new Vector3(10, 30, 1), Quaternion.identity);
		
		Agent.Create(agent, new Vector3(20, 23, 1), this, "", "agent1", 100);
		Agent.Create(agent, new Vector3(30, 10, 1), this, "", "agent2", 100);
		Agent.Create(agent, new Vector3(10, 30, 1), this, "", "agent3", 100);
		Agent.Create(agent, new Vector3(22, 2, 1),  this, "", "agent4", 100);
		Agent.Create(agent, new Vector3(13, 10, 1), this, "", "agent5", 100);
		Agent.Create(agent, new Vector3(14, 30, 1), this, "", "agent6", 100);
		
		Gold.Create (gold,  new Vector3(6,  0, 15), this, "", "gold1",  2);
		Gold.Create (gold,  new Vector3(22, 0, 4 ), this, "", "gold2",  2);
		Gold.Create (gold,  new Vector3(27, 0, 15), this, "", "gold3",  2);
		Gold.Create (gold,  new Vector3(2,  0, 4 ), this, "", "gold4",  2);
		Gold.Create (gold,  new Vector3(5,  0, 22), this, "", "gold5",  2);
		Gold.Create (gold,  new Vector3(26, 0, 10), this, "", "gold6",  2);
		Gold.Create (gold,  new Vector3(12, 0, 18), this, "", "gold7",  2);
		Gold.Create (gold,  new Vector3(12, 0, 18), this, "", "gold8",  2);
		Gold.Create (gold,  new Vector3(26, 0, 6 ), this, "", "gold9",  2);
		Gold.Create (gold,  new Vector3(11, 0, 4 ), this, "", "gold10", 2);
	}
	
	// Update is called once per frame
//	void Update () {
//	
//	}
}
