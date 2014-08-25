//tomado de http://wiki.unity3d.com/index.php?title=RigidbodyFPSWalker y modificado

using UnityEngine;
using System.Collections;
using System;
 
[RequireComponent (typeof (Rigidbody))]
[RequireComponent (typeof (CapsuleCollider))]
 
public class RigidBodyController : MonoBehaviour {
 
	public  float    _speed            = 20.0f;
	public  float    _gravity          = 0.01f;
	public  float    maxVelocityChange = 10.0f;
	public  int      _proximityRange   = 1;
	public  Vector3  target;
	public  bool     moving 			  = false;
	public  bool     grounded          = false;
	private Vector3  velocityVector;
	private Agent    _agent;
	private DateTime lastMovementTS;
	private bool     movementStoppedInvoked = false;
	private Vector3  origin;
	
	void Start(){
		_agent = GetComponent<Agent>();
		lastMovementTS = DateTime.Now;
	}
  
	void Awake () {
	    rigidbody.freezeRotation = true;
	    rigidbody.useGravity     = false;
	}
 
	void FixedUpdate () {
		
	    if (moving && grounded) {
	    
	        if (!((target - transform.position).magnitude < _proximityRange)){		// if not near
				
				// Calculate how fast we should be moving
				
		        Vector3 targetVelocity  = (target - transform.position).normalized;
		        targetVelocity          = transform.TransformDirection(targetVelocity);
		        targetVelocity         *= _speed;
	 
		        // Apply a force that attempts to reach our target velocity
		        Vector3 velocity       = rigidbody.velocity;
		        Vector3 velocityChange = (targetVelocity - velocity);
		        velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
		        velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
		        velocityChange.y = 0;
		        rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
			}
 			else{
				moving = false;
				_agent.stoppedAction();
			}
	    }
 
	    // We apply gravity manually for more tuning control
	    rigidbody.AddForce(new Vector3 (0, -_gravity * rigidbody.mass, 0));
 
	    grounded = false;
	}

	
	void OnCollisionStay(){
		grounded = true;  
		if (moving &&
			!movementStoppedInvoked &&
			this.rigidbody.velocity.magnitude < 0.1f &&
			(DateTime.Now - this.lastMovementTS).Milliseconds > 500)
		
		{
			
			movementStoppedInvoked = true;
			Invoke("movementStopped", 0.2f);
		}

	}
	
	void movementStopped(){
		movementStoppedInvoked = false;
		SimulationEngineComponentScript.ss.stdout.Send("movementStopped invoked");
		if (this.rigidbody.velocity.magnitude < 0.1f){
			SimulationEngineComponentScript.ss.stdout.Send("movementStopped took effect");
			moving = false;
			if ((target - transform.position).magnitude < _proximityRange){
				this._agent.stoppedAction();
			}
			else{
				this._agent.actionFailed();
			}
		}
	}
	
	public float move(Vector3 target){
		lastMovementTS = DateTime.Now;
		moving         = true;
		this.target    = target;
		Debug.Log("origen " + transform.position);
		Debug.Log("destino " + target);		
		this._speed = connectionSpeed(transform.position - new Vector3(0,1f,0), target);
		Debug.Log("speed " + this._speed);
		float cost = connectionCost(transform.position - new Vector3(0,1f,0), target);
		Debug.Log("cost " + cost);
		
		return cost;
	}
	
	public static float connectionSpeed(Vector3 orig, Vector3 dest) {
		Vector3 distVector = (dest - orig);
		float height = distVector.y;
		if (height < 0)
			height = 0;
		else
			Debug.Log(distVector.y);
		
		distVector.y = 0;
		float dist = distVector.magnitude;		
		float angle = Mathf.Atan(height / dist);
		float speed = 10 / (1 + angle * 20); 
		if(speed != 10)
			Debug.Log(speed);
		
		return speed;
	}
	
	public static float connectionCost(Vector3 orig, Vector3 dest) {
		float speed = connectionSpeed(orig, dest);
		
		Vector3 distVector = (dest - orig);
		distVector.y = 0;
		float dist = distVector.magnitude;
		
		Debug.Log("dist " + dist);
		
		return (dist / speed)*10; //En decimas de segundos
	}
	
/*	public void move(Vector3 target){
		lastMovementTS = DateTime.Now;
		moving         = true;
		this.target    = target;
	}
 */
}
