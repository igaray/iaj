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
public class StringQueue {
    
    private Queue queue;
    
    public StringQueue() {
        queue = new Queue();
    }
    
    public void Enqueue(string str) {
        lock (this) {
            queue.Enqueue(str);
        }
    }
    
    public bool Dequeue(out string str) {
        lock (this) {
            bool result = false;
            str = null;
            if (queue.Count > 0) {
                str = (string)queue.Dequeue();
                result = true;
            }
            return result;
        }
    }
    
    public bool isEmpty() {
        return (queue.Count == 0);
    }
    
    public bool notEmpty() {
        return (queue.Count > 0);
    }
}

/******************************************************************************/
public class SimulationEngineComponentScript : MonoBehaviour {

    // SIMULATION
    SimulationState  ss;
    SimulationEngine se;

    // UNITY GUI
    public GUISkin mySkin;
    public Vector2 scrollPosition;
    private string outputText = "";
    private StringQueue stdout = new StringQueue();
    
    // Use this for initialization
    void Start () {
        ss = new SimulationState("config.xml");
        stdout.Enqueue("Finished Start.");
    }
    
    // Update is called once per frame
    void Update () {
        // Get all the text out of the queue.
        string str;
        while (stdout.notEmpty()) {
            if (stdout.Dequeue(out str)) {
                outputText += str;
            }
        }
        
    }

    void OnGUI () {
        GUI.skin = mySkin;
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(256), GUILayout.Height(512));
        GUILayout.Box(outputText);
        GUILayout.EndScrollView();
    }

    void OnApplicationQuit() {
        se.stop();
    }
}