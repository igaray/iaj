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

    public  Dictionary<string, string>   config;
    public  Dictionary<string, int>      agentIDs; // nombres -> ids
    public  Dictionary<int, AgentState>  agents;   // ids -> estado interno del agente
//    public Dictionary<int, ObjectState> objects;
    public  MailBox<Action>              readyActionQueue;
    public  MailBox<PerceptRequest>      perceptRequests;
	public  MailBox<InstantiateRequest>  instantiateRequests;
	public  MailBox<string>              stdout;
	public  GameObject					 goldPrefab;
	private float	   					 delta;
	private Dictionary <String, Gold>	 coinsIn;
	public  IDictionary<String, Gold>	 coins
	{
		get
		{
			return coinsIn;
		}
		set
		{
			coinsIn = value as Dictionary <String, Gold>;
		}
	}
	
	public  float   				     _delta
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
		

    public SimulationState(string ConfigurationFilePath, GameObject gold = null) {
        config               = new Dictionary<string, string>();
		agentIDs             = new Dictionary<string, int>();
		agents               = new Dictionary<int, AgentState>();
        //objects              = new Dictionary<int, ObjectState>();
		coins				 = new Dictionary <String, Gold>();
        readyActionQueue     = new MailBox<Action>(true);
        perceptRequests      = new MailBox<PerceptRequest>(true);
		instantiateRequests  = new MailBox<InstantiateRequest>(true);
        stdout               = new MailBox<string>(true);
		goldPrefab 			 = gold;
		_delta               = 0.1f;

        /*
        XmlDocument document = new XmlDocument();
        document.Load(new StreamReader("config.xml"));

        config["simulation_duration"] = document.SelectSingleNode("/config/simulation_duration").InnerText;
        //config["rows"] = document.SelectSingleNode("/config/size/rows").InnerText;
        //config["cols"] = document.SelectSingleNode("/config/size/cols").InnerText;
        //config["vision_length"] = document.SelectSingleNode("/config/vision_length").InnerText;
        //config["unconscious_time"] = document.SelectSingleNode("/config/unconscious_time").InnerText;

        XmlNodeList xnl = document.SelectNodes("/config/actions/action");
        foreach (XmlNode action in xnl) {
            string name     = action.Attributes["name"].Value;
            string duration = action.Attributes["duration"].Value;
            config["action_duration_"+name] = duration; 
        }


        stdout.Send("Loaded the following configuration:");
        foreach (KeyValuePair<string, string> pair in config) {
            stdout.Send(String.Format("{0}:\t{1}", pair.Key, pair.Value));
        }
        */

        /*
        this is to simulate the end result of reading in the config file
        */
        config["simulation_duration"]    = "10";
        config["action_duration_noop"]   = "1000";
        config["action_duration_move"]   = "1000";
        config["action_duration_pickup"] = "0";
        config["action_duration_drop"]   = "0";
        config["action_duration_attack"] = "1000";
    }

    public bool executableAction(Action action) {
        bool result = false;
		Agent agent = agents[action.agentID].agentController;
        switch (action.type) {
            //case ActionType.: {
            //    result = ;
            //    break;
            //}
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
                // TODO
                // check if the object the agent wants to pick up is in range
                result = false;
                break;
            }
            case ActionType.drop: {
                // TODO
                // check if the object  the agent wants to drop is in its inventory
                result = false;
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
                // TODO
                // add the object to the agent's inventory
                // update's the object's position
                break;
            }
            case ActionType.drop: {
                // TODO
                // remove the object from the agent's inventory
                // update the object's position
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
