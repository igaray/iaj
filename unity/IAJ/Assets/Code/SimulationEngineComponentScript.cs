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
public class SimulationEngineComponentScript : MonoBehaviour {

    // SIMULATION
    private SimulationState     ss;
    private SimulationEngine    se;

    // UNITY GUI
    public  GUISkin             mySkin;
    public  Vector2             scrollPosition;
    public  string              outputText = "";
	public  GameObject			agentPrefab;
    
    // Use this for initialization
    void Start () {
        ss = new SimulationState("C:\\config.xml");
        se = new SimulationEngine(ss);

        InvokeRepeating( "DoWork", 0, 0.1f );
    }
    
    // Update is called once per frame
    void Update () {
    }

    void OnGUI () {
        GUI.skin = mySkin;
        
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(512), GUILayout.Height(512));
        GUILayout.Box(outputText);
        GUILayout.EndScrollView();

        if (GUI.Button(new Rect(4,512,128,20), "Start")) {
            ss.stdout.Send("Starting simulation engine...");
            se.start();
        }

        if (GUI.Button(new Rect(132,512,128,20), "Stop")) {
            ss.stdout.Send("Stopping simulation engine...");
            se.stop();
        }
    }

    void OnApplicationQuit() {
        se.stop();
    }


    void DoWork() {

        // Get all the text out of the queue.
        string str;
        while (ss.stdout.NotEmpty()) {
            if (ss.stdout.NBRecv(out str)) {
                outputText += str + "\n";
            }
        }

        se.generatePercepts();
        se.handleActions();
        se.instantiateAgents(agentPrefab);
    }
}