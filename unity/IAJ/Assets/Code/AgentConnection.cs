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
public class AgentConnection {

    // NETWORK & OPERATION
    private Thread          agentConnectionThread;
    private TcpClient       tcpClient;
    private NetworkStream   networkStream;
    private StreamReader    streamReader;
    private StreamWriter    streamWriter;

    // SIMULATION 
    private bool                 quit;
    private SimulationState      simulationState;

    // PUBLIC VARIABLES
	public string                name;
	public AgentState            agentState;
	
	public AgentConnection(SimulationState ss, TcpClient tcpc) {

        // NETWORK & OPERATION
        tcpClient              = tcpc;
        networkStream          = tcpc.GetStream();
        streamReader           = new StreamReader(networkStream);
        streamWriter           = new StreamWriter(networkStream);
        agentConnectionThread  = new Thread(new ThreadStart(this.run));
		
        agentConnectionThread.IsBackground = true;

        // SIMULATION 
        quit                   = false;
        simulationState        = ss;

	}

    public void start() {
        simulationState.stdout.Send(String.Format("AC {0}: starting thread", agentState.agentID));
        agentConnectionThread.Start();
    }

    public void stop() {
        quit = true;
        simulationState.stdout.Send(String.Format("AC {0}: quitting", agentState.agentID));
        try {
            agentConnectionThread.Abort();
        }
        catch (ThreadAbortException) {
            simulationState.stdout.Send("Agent connection thread abort failed.");
        }
    }
	
	public bool init() {
		bool result = false;
		// Authenticate
        if (authenticate()) {
			result = true;
        }
		else {
            simulationState.stdout.Send("AC: Authentication error.");
            quit   = true;
			result = false;
		}
		return result;
	}
	
    public void run() {
		
		int  				  agentID  = agentState.agentID;
	    MailBox<Action>       actions  = agentState.actions;
    	MailBox<ActionResult> results  = agentState.results;
    	MailBox<Percept>      percepts = agentState.percepts;
		
        MailBox<PerceptRequest> perceptRequests = simulationState.perceptRequests;
        Percept                 percept;
        Action                  action;
        ActionResult            result;
	
        while (!quit) {
            Thread.Sleep(0);
            try {
                perceptRequests.Send(new PerceptRequest(agentID, percepts));    // send a percept request to unity

                simulationState.stdout.Send(String.Format("AC {0}: sending percept request.", agentID));

                percepts.Recv(out percept);                                     // block until I receive percept from unity

                simulationState.stdout.Send(String.Format("AC {0}: percept ready, sending...", agentID));

                sendPercept(percept);                                           // send percept to agent

                simulationState.stdout.Send(String.Format("AC {0}: waiting for action...", agentID));

                receiveAction(out action);                                      // receive action from agent

                simulationState.stdout.Send(String.Format("AC {0}: action received.", agentID));

                if (action.type == ActionType.goodbye) {                        // if the action is say goodbye, close the connection
                    sendResult(ActionResult.success);
                    quit = true;
                } else {
                    simulationState.stdout.Send(String.Format("AC {0}: sending action to handler...", agentID));
                    actions.Send(action);                                       // send action to handler
                    if (results.Recv(out result)) {                             // get action result from handler
                        simulationState.stdout.Send(String.Format("AC {0}: action result received.", agentID));
                        Thread.Sleep(action.duration);                          // sleep for the duration of the action. 
                        simulationState.stdout.Send(String.Format("AC {0}: sending action result to agent.", agentID));
                        sendResult(result);                                     // send action result to agent
                    }
                }
                sendResult(ActionResult.success);

                simulationState.stdout.Send(String.Format("AC {0}: perceive-act loop iteration complete.", agentID));
            }
            catch (System.ObjectDisposedException) {
                quit = true;
            }
            catch (System.IO.IOException) {
                quit = true;
            }
        }
        simulationState.stdout.Send(String.Format("AC {0}: quitting...", agentID));
        try {
            tcpClient.Close();

            simulationState.stdout.Send(String.Format("AC {0}: Connection closed.", agentID));
        }
        catch (System.ObjectDisposedException) {
            simulationState.stdout.Send(String.Format("AC {0}: Error while closing connection.", agentID));
        }
    }

    private bool authenticate() {
        bool   result = false;
        string xml    = streamReader.ReadLine();

        simulationState.stdout.Send(String.Format("AC: received authentication: {0}", xml));
        
        try {
            // It might be the case that the string passed in null or 
            // empty, in which case a default action of type noop is made.
            if ((xml == null) || (xml == "")) {

                simulationState.stdout.Send("AC: Error: xml empry or null.");
            }
            else {
                XmlDocument document;
                document = new XmlDocument();
                document.LoadXml(xml);
                name = document.SelectSingleNode("/authentication/name").InnerText;

                // signal authentication success to agent.
                streamWriter.Write("success.\r");
                streamWriter.Flush();
        
                result = true;
                simulationState.stdout.Send(String.Format("AC {0}: successfully authenticated.", name));
            }
        }
        catch (System.Xml.XmlException) {
            simulationState.stdout.Send("AC: Error: bad authentication xml.");
            streamWriter.Write("failure.\r");
            streamWriter.Flush();
        }
        return result;
    }

    private void sendPercept(Percept percept) {
        // convert percept to xml string
        // send it over the wire
		Debug.LogError(percept.toProlog());
        streamWriter.Write(percept.toProlog());
        streamWriter.Flush();
    }

    private void receiveAction(out Action action) {

        action = default(Action);

        // Reading from socket

        // read action xml string from the wire
        string message = streamReader.ReadLine();

        simulationState.stdout.Send(String.Format("AC {0}: received action: {1}", agentState.agentID, message));

        // convert xml into action object
        try {
            action = new Action(simulationState, agentState.agentID, message);
        }
        catch (Exception e) {
            Debug.LogError("Error in received action." + e.ToString());
        } 
    }

    private void sendResult(ActionResult result) {
        // Writing to socket

        string message = "unknown.\r";
        switch (result) {
            case ActionResult.success: {
                message = "success.\r";
                break;
            }
            case ActionResult.failure: {
                message = "failure.\r";
                break;
            }
        }
        // send it over the wire
        streamWriter.Write(message);
        streamWriter.Flush();
    }
}
