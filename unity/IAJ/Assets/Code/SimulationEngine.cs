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
// SYSTEM AUXILIARY CLASSES

/******************************************************************************/
public class MailBox<T> {

    private Queue<T>  queue;
    private Semaphore semaphore;

    public MailBox() {
        queue     = new System.Collections.Generic.Queue<T>();
        semaphore = new Semaphore(0, Int32.MaxValue);
    }

    public void Send(T item) {
        lock (this) {
            queue.Enqueue(item);
            semaphore.Release();
        }            
    }

    /* 
     * The method guarantees that an item will be dequeued, blocking 
     * if the queue is empty until an item is inserted and it can be
     * dequeued.
     * 
     * This Recv method will first block on the semaphore, 
     * which indicates whether there are any items in the queue or not.
     * If there are elements in the queue, the semaphore's value 
     * should be greater than zero, and the caller will attempt to 
     * acquire the mutex.
     * Once the mutex is acquired, it will check to see if the queue 
     * is in fact non-empty, and dequeu an item. 
     * This check is rather redundant if all things go well, 
     * because the caller should never get to the point in which it 
     * will attempt a Recv if the semaphore's value is non-zero.
     * In practice, the call to Count should always return a value 
     * greater than zero, the method always return true, and item
     * always be assigned an element dequeued from the queue. 
     * However, I am loath to remove the check until the code is 
     * more tested in case there is some situation I have not 
     * considered.
     * 
     * The wait must be performed first on the semaphore because if it
     * is done first on the mutex, the caller may acquire the mutex 
     * when the queue is empty. No other caller may acquire the mutex
     * to insert an item in the queue, and a deadlock will occur.  
     */
    public bool Recv(out T item) {
        bool result;
        semaphore.WaitOne();
        lock (this) {
            if (queue.Count > 0) {
                item = (T)queue.Dequeue();
                result = true;
            } else {
                Debug.Log("PANIC! queue is empty.");
                item = default(T);
                result = false;
            }
        }
        return result;
    }

    public bool NBRecv(out T item) {
        bool result;
        lock (this) {
            if (queue.Count > 0) {
                item = (T)queue.Dequeue();
                result = true;
            } else {
                item = default(T);
                result = false;
            }
        }
        return result;
    }
}

/******************************************************************************/
// SIMULATION STATIC ENTITIES

public class Position { 
    public int x;
    public int y;
    public int z;

    public Position() {

    }

    public Position(string s) {

    }
}

//public class Percept {
//    
//    public Percept(SimulationState ss, int agentID) {
//        // TODO
//    }
//    
//    public String toXML() {
//        // TODO
//        return "";
//    }
//
//    public string toProlog() {
//        //return "[position(1,2,3), agents([]), objects([])].\r";
//        return "percept(position(1,2,3), agents([]), objects([]), inventory([])).\r";
//    }
//}

public class PerceptRequest {

    public int agentID;
    public MailBox<Percept> agentPerceptMailbox;

    public PerceptRequest(int AID, MailBox<Percept> AP) {
        // Aca va informacion que usa Unity para generar la percepcion
        // principalmente, para que agente se esta generando
        // con esto, unity identifica el gameobject correspondiente 
        // al agente, y una vez obtenido eso, corre los spherecast, etc
        // ademas, tiene una referencia al mailbox de percepciones del agente
        // en la cual unity va a insertar la percepcion generada
        this.agentID             = AID;
        this.agentPerceptMailbox = AP;
    }
}

public enum ActionType {
    unknown, goodbye, noop, move, attack, pickup, drop
};

public enum ActionResult {
    success, failure
};

public class Action {
    
    public ActionType type;
    public int        duration;
    public string     actionID = "0"; // ID of the action, provided by agent.
    public int        agentID;        // ID of the agent performing the action.
    public int        targetID;       // ID of an agent that is the recipient of the action.
    public int        objectID;
    public Position   position;

