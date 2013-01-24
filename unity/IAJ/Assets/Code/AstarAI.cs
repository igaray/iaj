using UnityEngine;
using System.Collections;
//Note this line, if it is left out, the script won't know that the class 'Path' exists and it will throw compiler errors
//This line should always be present at the top of scripts which use pathfinding
using Pathfinding;


public class AstarAI : MonoBehaviour {
    //The point to move to
    public Vector3 targetPosition;
	
	private int index = 0;
	private Vector3[] vectores = new Vector3[]{new Vector3(1,0,1),new Vector3(1,0,20),new Vector3(20,0,20),new Vector3(20,0,1)};
	//private Vector3[] vectores = new Vector3[]{new Vector3(1,0,30)};
    
    private Seeker seeker;
    private CharacterController controller;
 
    //The calculated path
    public Path path;
    
    //The AI's speed per second
    public float speed = 100;
    
    //The max distance from the AI to a waypoint for it to continue to the next waypoint
    public float nextWaypointDistance = 3;
 
    //The waypoint we are currently moving towards
    private int currentWaypoint = 0;
 
    public void Start () {
        seeker = GetComponent<Seeker>();
        controller = GetComponent<CharacterController>();
		//targetPosition = vectores[index];
		
        //Start a new path to the targetPosition, return the result to the OnPathComplete function
        seeker.StartPath (transform.position, targetPosition, OnPathComplete);
    }
    
    public void OnPathComplete (Path p) {
        Debug.Log ("Yey, we got a path back. Did it have an error? " + p.error);
		//Perception percept = this.GetComponent<Perception>();
		try{
			//percept.drop(percept.tesoros[0]);
		} 
		catch{
		}
        if (!p.error) {
            path = p;
            //Reset the waypoint counter
            currentWaypoint = 0;
        }
    }
	
	public void NextPosition(Vector3 newPosition){
		targetPosition = newPosition;
		seeker.StartPath (transform.position, targetPosition, OnPathComplete);
	}
 
    public void FixedUpdate () {
        if (path == null) {

            return;
        }
        
        if (currentWaypoint >= path.vectorPath.Length) {

            targetPosition = vectores[index];
			Debug.Log (targetPosition);
			index = (index + 1) % vectores.GetLength(0);
			
			seeker.StartPath (transform.position, targetPosition, OnPathComplete);
            return;
        }
        
        //Direction to the next waypoint
        Vector3 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
        dir *= speed * Time.fixedDeltaTime;
        controller.SimpleMove (dir);
        
        //Check if we are close enough to the next waypoint
        //If we are, proceed to follow the next waypoint
        if (Vector3.Distance (transform.position, path.vectorPath[currentWaypoint]) < nextWaypointDistance) {
            currentWaypoint++;
            return;
        }
    }
} 