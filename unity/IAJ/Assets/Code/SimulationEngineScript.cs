using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Pathfinding;
using Pathfinding.Nodes;

/******************************************************************************/
//public enum ActionType {unknown, noop, move, attack, parry};
//
//public enum ActionResult {success, failure};

//public class AgentState {
//	
//	private Vector3 position;
//	private String name;
//	private ActionResult lastActionResult;
//}

//public class ObjectState {
//	
//	private Vector3 position;
//	private String name;
//}

//public class Action {
//	
//	private ActionType type;
//	
//	public Action(ActionType type) {
//		this.type = type;
//	}
//	
//	public Action fromXML(string xml) {
//		// TODO
//		return null;
//	}
//}
//

public class Percept {
	
	public List<List<IPerceivableEntity> > elements = new List<List<IPerceivableEntity> >();
	
	// the perception is created and generated here
	public Percept(SimulationState state, int agentID) {
		state.agents[agentID].agentController.perceive(this); 
	}
	
	public string toProlog() {
		
		string aux = "";
		Hashtable dic;
		
		foreach (List<IPerceivableEntity> list in elements){
//			switch(e.elementType)
//			{
//				case 0: 											// Entity
//					dic = e.entity.perception();
//					foreach(DictionaryEntry entry in dic){
//						Debug.Log(entry.Key + ": " + entry.Value);
//					}
//					break;
//				case 1:												// List of entities
//					foreach(Entity entity in e.entities){
//						dic = entity.perception();
//						foreach(DictionaryEntry entry in dic){
//							Debug.Log(entry.Key + ": " + entry.Value);
//						}
//					}
//					break;
//				case 2:												// List of nodes
//					foreach(GridNode node in e.nodes){
//						//TODO generar alguna manera de pasaar a texto los nodos
//						node.ContainsConnection(node);
//					}
//					break;
//			}
			foreach(IPerceivableEntity e in list){
				dic = e.perception();
				foreach(DictionaryEntry entry in dic){
					Debug.Log(entry.Key + ": " + entry.Value);
				}
			}
		}
		return aux; 
	}
	
	// posiblemente innecesario
//	public void add(Entity e){
//		elements.Add(new PerceptElement(e));
//	}
	
	public void addEntities(List<IPerceivableEntity> e){
		elements.Add(e);
	}
	
//	public void addNodes(List<GridNode> e){
//		elements.Add((new PerceptElement()).addNodes(e));
//	}
//	
//	public void add(List<GridNode> e){
//		elements.Add(new PerceptElement(e));
//	}
}

//public class PerceptRequest {
//	
//	public int agentID;
//	public PerceptQueue percepts;
//	
//	public PerceptRequest(int AID, PerceptQueue percepts) {
//		this.agentID  = AID;
//		this.percepts = percepts;
//	}	
//}

public class ActionQueue {
	
	private Queue queue;
	private Mutex mutex;

    public ActionQueue() {
        queue = new Queue();
        mutex = new Mutex(false);
    }
    
    public void Enqueue(Action a) {
        mutex.WaitOne();
        queue.Enqueue(a);
        mutex.ReleaseMutex();
    }
    
    public bool Dequeue(out Action a) {
		bool result = false;
		a = null;
        mutex.WaitOne();
        if (queue.Count > 0) {
            a = (Action)queue.Dequeue();
			result = true;
        }
        mutex.ReleaseMutex();
        return result;
    }
	
	public bool isEmpty() {
		return (queue.Count == 0);
	}
	
	public bool notEmpty() {
		return (queue.Count > 0);
	}
}

public class PerceptQueue {
	
	private Queue queue;
	private Mutex mutex;

    public PerceptQueue() {
        queue = new Queue();
        mutex = new Mutex(false);
    }
    
    public void Enqueue(Percept p) {
        mutex.WaitOne();
        queue.Enqueue(p);
        mutex.ReleaseMutex();
    }
    
    public bool Dequeue(out Percept p) {
		bool result = false;
		p = null;
        mutex.WaitOne();
        if (queue.Count > 0) {
            p = (Percept)queue.Dequeue();
			result = true;
        }
        mutex.ReleaseMutex();
        return result;
    }
	
	public bool isEmpty() {
		return (queue.Count == 0);
	}
	
