using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Percept {
	
	public List<List<IPerceivableEntity> > elements = new List<List<IPerceivableEntity> >();
	
	// the perception is created and generated here
	public Percept(SimulationState state, int agentID) {
		/*
		if (state.agents.ContainsKey(agentID)) {
			state.agents[agentID].agentController.perceive(this); 
		}
		else {
			Debug.LogError("Percept creation fail.");
		}
		*/
	}

	public string toProlog() {
		/*
		string aux = "";
		Hashtable dic;
		
		foreach (List<IPerceivableEntity> list in elements){
			foreach(IPerceivableEntity e in list){
				dic = e.perception();
				foreach(DictionaryEntry entry in dic){
					Debug.Log(entry.Key + ": " + entry.Value); //TODO: Generar código Prolog
				}
			}
		}
		return aux; 
		*/
		return "percept(position(1,2,3), agents([]), objects([]), inventory([])).\r";
	}
	
	public void addEntities(List<IPerceivableEntity> e){
		elements.Add(e);
	}
}



