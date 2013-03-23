using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public enum ActionType {
    unknown, goodbye, noop, move, attack, pickup, drop
};

public enum ActionResult {
    success, failure
};

public class Action {
    
    public ActionType type     = ActionType.noop;
    public string     actionID = "0"; // ID of the action, provided by agent.
    public int        agentID  = 0;   // ID of the agent performing the action.
    public int        targetID = 0;   // ID of an agent that is the recipient of the action.
    public string     objectID = "";
    public float      duration = 0f;

    public Action() {
    }

    public Action(SimulationState ss, int aid, string xml) {

        // It might be the case that the string passed in null or 
        // empty, in which case a default action of type noop is made.
        if ((xml == null) || (xml == "")) {
            this.type = ActionType.noop;
        }
        else {
            // TODO finish implemeting
            XmlDocument document;
            string      type_str;

            try {
                document = new XmlDocument();
                document.LoadXml(xml);

                type_str = document.SelectSingleNode("/action/type").InnerText;
                this.agentID  = aid;
                this.actionID = document.SelectSingleNode("/action/id").InnerText;
                this.duration = ss.config.actionDurations[type_str];
                
                if (type_str == "goodbye") {
                    this.type = ActionType.goodbye;
                }
                if (type_str == "noop") {
                    this.type = ActionType.noop;
                }
                if (type_str == "move") {
                    this.type     = ActionType.move;
                    //this.position = new Position(document.SelectSingleNode("/action/position").Value);
					
					//Acá no me anduvo Value, y si InnerText. No sé por qué
					this.targetID = Convert.ToInt32(document.SelectSingleNode("/action/position").InnerText);
                }
                if (type_str == "attack") {
                    this.type     = ActionType.attack;
                    this.targetID = Convert.ToInt32(document.SelectSingleNode("/action/agent/id").Value);
                }
                if (type_str == "pickup") {
                    this.type     = ActionType.pickup;
                    this.objectID = document.SelectSingleNode("/action/object/id").InnerText;
					
                }
                if (type_str == "drop") {
                    this.type     = ActionType.drop;
                    this.objectID = document.SelectSingleNode("/action/object/id").InnerText;
                }
            }
            catch (System.Xml.XmlException) {
                // TODO somehow signal failure to agent
                ss.stdout.Send(String.Format("AC {0}: Error: bad action xml.", agentID));
            }
        }
    }
}