	public bool notEmpty() {
		return (queue.Count > 0);
	}
}

public class ResultQueue {
		
	private Queue queue;
	private Mutex mutex;

    public ResultQueue() {
        queue = new Queue();
        mutex = new Mutex(false);
    }
    
    public void Enqueue(ActionResult ar) {
        mutex.WaitOne();
        queue.Enqueue(ar);
        mutex.ReleaseMutex();
    }
    
    public bool Dequeue(out ActionResult ar) {
		bool result = false;
		ar = ActionResult.failure;
        mutex.WaitOne();
        if (queue.Count > 0) {
            ar = (ActionResult)queue.Dequeue();
			result = true;
        }
        mutex.ReleaseMutex();
        return result;
    }
	
	public bool isEmpty() {
		return (queue.Count == 0);
	}
	
	public bool notEmpty() {
		return (queue.Count > 0);
	}
}

public class StringQueue {
    
    private Queue queue;
    private Mutex mutex;
    
    public StringQueue() {
        queue = new Queue();
        mutex = new Mutex(false);
    }
    
    public void Enqueue(string str) {
        mutex.WaitOne();
        queue.Enqueue(str);
        mutex.ReleaseMutex();
    }
    
    public bool Dequeue(out string str) {
		bool result = false;
		str = null;
        mutex.WaitOne();
        if (queue.Count > 0) {
            str = (string)queue.Dequeue();
			result = true;
        }
        mutex.ReleaseMutex();
        return result;
    }
	
	public bool isEmpty() {
		return (queue.Count == 0);
	}
	
	public bool notEmpty() {
		return (queue.Count > 0);
	}
}

public class RequestQueue {
    
    private Queue queue;
    private Mutex mutex;
    
    public RequestQueue() {
        queue = new Queue();
        mutex = new Mutex(false);
    }
    
    public void Enqueue(PerceptRequest req) {
        mutex.WaitOne();
        queue.Enqueue(req);
        mutex.ReleaseMutex();
    }
    
    public bool Dequeue(out PerceptRequest req) {
		bool result = false;
		req = null;
        mutex.WaitOne();
        if (queue.Count > 0) {
            req = (PerceptRequest)queue.Dequeue();
			result = true;
        }
        mutex.ReleaseMutex();
        return result;
    }
	
	public bool isEmpty() {
		return (queue.Count == 0);
	}
	
	public bool notEmpty() {
		return (queue.Count > 0);
	}
}


/******************************************************************************/
//public class ConnectionHandler {
//
//    private bool quit;
//    private AgentConnection[] agentConnections;
//
//    public ConnectionHandler() {
//        // initialize everything
//    }
//
//    public void run() {
//        while (!quit) {
//            /*
//            listen on serverSocket
//            on accept
//                spawn agentAvatar 
//                add it to agentAvatars
//                tell it to run()
//            */
//        }
//    }
//}
//
//public class AgentConnection {
//
//	private Socket socket;
//	private NetworkStream ns;
//	private StreamReader  sr;
//	private StreamWriter  sw;
//
//	public int id;
//	public ActionQueue actionQueue;
//	public ResultQueue resultQueue;
//	public PerceptQueue perceptQueue;
//	
//	public void start(Socket socket, ActionQueue globalActionQueue) {
//		// initialize everything
//		actionQueue  = globalActionQueue;
//		resultQueue  = new ResultQueue();
//		perceptQueue = new PerceptQueue();
//	}
//
//	public void run() {
//        /*
//        prepare percept
//        send percept
//        receive action
//        enqueue action in actionQueue
//        deque action result
//        send action result
//        */
//	}
//}



/******************************************************************************/
//public class SimulationEngine {
//    
//    private ConnectionHandler connectionHandler;
//    private ActionHandler actionHandler;
//
//    public SimulationEngine() {
//        // start the action handler
//        // start the connection handler
//    }
//}
//
//public class ActionHandler  {
//    
//    private bool        quit = false;
//    private ActionQueue actionQueue;
//    private AgentState  agents;
//    private ObjectState objects;
//
//    public void start() {
//        // initialize everything
//    }
//
//    public void run() {
//        while (!quit) {
//            /*
//            get action from actionQueue
//            apply action effects to world state
//            */
//        }
//    }   
//}



/******************************************************************************/
public class ThreadTest {
    
