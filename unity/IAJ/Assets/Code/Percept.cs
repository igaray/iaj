using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Percept {
	
	private List<IPerceivableEntity> elements = new List<IPerceivableEntity>();
	public  string                   p, aux;
	public  List<string>             auxList;
	
	// the perception is created and generated here
	public Percept(SimulationState state, int agentID, bool inProlog = true) {
		
		if (inProlog){
			auxList = new List<string>();
			
			if (state.agents.ContainsKey(agentID)) {
				state.agents[agentID].agentController.perceive(this); 
				
				foreach(IPerceivableEntity e in elements){				
					aux = e.toProlog();	
					auxList.Add(aux);
				}
				auxList.Add(state.agents[agentID].agentController.selfProperties());
				p = PrologList.AtomList<string>(auxList);
				
			}
			else {
				Debug.LogError("Percept creation fail.");
			}
		}
		else{
			// xml
			throw new NotImplementedException();
		}
		
	}

	public override string ToString(){
		
		return p + ".\r"; 
		
		//return "percept(position(1,2,3), agents([]), objects([]), inventory([])).\r";
	}
	
	// TODO: revisar si esto va
	public void addEntities(IPerceivableEntity e){
		elements.Add(e);
	}
	
	public void addEntitiesRange(List<IPerceivableEntity> list){
		elements.AddRange(list);
	}
}
