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
using Pathfinding.Nodes;
using Pathfinding;

// Runs within Unity3D
public class SimulationState : IEngine{
			
    public  SimulationConfig             config;
    public  Dictionary<string, int>      agentIDs; // nombres -> ids
    public  Dictionary<int, AgentState>  agents;   // ids -> estado interno del agente
//    public Dictionary<int, ObjectState> objects;
    public  MailBox<Action>              readyActionQueue;
    public  MailBox<PerceptRequest>      perceptRequests;
	public  MailBox<InstantiateRequest>  instantiateRequests;
	public  MailBox<string>              stdout;
	public  GameObject					 goldPrefab;
	public 	GameObject					 potionPrefab;
	private float	   					 delta;
	private Dictionary <string, EObject>	 objectsIn;
	public  Dictionary <string, Inn>	 inns;
	public  Dictionary <string, Grave>	 graves;
	public  Dictionary <int, Inn>	 nodeToInn;
	private Dictionary <string, float>   actionDurationsDic;
	public  IDictionary<string, EObject>	 objects
	{
		get
		{
			return objectsIn;
		}
		set
		{
			objectsIn = value as Dictionary <string, EObject>;
		}
	}	
	
	public float _delta
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
	
	public IDictionary<string, float> actionDurations
	{
		get
		{
			return config.actionDurations;
		}
	}
	public bool test
	{
		get
		{
			return false;
		}
	}
	
	public int gameTime = 0;
	private System.Timers.Timer timer;
	
		
    public SimulationState(string ConfigurationFilePath, GameObject gold = null, GameObject potion = null) {
		config               = new SimulationConfig();		
		agentIDs             = new Dictionary<string, int>       ();
		agents               = new Dictionary<int,    AgentState>();		

        //objects              = new Dictionary<int, ObjectState>();
		objects				 = new Dictionary<string, EObject>      ();
		inns				 = new Dictionary<string, Inn>      ();
		graves				 = new Dictionary<string, Grave>      ();
		nodeToInn		     = new Dictionary<int, Inn>();
        readyActionQueue     = new MailBox   <Action>            (true);
        perceptRequests      = new MailBox   <PerceptRequest>    (true);
		instantiateRequests  = new MailBox   <InstantiateRequest>(true);
        stdout               = new MailBox   <string>            (true);
		goldPrefab 			 = gold;
		potionPrefab		 = potion;
		_delta               = 0.1f;		
    }
	
	/*
	 * In the future SimulationState should implement the singleton pattern
	 * */
	public static SimulationState getInstance() {
		return SimulationEngineComponentScript.ss;
	}
	
    public bool executableAction(Action action) {
        bool result = false;
		Agent agent = agents[action.agentID].agentController;
		try {
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
                result = agent.movePreConf(action.targetNodeID);
                break;
            }
            case ActionType.attack: {                
				//SimulationEngineComponentScript.ss.stdout.Send("objectID = "+action.objectID+" ");
				//SimulationEngineComponentScript.ss.stdout.Send("targetAgent = "+agents[agentIDs[action.objectID]].agentController+" ");				
                result = agent.attackPreCon(agents[agentIDs[action.objectID]].agentController);				
                break;
            }
            case ActionType.pickup: {
                result = agent.pickupPreCon(objects[action.objectID]);
                break;
            }
            case ActionType.drop: {
                result = agent.dropPreCon(objects[action.objectID]);;
                break;
            }
			case ActionType.cast_spell: {				
				if (action.description.Equals("open")) {					
					SimulationState.getInstance().stdout.Send(agent.ToString());
					SimulationState.getInstance().stdout.Send(action.objectID);				
					result = agent.castSpellOpenPreCon(graves[action.targetID], objects[action.objectID]);								
				} else if (action.description.Equals("sleep"))
					result = agent.castSpellSleepPreCon(agents[agentIDs[action.targetID]].agentController, objects[action.objectID]);													
				break;
				}
			}
		} catch (System.Collections.Generic.KeyNotFoundException e) {
             SimulationState.getInstance().stdout.Send(String.Format("Key not found in SimulationState.executableAction. Precondition of action {0} Failed: {1} ", action.type.ToString(), e.ToString()));
			 Debug.LogError(e.ToString());
        }        
        return result;
    }

    public void applyActionEffects(Action action) {
		Agent agent = agents[action.agentID].agentController;
        switch (action.type) {
            case ActionType.noop: {
				agent.noopPosCon();
                break;
            }
            case ActionType.move: {
                agent.movePosCond(action.targetNodeID);
                break;
            }
            case ActionType.attack: {
                agent.attackPosCon(agents[agentIDs[action.objectID]].agentController);
                break;
            }
            case ActionType.pickup: {
				agent.pickupPosCon(objects[action.objectID]);
                break;
            }
            case ActionType.drop: {
                // TODO
                // remove the object from the agent's inventory
                // update the object's position
				agent.dropPosCon(objects[action.objectID]);
                break;
            }
			case ActionType.cast_spell: {
				if (action.description.Equals("open"))
					agent.castSpellOpenPosCon(graves[action.targetID], objects[action.objectID]);
				if (action.description.Equals("sleep"))
					agent.castSpellSleepPosCon(agents[agentIDs[action.targetID]].agentController, objects[action.objectID]);
				break;
			}
        }        		
		if (!action.type.Equals(ActionType.move))
			agent.stopActionAfter(agent.actionDurations[action.type.ToString()]);								        
		Dictionary<SimulationConfig.AgAttributes, float> actionEffects = SimulationConfig.actionEffectsOnAttributes[action.type];
		foreach (SimulationConfig.AgAttributes attr in actionEffects.Keys) {
			if (attr.Equals(SimulationConfig.AgAttributes.HP))
				agent.addLife((int)actionEffects[attr]);
			else if (attr.Equals(SimulationConfig.AgAttributes.XP))
				agent.addSkill((int)actionEffects[attr]);
		}
    }		
	
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
	
	public void addInn(Inn inn){				
		inn._name   = "inn" + inns.Count;
		inns[inn._name]  = inn;
		nodeToInn[(inn.getNode() as GridNode).GetIndex()] = inn;		
	}
	
	public void addGrave(Grave grave){				
		SimulationState.getInstance().stdout.Send("entro addGrave");
		grave._name   = "grave" + graves.Count;
		SimulationState.getInstance().stdout.Send("name: "+grave._name);
		graves[grave._name]  = grave;		
	}
	
	public int getTime() {
		return gameTime;
	}
	
	/*
	public static HashSet<Entity> has(Entity container) {
		return hasDict[container];
	}
	
	public static void addHas(Entity container, Entity contained) {				
		has(container).Add(contained);
	}
	
	public static void removeHas() {		
		has(container).Remove(contained);			
	}
	*/
}
