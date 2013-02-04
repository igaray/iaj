//tomado de http://wiki.unity3d.com/index.php?title=RigidbodyFPSWalker y modificado

using UnityEngine;
using System.Collections;
 
[RequireComponent (typeof (Rigidbody))]
[RequireComponent (typeof (CapsuleCollider))]
 
public class RigidBodyController : MonoBehaviour {
 
	public  float   _speed            = 5.0f;
	public  float   _gravity          = 9.81f;
	public  float   maxVelocityChange = 10.0f;
	public  int     _proximityRange   = 1;
	public  Vector3 target;
	public  bool    moving 			  = false;
	private bool    grounded          = false;
	private Vector3 velocityVector;
	private Agent   _agent;
	
	void Start(){
		_agent = GetComponent<Agent>();
	}
  
	void Awake () {
	    rigidbody.freezeRotation = true;
	    rigidbody.useGravity = false;
	}
 
	void FixedUpdate () {
		
	    if (moving && grounded) {
	    
	        if (!((target - transform.position).magnitude < _proximityRange)){		// if not near
				
				// Calculate how fast we should be moving
				
		        Vector3 targetVelocity = (target - transform.position).normalized;
		        targetVelocity = transform.TransformDirection(targetVelocity);
		        targetVelocity *= _speed;
	 
		        // Apply a force that attempts to reach our target velocity
		        Vector3 velocity = rigidbody.velocity;
		        Vector3 velocityChange = (targetVelocity - velocity);
		        velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
		        velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
		        velocityChange.y = 0;
		        rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
			}
 			else
				moving = false;
	    }
 
	    // We apply gravity manually for more tuning control
	    rigidbody.AddForce(new Vector3 (0, -_gravity * rigidbody.mass, 0));
 
	    grounded = false;
	}

	void OnCollisionStay () {
	    grounded = true;    
	}
	
	public void move(Vector3 target){
		moving = true;
		this.target = target;
	}
 
}
