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
    
    // Use this for initialization
    void Start () {
        ss = new SimulationState("C:\\config.xml");
        se = new SimulationEngine(ss);

        se.start();

        InvokeRepeating( "PrintOutput",       0, 0.1f );
        InvokeRepeating( "GeneratePercepts",  0, 0.1f );
        InvokeRepeating( "HandleActions",     0, 0.1f );
        InvokeRepeating( "InstantiateAgents", 0, 0.1f );

        ss.stdout.Send("Start finished.");
    }
    
    // Update is called once per frame
    void Update () {
    }

    void OnGUI () {
        GUI.skin = mySkin;
        
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(256), GUILayout.Height(512));
        GUILayout.Box(outputText);
        GUILayout.EndScrollView();

        if (GUI.Button(new Rect(4,512,248,20), "Stop")) {
            ss.stdout.Send("Stopping...");
            se.stop();
        }
    }

    void OnApplicationQuit() {
        ss.stdout.Send("Quitting...");
        se.stop();
    }


    void PrintOutput() {
        // Get all the text out of the queue.
        string str;
        while (ss.stdout.notEmpty()) {
            if (ss.stdout.Recv(out str)) {
                outputText += str + "\n";
            }
        }
    }

    void GeneratePercepts() {
        se.generatePercepts();
    }

    void HandleActions() {
        se.handleActions();
    }

    void InstantiateAgents() {
        se.instantiateAgents();
    }
}
