using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

/******************************************************************************/
public enum ActionType {unknown, noop, move, attack, parry};

public enum ActionResult {success, failure};

public class AgentState {
	
	private Vector3 position;
	private String name;
	private ActionResult lastActionResult;
}

public class ObjectState {
	
	private Vector3 position;
	private String name;
}

public class Action {
	
	private ActionType type;
	
	public Action(ActionType type) {
		this.type = type;
	}
	
	public Action fromXML(string xml) {
		// TODO
		return null;
	}
}

public class Percept {
	
	public Percept() {
		// TODO
	}
	
	public String toXML() {
		// TODO
		return "";
	}
}

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



/******************************************************************************/
public class ConnectionHandler {

    private bool quit;
    private AgentConnection[] agentConnections;

    public ConnectionHandler() {
        // initialize everything
    }

    public void run() {
        while (!quit) {
            /*
            listen on serverSocket
            on accept
                spawn agentAvatar 
                add it to agentAvatars
                tell it to run()
            */
        }
    }
}

public class AgentConnection {

	private Socket socket;
	private NetworkStream ns;
	private StreamReader  sr;
	private StreamWriter  sw;

	public int id;
	public ActionQueue actionQueue;
	public ResultQueue resultQueue;
	public PerceptQueue perceptQueue;
	
	public void start(Socket socket, ActionQueue globalActionQueue) {
		// initialize everything
		actionQueue  = globalActionQueue;
		resultQueue  = new ResultQueue();
		perceptQueue = new PerceptQueue();
	}

	public void run() {
        /*
        prepare percept
        send percept
        receive action
        enqueue action in actionQueue
        deque action result
        send action result
        */
	}
}



/******************************************************************************/
public class SimulationEngine {
    
    private ConnectionHandler connectionHandler;
    private ActionHandler actionHandler;

    public SimulationEngine() {
        // start the action handler
        // start the connection handler
    }
}

public class ActionHandler  {
    
    private bool        quit = false;
    private ActionQueue actionQueue;
    private AgentState  agents;
    private ObjectState objects;

    public void start() {
        // initialize everything
    }

    public void run() {
        while (!quit) {
            /*
            get action from actionQueue
            apply action effects to world state
            */
        }
    }   
}



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
//		networkStream.Read(bytesFrom, 0, (int)clientSocket.ReceiveBufferSize);
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

//    private ThreadTest oThreadTest1;
//    private ThreadTest oThreadTest2;
//
//    private NetworkTest oNetworkTest;
	
//    private Thread oThread1;
//    private Thread oThread2;
//    private Thread oThread3;

    // Use this for initialization
    void Start () {
		
        outputQueue = new StringQueue();
        outputQueue.Enqueue("Loading... ");

//		oThreadTest1 = new ThreadTest("thread1", outputQueue);
//		oThread1 = new Thread(new ThreadStart(oThreadTest1.run));
//		oThread1.Start();

//		oThreadTest2 = new ThreadTest("thread2", outputQueue);
//		oThread2 = new Thread(new ThreadStart(oThreadTest2.run));
//		oThread2.Start();

//		oNetworkTest = new NetworkTest("network", outputQueue);
//		oThread3 = new Thread(new ThreadStart(oNetworkTest.run));
//		oThread3.Start();
		
        outputQueue.Enqueue("done.\n");
		print("ok");
    }
    
    // Update is called once per frame
    void Update () {
		string str;

		// Get all the text out of the queue.
		while (outputQueue.notEmpty()) {
			if (outputQueue.Dequeue(out str)) {
				outputText += str;
			}
		}
		frame++;
    }

    void OnGUI () {
        GUI.skin = mySkin;
		scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(256), GUILayout.Height(512));
		GUILayout.Box(outputText);
		GUILayout.EndScrollView();
    }

	void OnApplicationQuit() {
//		oThread1.Abort();
//		oThread2.Abort();
//		oThread3.Abort();
	}
}
