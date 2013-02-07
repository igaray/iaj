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

// Runs within its own thread, cannot call Unity3D API methods.
public class ConnectionHandler {

    // NETWORK & OPERATION
    public Thread                connectionHandlerThread;
    public TcpListener           tcpListener;
    public TcpClient             tcpClient;

    // SIMULATION
    public bool                  quit;
    public List<AgentConnection> agentConnections;
    public SimulationState       simulationState;
	public MailBox<bool> 		 instantiationResults;
 
    public ConnectionHandler(SimulationState ss) {
        // NETWORK & OPERATION
        tcpListener             = new TcpListener(IPAddress.Parse("127.0.0.1"), 8888);
        connectionHandlerThread = new Thread(new ThreadStart(this.run));
        connectionHandlerThread.IsBackground = true;

        // SIMULATION
        quit                    = false;
        simulationState         = ss;
        agentConnections        = new List<AgentConnection>();
		instantiationResults    = new MailBox<bool>(false);
    }

    public void start() {
        simulationState.stdout.Send("CH: starting thread");
        connectionHandlerThread.Start();
    } 

    public void stop() {
        simulationState.stdout.Send("CH: stopping...");

        tcpListener.Stop();

        simulationState.stdout.Send("CH: tcp listener stopped...");

        foreach (AgentConnection ac in agentConnections) {
            ac.stop();
        }

        simulationState.stdout.Send("CH: agents stopped...");

        // Setting quit to true is rather useless, because the 
        // connection handler's thread will almost surely blocked in 
        // the call to AcceptTcpClient()
        // For this reason, we call Abort() on the thread object.
        quit = true;
        try {
            connectionHandlerThread.Abort();
        }
        catch (ThreadAbortException) {
            simulationState.stdout.Send("Connection handler thread abort failed.");
        }

        simulationState.stdout.Send("CH: connection thread aborted...");
    }

    public void run() {

		bool result;
        AgentConnection    agentConnection;
		InstantiateRequest request;

        simulationState.stdout.Send("CH: entering main loop");

        tcpListener.Start();

        while (!quit) {
            Thread.Sleep(0);
            tcpClient       = tcpListener.AcceptTcpClient();
            agentConnection = new AgentConnection(simulationState, tcpClient);

            simulationState.stdout.Send(String.Format("CH: accepted client."));

			if (agentConnection.init()) {
				agentConnections.Add(agentConnection);
					
				request = new InstantiateRequest(agentConnection);
				simulationState.instantiateRequests.Send(request);
				instantiationResults.Recv(out result);
	
				if (result) {
					agentConnection.start();
				}
			}
        }
        simulationState.stdout.Send("CH: exit main loop");
    }
}

