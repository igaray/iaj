using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Xml;

// Runs within Unity3D
public class SimulationState : IEngine{

    public  SimulationConfig             config;
    public  Dictionary<string, int>      agentIDs; // nombres -> ids
    public  Dictionary<int, AgentState>  agents;   // ids -> estado interno del agente
    public  MailBox<Action>              readyActionQueue;
    public  MailBox<PerceptRequest>      perceptRequests;
	public  MailBox<InstantiateRequest>  instantiateRequests;
	public  MailBox<string>              stdout;
	public  GameObject					 goldPrefab;
	private float	   					 delta;
	private Dictionary <string, Gold>	 coinsIn;
	private Dictionary <string, float>   actionDurationsDic;
	public  IDictionary<string, Gold>	 coins
    	{
    		get { return coinsIn; }
    		set { coinsIn = value as Dictionary <string, Gold>; }
    	}
	
	public float _delta
    	{
    		get { return delta; }
    		set { delta = value; }
    	}
	
	public IDictionary<string, float> actionDurations
    	{
    		get { return config.actionDurations; }
    	}

	public bool test
    	{
    		get { return false; }
    	}
		
    public SimulationState(string ConfigurationFilePath, GameObject gold = null) {
        config               = new SimulationConfig("config.xml");
		agentIDs             = new Dictionary<string, int>       ();
		agents               = new Dictionary<int,    AgentState>();
		coins				 = new Dictionary<string, Gold>      ();
        readyActionQueue     = new MailBox   <Action>            (true);
        perceptRequests      = new MailBox   <PerceptRequest>    (true);
		instantiateRequests  = new MailBox   <InstantiateRequest>(true);
        stdout               = new MailBox   <string>            (true);
		goldPrefab 			 = gold;
		_delta               = 0.1f;
    }

    public bool executableAction(Action action) {
        bool result = false;
		Agent agent = agents[action.agentID].agentController;
        switch (action.type) {
            case ActionType.noop: {
                result = true;
                break;
            }
            case ActionType.move: {
                result = agent.movePreConf(action.targetID);
                break;
            }
            case ActionType.attack: {
                // TODO
                // check if the target the agent wants to attack is in range
                result = false;
                break;
            }
            case ActionType.pickup: {
                result = agent.pickupPreCon(coins[action.objectID]);
                break;
            }
            case ActionType.drop: {
                result = agent.dropPreCon(coins[action.objectID]);;
                break;
            }
        }
        return result;
    }

    public void applyActionEffects(Action action) {
		Agent agent = agents[action.agentID].agentController;
        switch (action.type) {
            case ActionType.noop: {
                break;
            }
            case ActionType.move: {
                agent.movePosCond(action.targetID);
                break;
            }
            case ActionType.attack: {
                // TODO
                // decrement the target's health
                break;
            }
            case ActionType.pickup: {
				agent.pickupPosCon(coins[action.objectID]);
                break;
            }
            case ActionType.drop: {
                // TODO
                // remove the object from the agent's inventory
                // update the object's position
				agent.dropPosCon(coins[action.objectID]);
                break;
            }
        }
        stdout.Send(String.Format("Agent {0} performs action {1} of type {2}", 
            action.agentID, 
            action.actionID, 
            action.type));
    }
	
	// this might not be necessary
	public void initializeCoins(){
		coins["gold1"]  = Gold.Create (goldPrefab,  new Vector3(6,  0, 15), this, "", "gold1",  2);
		coins["gold2"]  = Gold.Create (goldPrefab,  new Vector3(22, 0, 4 ), this, "", "gold2",  2);
		coins["gold3"]  = Gold.Create (goldPrefab,  new Vector3(27, 0, 15), this, "", "gold3",  2);
		coins["gold4"]  = Gold.Create (goldPrefab,  new Vector3(2,  0, 4 ), this, "", "gold4",  2);
		coins["gold5"]  = Gold.Create (goldPrefab,  new Vector3(5,  0, 22), this, "", "gold5",  2);
		coins["gold6"]  = Gold.Create (goldPrefab,  new Vector3(26, 0, 10), this, "", "gold6",  2);
		coins["gold7"]  = Gold.Create (goldPrefab,  new Vector3(12, 0, 18), this, "", "gold7",  2);
		coins["gold8"]  = Gold.Create (goldPrefab,  new Vector3(12, 0, 18), this, "", "gold8",  2);
		coins["gold9"]  = Gold.Create (goldPrefab,  new Vector3(26, 0, 6 ), this, "", "gold9",  2);
		coins["gold10"] = Gold.Create (goldPrefab,  new Vector3(11, 0, 4 ), this, "", "gold10", 2);
	}
	
	public void addGold(Gold gold){
		string name = "gold" + coins.Count;
		gold._name  = name;
		coins[name] = gold;
	}
}
