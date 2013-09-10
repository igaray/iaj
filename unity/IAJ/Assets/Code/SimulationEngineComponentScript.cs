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

/******************************************************************************/
public class SimulationEngineComponentScript : MonoBehaviour, IEngineComponent{

    // SIMULATION
    public static SimulationState  ss;
    private SimulationEngine se;

    // UNITY GUI
    public  GUISkin          mySkin;
    public  Vector2          scrollPosition;
    public  string           outputText = "";
	public  GameObject		 agentPrefab, goldPrefab;
	public  IEngine		  	 engine
	{
		get
		{
			return ss;
		}
	}

    
    // Use this for initialization
    void Awake () {
        ss = new SimulationState("C:\\config.xml", goldPrefab);
        se = new SimulationEngine(ss);

        InvokeRepeating( "DoWork", 0, 0.1f );
		
		//se.instantiateDummyAgent("dummy1", agentPrefab);
		//se.instantiateDummyAgent("dummy2", agentPrefab);
    }
    
    // Update is called once per frame
    void Update () {
    }

    void OnGUI () {
        GUI.skin = mySkin;
        
		/*
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(512), GUILayout.Height(512));
        GUILayout.Box(outputText);
        GUILayout.EndScrollView();
		*/
		
        if (GUI.Button(new Rect(4,682,128,20), "Start")) {			
            ss.stdout.Send("Starting simulation engine...\n");
            se.start();
        }

        if (GUI.Button(new Rect(132,682,128,20), "Stop")) {
            ss.stdout.Send("Stopping simulation engine...\n");
            se.stop();
        }		
		
    }
	
	//public GUIStyle timeLabel;
	
	void WindowFunction(int windowId) {				
		GUILayout.BeginVertical();
			GUILayout.Label(SimulationState.getInstance().gameTime.ToString());
			foreach (AgentState agentState in SimulationState.getInstance().agents.Values) {
				AgentPanel(agentState.agentController);
			}
		GUILayout.EndVertical();
	}
	
	void AgentPanel(Agent agent) {
		//GUILayout.BeginArea(new Rect(0, 45, 160, 270));
		GUILayout.BeginVertical();
			//GUILayout.Box(new Rect(0,0,160,270));
			GUILayout.Box(agent._name);			
			//GUILayout.Label(new Rect(0,0,160,40), agent._name);
			//GUILayout.Label(agent._name);
		GUILayout.EndVertical();
	}

    void OnApplicationQuit() {
        se.stop();
    }


    void DoWork() {

        // Get all the text out of the queue.
        string str;
        while (ss.stdout.NotEmpty()) {
            if (ss.stdout.NBRecv(out str)) {
                outputText += str;
            }
        }
		
		se.dynamicEnvUpdate();
        se.generatePercepts();
        se.handleActions();
        se.instantiateAgents(agentPrefab);
    }
}
