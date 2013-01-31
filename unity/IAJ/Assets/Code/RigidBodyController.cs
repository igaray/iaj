//tomado de http://wiki.unity3d.com/index.php?title=RigidbodyFPSWalker y modificado

using UnityEngine;
using System.Collections;
 

[RequireComponent (typeof (Rigidbody))]
[RequireComponent (typeof (CapsuleCollider))]
 
public class RigidBodyController : MonoBehaviour {
 
	public float speed = 5.0f;
	public float gravity = 9.81f;
	public float maxVelocityChange = 10.0f;
	public  int   _proximityRange = 1;
	private bool grounded = false;
 
	public Vector3  target;
	public  bool    moving = false;
	private Vector3 velocityVector;
	
	
	private Agent agent;
	
	void Start(){
		agent = GetComponent<Agent>();
	}
  
	void Awake () {
	    rigidbody.freezeRotation = true;
	    rigidbody.useGravity = false;
	}
 
	void FixedUpdate () {
		
	    if (moving && grounded) {
	        if (!((target - transform.position).magnitude < _proximityRange)){
				
				// Calculate how fast we should be moving
				
		        Vector3 targetVelocity = (target - transform.position).normalized;
		        targetVelocity = transform.TransformDirection(targetVelocity);
		        targetVelocity *= speed;
	 
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
	    rigidbody.AddForce(new Vector3 (0, -gravity * rigidbody.mass, 0));
 
	    grounded = false;
	}
	
	bool near(){
		return (target - transform.position).magnitude < _proximityRange;
	}
 
	void OnCollisionStay () {
	    grounded = true;    
	}
	
	public void move(Vector3 target){
		moving = true;
		this.target = target;
	}
 
//	float CalculateJumpVerticalSpeed () {
//	    // From the jump height and gravity we deduce the upwards speed 
//	    // for the character to reach at the apex.
//	    return Mathf.Sqrt(2 * jumpHeight * gravity);
//	}
}