    public Action(int aid) {
        this.agentID = aid;
        this.type    = ActionType.noop;
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
                this.duration = Convert.ToInt32(ss.config["action_duration_"+type_str]);
                
                if (type_str == "goodbye") {
                    this.type = ActionType.goodbye;
                }
                if (type_str == "noop") {
                    this.type = ActionType.noop;
                }
                if (type_str == "move") {
                    this.type     = ActionType.move;
                    this.position = new Position(document.SelectSingleNode("/action/position").Value);
                }
                if (type_str == "attack") {
                    this.type     = ActionType.attack;
                    this.targetID = Convert.ToInt32(document.SelectSingleNode("/action/agent/id").Value);
                }
                if (type_str == "pickup") {
                    this.type     = ActionType.pickup;
                    this.objectID = Convert.ToInt32(document.SelectSingleNode("/action/object/id").Value);
                }
                if (type_str == "drop") {
                    this.type  = ActionType.drop;
                    this.objectID = Convert.ToInt32(document.SelectSingleNode("/action/object/id").Value);
                }
            }
            catch (System.Xml.XmlException) {
                // TODO somehow signal failure to agent
                Debug.Log(String.Format("AC {0}: Error: bad action xml.", agentID));
            }
        }
    }
}

public class ObjectState {
    
    public Position position;
    public string   name;
    public string   type;

    public ObjectState(Position position, string name, string type) {
        this.position = position;
        this.name     = name;
        this.type     = type;
    }
}

public class AgentState {
    
    public int                   id;
    public bool                  connected;
    public string                name;

    public MailBox<Action>       actions;
    public MailBox<ActionResult> results;
    public MailBox<Percept>      percepts;

    public Position              position;
    public ActionResult          lastActionResult;
	
	public Agent				 agentController;  //TODO: asignarlo cuando se crea a este muchacho

    public AgentState(SimulationState ss, int id) {
        this.id               = id;
        this.connected        = true;
        this.lastActionResult = ActionResult.success;
        this.actions          = ss.readyActionQueue;
        this.results          = new MailBox<ActionResult>();
        this.percepts         = new MailBox<Percept>();
    }

    public void move(Position vector) {
        position.x += vector.x;
        position.y += vector.y;
        position.z += vector.z;
    }

    public String toString() {
        return String.Format("Agent {0} \n    id:  {1} \n    pos: {2}\n    LAR: {3}\n", 
            name, id, position, lastActionResult);
    }
}

/******************************************************************************/
// SIMULATION ENGINE CORE

// Runs within Unity3D
public class SimulationState {

    public Dictionary<string, string>   config;
    public Dictionary<string, int>      agentIDs;
    public Dictionary<int, AgentState>  agents;
//	public List<AgentState>             agents; // no queda mejor?
    public Dictionary<int, ObjectState> objects;
    public MailBox<Action>              readyActionQueue;
    public MailBox<PerceptRequest>      perceptRequests;

    public SimulationState(string ConfigurationFilePath) {
        config           = new Dictionary<string, string>();
        agents           = new Dictionary<int, AgentState>();
        objects          = new Dictionary<int, ObjectState>();
        readyActionQueue = new MailBox<Action>();
        perceptRequests  = new MailBox<PerceptRequest>();

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

        Debug.Log("Loaded the following configuration:");
        foreach (KeyValuePair<string, string> pair in config) {
            Debug.Log(String.Format("{0}:\t{1}", pair.Key, pair.Value));
        }
    }

