using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

// This class will be erased, and replaced by the real engine
public class Engine : MonoBehaviour, IEngine, IEngineComponent {
	
	public  GameObject  			   agent, gold;
	public  Inn				           inn;
	private float	   				   delta;
	private Dictionary <string, EObject>  objectsIn;
	private Dictionary <string, float> durations;
	public static Dictionary <string, int> actionCost;
	public  IEngine					   engine
	{
		get
		{
			return this;
		}
	}
	
	public  IDictionary<string, EObject> objects
	{
		get
		{
			return objectsIn;
		}
		set
		{
			objectsIn = value as Dictionary <String, EObject>;
		}
	}
	
	public  float      _delta
	{
		get
		{
			return delta;
		}
		set
		{
			delta = value;
		}
	}
	
	public  IDictionary <string, float> actionDurations
	{
		get 
		{
			return durations;
		}
	}
	public  bool test
	{
		get
		{
			return true;
		}
	}
		
	
	private Agent[] agents = new Agent[6];
	
	void Awake () {
		
		_delta = 0.1f;
		
		objects = new Dictionary <String, EObject>();
		
		durations = new Dictionary<string, float>();
		durations["pickup"] = 0.5f;
		durations["drop"]   = 0.5f;
		
		agents [0] = Agent.Create(agent, new Vector3(20, 23, 1), this, "", "agent1", 1000);
		agents [1] = Agent.Create(agent, new Vector3(30, 10, 1), this, "", "agent2", 1000);
		agents [2] = Agent.Create(agent, new Vector3(10, 30, 1), this, "", "agent3", 1000);
		agents [3] = Agent.Create(agent, new Vector3(22, 2, 1),  this, "", "agent4", 1000);
		agents [4] = Agent.Create(agent, new Vector3(13, 10, 1), this, "", "agent5", 1000);
		agents [5] = Agent.Create(agent, new Vector3(14, 30, 1), this, "", "agent6", 1000);
				
//		coins["gold1"]  = Gold.Create (gold,  new Vector3(6,  0, 15), this, "", "gold1",  2);
//		coins["gold2"]  = Gold.Create (gold,  new Vector3(22, 0, 4 ), this, "", "gold2",  2);
//		coins["gold3"]  = Gold.Create (gold,  new Vector3(27, 0, 15), this, "", "gold3",  2);
//		coins["gold4"]  = Gold.Create (gold,  new Vector3(2,  0, 4 ), this, "", "gold4",  2);
//		coins["gold5"]  = Gold.Create (gold,  new Vector3(5,  0, 22), this, "", "gold5",  2);
//		coins["gold6"]  = Gold.Create (gold,  new Vector3(26, 0, 10), this, "", "gold6",  2);
//		coins["gold7"]  = Gold.Create (gold,  new Vector3(12, 0, 18), this, "", "gold7",  2);
//		coins["gold8"]  = Gold.Create (gold,  new Vector3(12, 0, 18), this, "", "gold8",  2);
//		coins["gold9"]  = Gold.Create (gold,  new Vector3(26, 0, 6 ), this, "", "gold9",  2);
//		coins["gold10"] = Gold.Create (gold,  new Vector3(11, 0, 4 ), this, "", "gold10", 2);

		
		inn = GameObject.FindWithTag ("inn").GetComponent<Inn>();
	}
	
	// implementación potencialmente bugosa
	public void addGold(Gold gold){	
		string name = "gold" + objects.Count;
		gold._name  = name;
		objects[name] = gold;
	}
	
	public void addPotion(Potion potion){		
		string name = "p" + objects.Count;
		potion._name  = name;
		objects[name] = potion;
	}
	
//	void Update () {
//		Transform p;
//		foreach(Agent a in agents){
//			p = a.transform;
//			if (inn.isInside(p.position))
//				Debug.Log(a.name + " is inside the inn");
//		}
//	}
}
