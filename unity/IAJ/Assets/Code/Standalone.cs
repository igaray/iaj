//using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Xml;

// Esta clase es la que es el script asociado al GameObject SimulationEngine
// Tiene los correspondientes metodos Start() y Update()

public class StandaloneServer {

    // SIMULATION
    static bool             quit;
    static SimulationState  ss;
    static SimulationEngine se;

    static void Start() {

        float delta = 0.1f;

        quit = false;
        ss = new SimulationState("config.xml");
        se = new SimulationEngine(ss);

        Console.WriteLine("Press press any key to stop the simulation engine.");
        Console.WriteLine("Note: there is no ANY key on your keyboard.");

        se.start();

        /*
        InvokeRepeating( "se.generatePercepts"           , 0, delta );
        InvokeRepeating( "se.actionHandler.handleAction" , 0, delta );
        */
    }

    static void Update() {
        // OOP FTW
        se.generatePercepts(); 
        se.actionHandler.handleAction();
    }

    // Main simulates a Unity3D MonoBehaviour component. 
    static void Main() {
        Start();
        while (! quit) {
            Update();
            if (Console.KeyAvailable) {
                Console.ReadKey(false);
                Console.WriteLine("SE: quitting.");
                se.stop();
                quit = true;
            }
        }
    }
}

