using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Percept {
	
	private List<List<IPerceivableEntity> > elements = new List<List<IPerceivableEntity> >();
	public  string       p;
	public  List<string> auxList;
	
	// the perception is created and generated here
	public Percept(SimulationState state, int agentID) {
		
		Dictionary<string,System.Object> dic;	
		auxList = new List<string>();
		
		if (state.agents.ContainsKey(agentID)) {
			state.agents[agentID].agentController.perceive(this); 
			foreach (List<IPerceivableEntity> list in elements){			
				foreach(IPerceivableEntity e in list){				
					dic = e.perception();	
					
					foreach(KeyValuePair<string, System.Object> entry in dic){
						//Debug.LogError(String.Format("{0}({1})", entry.Key, entry.Value.ToString())); 
						auxList.Add(String.Format("{0}({1})", entry.Key, entry.Value.ToString())); 
					}
					
				}
			}
			p = (new PrologList<string>(auxList)).ToString();
			Debug.LogError(p); 
		}
		else {
			Debug.LogError("Percept creation fail.");
		}
		
	}

	public string toProlog() {
		return p + ".\r"; 
		
		//return "percept(position(1,2,3), agents([]), objects([]), inventory([])).\r";
	}
	
	public void addEntities(List<IPerceivableEntity> e){
		elements.Add(e);
	}
}

public class PrologList<T>{
	
	public List<T> list;
	
	public PrologList(List<T> l){
		list = l;
	}
	
	public override string ToString ()
	{
		string aux = "[";
		foreach(T e in list){
			aux += e.ToString() + ",";
		}	
		aux = aux.TrimEnd(",".ToCharArray()) + "]";
		return aux;
	}
}