    private string name;
	private int i;
	private StringQueue sq;

    public ThreadTest(string name, StringQueue sq) {
        this.name = name;
		this.sq = sq;
		this.i = 0;
    }

    public void run() {
        while (true) {
            i++;
			sq.Enqueue(name.ToString() + " " + i.ToString() + "\n");
			Thread.Sleep(1000);
        }
   }
}
public class NetworkTest {
    
    private string name;
	private int i;
	private StringQueue sq;

    public NetworkTest(string name, StringQueue sq) {
        this.name = name;
		this.sq = sq;
		this.i = 0;
    }

    public void run() {

//        while (true) {
//            i++;
//			sq.Enqueue(name.ToString() + " " + i.ToString() + "\n");
//			Thread.Sleep(1000);
//        }

		TcpListener serverSocket = new TcpListener(IPAddress.Parse("127.0.0.1"), 8888);
		sq.Enqueue(">> server socket create\n");
		TcpClient clientSocket = default(TcpClient);
		sq.Enqueue(">> client socket create\n");
		serverSocket.Start();
		sq.Enqueue(">> start\n");
		clientSocket = serverSocket.AcceptTcpClient();
		sq.Enqueue(">> accept\n");
		
//		NetworkStream ns = new NetworkStream(clientSocket);
		NetworkStream ns = clientSocket.GetStream();
		StreamReader  sr = new StreamReader(ns);
		StreamWriter  sw = new StreamWriter(ns);
		
		string data = sr.ReadLine();
		sq.Enqueue(">> data:" + data + "\n");

		data = sr.ReadLine();
		sq.Enqueue(">> data:" + data + "\n");

//		sw.WriteLine("data from server");
//		sw.Flush();
		
//		NetworkStream networkStream = clientSocket.GetStream();
//		byte[] bytesFrom = new byte[100];
//		networkStream.ReadbytesFrom, 0, (int)clientSocket.ReceiveBufferSize);
//		sq.Enqueue(">> read\n");
//		string dataFromClient = System.Text.Encoding.UTF8.GetString(bytesFrom);
//		sq.Enqueue(">> data: " + dataFromClient + "\n");
		
		clientSocket.Close();
		sq.Enqueue(">> close\n");
		serverSocket.Stop();
		sq.Enqueue(">> stop\n");
   }
}



/******************************************************************************/
public class SimulationEngineScript : MonoBehaviour {
    
    public GUISkin mySkin;
	public Vector2 scrollPosition;
	
	public System.Int64 frame = 0;
	
    private string outputText = "";
    private StringQueue outputQueue;
	private RequestQueue perceptRequests;
	
	public SimulationState state; 

    // Use this for initialization
    void Start () {
		
		perceptRequests = new RequestQueue();
        outputQueue = new StringQueue();
        outputQueue.Enqueue("Loading... ");

        outputQueue.Enqueue("done.\n");
		print("ok");
		
		InvokeRepeating("GeneratePercepts", 0.0f, 0.1f);
		InvokeRepeating("HandleActions",    0.0f, 0.1f);
    }
    
	// TODO LEO
//	void makePercept(int agentID, MailBox<Percept> percepts) {
//		
//	}
	
	void GeneratePercepts () {
		
		PerceptRequest request;
		
		// for every percept request object in the percept request mailbox, 
		while (perceptRequests.notEmpty()) {
			
			// get the percept request object
			perceptRequests.Dequeue(out request);

			// genereate the percept
			// TODO LEO: implementar esto
			//makePercept(request.agentID, request.agentPerceptMailbox);
	
			Percept percept = new Percept(state, request.agentID);
			
			// insert the percept in the agent percept mailbox}
			request.agentPerceptMailbox.Send(percept);
		}
	}
	
	void HandleActions () {
		string str;

		// Get all the text out of the queue.
		while (outputQueue.notEmpty()) {
			if (outputQueue.Dequeue(out str)) {
				outputText += str;
			}
		}
		frame++;
		outputQueue.Enqueue(".");
    }
	
	void Update() {
	}
	
    void OnGUI () {
        GUI.skin = mySkin;
		scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(256), GUILayout.Height(512));
		GUILayout.Box(outputText);
		GUILayout.EndScrollView();
    }

	void OnApplicationQuit() {
	}
}