    public bool executableAction(Action action) {
        bool result = false;
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
                // TODO
                // check if the position the agent wants to move to is adjacent
                result = false;
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
        switch (action.type) {
            case ActionType.noop: {
                break;
            }
            case ActionType.move: {
                // TODO
                // change the agent's position
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
        
        Debug.Log(String.Format("Agent {0} performs action {1} of type {2}", 
            action.agentID, 
            action.actionID, 
            action.type));
    }
}

// Runs within Unity3D
public class SimulationEngine {
    
	public SimulationState   simulationState;
    public ConnectionHandler connectionHandler;
//    public ActionHandler     actionHandler;

    public SimulationEngine(SimulationState ss) {
		simulationState   = ss;
//        actionHandler     = new ActionHandler(ss);
        connectionHandler = new ConnectionHandler(ss);
    }

    /* Don't get confused by the fact that SimulationEngine has start()
     * and stop() methods, it does not run its own thread. 
     * It just encapsulates all the other objects and threads that 
     * make up the simulation engine. 
     * Correspondingly, it has no run() method.
     */
    public void start() {
        // Tell all the threads to start.
        connectionHandler.start();
    }

    public void stop() {
        // Tell all the threads to stop.
        connectionHandler.stop();
    }

    public void generatePercepts() {
        // aca hay que sacar todos los requests de la cola y para cada uno, generar la percepcion correspondiente
    }

    public void handleActions() {

        Action                      currentAction;
        MailBox<Action>             raq    = simulationState.readyActionQueue;
        Dictionary<int, AgentState> agents = simulationState.agents;

        // get action from the ready action queue
        // if the action is executable,
        //     put it in unity's action queue
        //     let the agent know that its action was executable
        // else
        //     let the agent know that its action was not executable
        if (raq.NBRecv(out currentAction)) {
            int agentID = currentAction.agentID;
            try {
                if (simulationState.executableAction(currentAction)) {
                    agents[agentID].results.Send(ActionResult.success);
                    agents[agentID].lastActionResult = ActionResult.success;
                    simulationState.applyActionEffects(currentAction);
                }
                else {
                    agents[agentID].results.Send(ActionResult.failure);
                    agents[agentID].lastActionResult = ActionResult.failure;
                }
            }
            catch (System.Collections.Generic.KeyNotFoundException) {
                Debug.Log(String.Format("AH: Error: agent id {0} not present in agent database.", agentID));
            }
        }
    }

	public void instantiateAgents() {
		
	}
}

// Runs within its own thread, cannot call Unity3D API methods.
public class ConnectionHandler {

    // NETWORK & OPERATION
    private Thread                connectionHandlerThread;
    private TcpListener           tcpListener;
    private TcpClient             tcpClient;

    // SIMULATION
    private bool                  quit;
    private int                   agentID;
    private List<AgentConnection> agentConnections;
    private SimulationState       simulationState;
 
    public ConnectionHandler(SimulationState ss) {
        // NETWORK & OPERATION
        tcpListener             = new TcpListener(IPAddress.Parse("127.0.0.1"), 8888);
        connectionHandlerThread = new Thread(new ThreadStart(this.run));

        // SIMULATION
        quit                    = false;
        agentID                 = 1;
        simulationState         = ss;
        agentConnections        = new List<AgentConnection>();
    }

    public void start() {
        Debug.Log("CH: starting thread");
        connectionHandlerThread.Start();
    } 

    public void stop() {
        Debug.Log("CH: quitting");
        foreach (AgentConnection ac in agentConnections) {
            //ac.agentState.connected = false;
            ac.stop();
        }

        // Setting quit to true is rather useless, because the 
        // connection handler's thread will almost surely blocked in 
        // the call to AcceptTcpClient()
        // For this reason, we call Abort() on the thread object.
        quit = true;
        connectionHandlerThread.Abort();
    }

    public void run() {

        AgentConnection agentConnection;

        Debug.Log("CH: entering main loop");
        tcpListener.Start();

        while (!quit) {
            tcpClient       = tcpListener.AcceptTcpClient();
            agentConnection = new AgentConnection(simulationState, agentID, tcpClient);
            Debug.Log(String.Format("CH: accepted client {0}", agentID));

            agentConnections.Add(agentConnection);
            simulationState.agents.Add(agentID, agentConnection.agentState); // Not quite thread safe.

            agentConnection.start();
            agentID++;
        }
        Debug.Log("connection handler: exit main loop");
    }
}

// Runs within its own thread, cannot call Unity3D API methods.
public class AgentConnection {

    // NETWORK & OPERATION
    private Thread          agentConnectionThread;
    private TcpClient       tcpClient;
    private NetworkStream   networkStream;
    private StreamReader    streamReader;
    private StreamWriter    streamWriter;

    // SIMULATION 
    private bool            quit;
    private SimulationState simulationState;

    // PUBLIC VARIABLES
    public int              agentID;
    public AgentState       agentState;
   
    public AgentConnection(SimulationState ss, int id, TcpClient tcpc) {

        // NETWORK & OPERATION
        tcpClient              = tcpc;
        networkStream          = tcpc.GetStream();
        streamReader           = new StreamReader(networkStream);
        streamWriter           = new StreamWriter(networkStream);
        agentConnectionThread  = new Thread(new ThreadStart(this.run));

        // SIMULATION 
        quit                   = false;
        agentID                = id;
        simulationState        = ss;
        agentState             = new AgentState(ss, id);
    }

    public void start() {
        Debug.Log(String.Format("AC {0}: starting |thread", agentID));
        agentConnectionThread.Start();
    }

    public void stop() {
        quit = true;
        Debug.Log(String.Format("AC {0}: quitting", agentID));
        agentConnectionThread.Abort();
    }

    public void run() {

        MailBox<Action>         actions         = agentState.actions;
        MailBox<ActionResult>   results         = agentState.results;
        MailBox<Percept>        percepts        = agentState.percepts;
        MailBox<PerceptRequest> perceptRequests = simulationState.perceptRequests;
        Percept                 percept;
        Action                  action;
        ActionResult            result;

        // Authenticate
        if (! authenticate()) {
            Debug.Log(String.Format("AC {0}: Authentication error.", agentID));
            quit = true;
        }
        
        while (!quit) {
            try {
                perceptRequests.Send(new PerceptRequest(agentID, percepts));    // send a percept request to unity
                percepts.Recv(out percept);                                     // block until I receive percept from unity //toqueteado
                sendPercept(percept);                                           // send percept to agent

                receiveAction(out action);                                      // receive action from agent
                if (action.type == ActionType.goodbye) {                        // if the action is say goodbye, close the connection
                    sendResult(ActionResult.success);
                    quit = true;
                } else {
                    actions.Send(action);                                       // send action to handler
                    if (results.Recv(out result)) {                             // get action result from handler
                        Thread.Sleep(action.duration);                          // sleep for the duration of the action. 
                        sendResult(result);                                     // send action result to agent
                    }
                }
            }
            catch (System.ObjectDisposedException) {
                quit = true;
            }
            catch (System.IO.IOException) {
                quit = true;
            }
        }
        try {
            tcpClient.Close();
            Debug.Log(String.Format("AC {0}: Connection closed.", agentID));
        }
        catch (System.ObjectDisposedException) {
            Debug.Log(String.Format("AC {0}: Error while closing connection.", agentID));
        }
    }

    private bool authenticate() {
        bool   result = false;
        string xml    = streamReader.ReadLine();
        Debug.Log(String.Format("AC {0}: received authentication: {1}", agentID, xml));
        try {
            // It might be the case that the string passed in null or 
            // empty, in which case a default action of type noop is made.
            if ((xml == null) || (xml == "")) {
                return false;
            }
            else {
                XmlDocument document;
                string      name;

                document = new XmlDocument();
                document.LoadXml(xml);

                name = document.SelectSingleNode("/authentication/name").InnerText;
                this.agentState.name = name;

                // signal authentication success to agent.
                streamWriter.Write("success.\r");
                streamWriter.Flush();
                return true;
            }
        }
        catch (System.Xml.XmlException) {
            // TODO somehow signal failure to agent
            Debug.Log(String.Format("AC {0}: Error: bad authentication xml.", agentID));
        }
        return result;
    }

    private void sendPercept(Percept percept) {
        // TODO
        // convert percept to xml string
        // send it over the wire
        // Writing to socket
        streamWriter.Write(percept.toProlog());
        streamWriter.Flush();
    }

    private void receiveAction(out Action action) {
        // Reading from socket

        // read action xml string from the wire
        string message = streamReader.ReadLine();
        Debug.Log(String.Format("AC {0}: received action: {1}", agentID, message));
        // convert xml into action object
        action = new Action(simulationState, agentID, message);
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
