using System;
using UnityEngine;

public class AgentState {
    
    public string                name;
    public int                   agentID;
	public Action                lastAction;
    public ActionResult          lastActionResult;
	public Agent				 agentController;
		
    public MailBox<Action>       actions;
    public MailBox<ActionResult> results;
    public MailBox<Percept>      percepts;
	
    public AgentState(SimulationState ss, string name, int id, Vector3 spawnSite, GameObject prefab) {
        this.agentID           = id;
		this.lastAction        = new Action();
        this.lastActionResult  = ActionResult.success;
        this.actions           = ss.readyActionQueue;
        this.results           = new MailBox<ActionResult>(false);
        this.percepts          = new MailBox<Percept>(false);
        this.agentController   = Agent.Create(prefab, spawnSite, ss, "", name, 100);  
		
		this.agentController.setCallback(results.Send);
		this.agentController.agentState = this;
	}

    public String toString() {
        return String.Format("Agent {0} \n    id:  {1} \n    pos: {2}\n    LAR: {3}\n", 
            name, agentID, agentController.position.ToString(), lastActionResult);
    }
}
