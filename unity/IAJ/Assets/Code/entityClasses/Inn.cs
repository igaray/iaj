using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class Inn : Building {

	public SimulationState ss;
	
	public float healCoefficient;
	
	private Dictionary<Agent, Interval> forbiddenEntry;		
	
	public override void Start(){
		base.Start();
		this._type = "inn";
		this.ss    = (GameObject.FindGameObjectWithTag("GameController").
			GetComponent(typeof(SimulationEngineComponentScript))
			as SimulationEngineComponentScript).engine as SimulationState;
				
		this.ss.addInn(this);
		
		forbiddenEntry = new Dictionary<Agent, Interval>();		
		
	}	
	
	
	public void heal(Agent agent){
		SimulationState.getInstance().stdout.Send(" a ");
		updateForbidden(agent);
		SimulationState.getInstance().stdout.Send(" b ");
		if (isForbidden(agent))
			return;		
		SimulationState.getInstance().stdout.Send(" c ");
		if (agent.life < agent.lifeTotal)
			agent.addLife(Mathf.CeilToInt(agent.lifeTotal * healCoefficient));
			//ss.stdout.Send (Mathf.CeilToInt(agent.lifeTotal * healCoefficient));
			//agent.addLife(Mathf.CeilToInt(1));
		SimulationState.getInstance().stdout.Send(" d ");
		Debug.Log ("Estoy en la posada");
		//ss.stdout.Send("Agente en la posada");
	}
	
	public override string toProlog(){				
		List<string> forbAgents = new List<string>();
		foreach(Agent ag in forbiddenEntry.Keys.ToList()) {
			if (isForbidden(ag))
				forbAgents.Add("["+ag.getPrologId()+","+forbiddenEntry[ag].getEnd()+"]");
		}
		return base.toProlog() + String.Format("[[forbidden_entry, {0}]])", PrologList.AtomList<string>(forbAgents));		
	}
	
	private bool isForbidden(Agent agent) {
		return forbiddenEntry.ContainsKey(agent) && forbiddenEntry[agent].contains(SimulationState.getInstance().getTime());
	}
	
	private void updateForbidden(Agent agent) {		
		cleanForbidden();
		int currentTime = SimulationState.getInstance().getTime();
		if (!forbiddenEntry.ContainsKey(agent)) {				
			//Register as forbidden in the future
			int forbidStart = SimulationState.getInstance().getTime() + getTimeToRecover(agent);
			Interval forbidInterval = new Interval(forbidStart, forbidStart + 200);
			forbiddenEntry[agent] = forbidInterval;			
		}
		
	}
	
	private int getTimeToRecover(Agent agent) {
		int lifeToRecover = agent.lifeTotal - agent.life;
		int timeToRecover = (int)(lifeToRecover / (agent.lifeTotal * healCoefficient));
		return timeToRecover + 10; // +10 to ensure the time is enough
	}
	
/*	private bool entryForbidden(Agent agent) {
		int currentTime = SimulationState.getInstance().getTime();
		if (forbiddenEntry.ContainsKey(agent)) {
			Interval interval = forbiddenEntry[agent];
			if (interval.getEnd() <= currentTime)
					forbiddenEntry.Remove(agent);
			return interval.contains(currentTime);				
		} else {
			int forbidStart = SimulationState.getInstance().getTime() + 50;
			Interval forbidInterval = new Interval(forbidStart, forbidStart + 200);
			forbiddenEntry[agent] = forbidInterval;
			return false;
		}
		
	}
*/
	private void cleanForbidden() {
		int currentTime = SimulationState.getInstance().getTime();
		foreach(Agent ag in forbiddenEntry.Keys) {
			Interval interval = forbiddenEntry[ag];
			if (interval.getEnd() < currentTime)
				forbiddenEntry.Remove(ag);				
		}
	}
}
