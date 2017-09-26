using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO add compressed gas thrusters (aerodynamics dont work on Mars!)
//figure out how to maintain speed (trade vertial speed to horizontal speed)

public class GliderController : MonoBehaviour {

	public float A = 0.1f;//front XSA
	public float Cd = 0.03f;
	public float AOA=0.01f;//angle of attack in radians
	public float wingArea=0.1f;
	public GameObject Cg ;
	public GameObject Cp;
	public float maxAngularVelocity=1f;
	public float rollTorque=200f;
	public float pitchTorque=200f;

	private float totalMass;
	private float solidRocketMass;
	private Vector3 gliderVelocity;
	private float dragForceMagnitude;
	private Vector3 dragForceDirection;
	private Vector3 dragForce;
	private float airDensity;
	private Transform solidRocket;
	private bool boosterState;
	private Rigidbody RB;
	private GameObject atmosphere;
	private float weight;



	// Use this for initialization
	void Start () {
		solidRocket = transform.Find ("SolidRocket");
		RB = GetComponent<Rigidbody> ();
		airDensity = FindObjectOfType<Atmosphere> ().airDensity;
		RB.maxAngularVelocity = maxAngularVelocity;
	}


	void FixedUpdate () {
		//TODO this line is inefficiant - change so booster sends a single message when it becomes active/inactive
		boosterState = FindObjectOfType<BoosterController> ().boosterActive;

		solidRocketMass = solidRocket.GetComponent<Rigidbody> ().mass;
		totalMass = solidRocketMass + RB.mass;

		//pitch angle (relative to up)
		float pitchAngle = Vector3.Angle (transform.forward,new Vector3 (0, 1, 0))*Mathf.PI/180;

		gliderVelocity = RB.velocity;
		float area = A;
		float attack = AOA;

		//movement
		if (Input.GetKey ("a")) {
			RB.AddRelativeTorque (0, 0, rollTorque);
		}

		if (Input.GetKey ("d")) {
			RB.AddRelativeTorque (0, 0, -rollTorque);
		}

		if (Input.GetKey ("w")) {
			RB.AddRelativeTorque ( pitchTorque,0,0);
			attack=-AOA;//decrease angle of attack TODO - research this factor
		}

		if (Input.GetKey ("s")) {
			RB.AddRelativeTorque (-pitchTorque,0,0);
			area = 2 * A; //increase drag TODO - research better factor to increase this
			attack=2*AOA;//increase angle of attack
		}


		//drag force
		dragForceMagnitude=(Mathf.Pow(gliderVelocity.magnitude,2f))*area*Cd*airDensity*0.5f;
		dragForceDirection = -RB.velocity.normalized;
		dragForce = dragForceMagnitude * dragForceDirection;
		RB.AddForceAtPosition (dragForce,Cp.transform.position);


		//Debug.DrawLine (Cp.transform.position, Cp.transform.position+(dragForceDirection*10f),Color.red,Mathf.Infinity);



		//TODO - apply life and weight to cg and cp
		//lift force - only apply if booster is inactive
		if (!boosterState) {
			float Cl = 2 * Mathf.PI * attack;
			float liftForce = Cl * airDensity * Mathf.Pow (RB.velocity.magnitude, 2) *wingArea / 2;
			RB.AddForce (liftForce*transform.up);
			print (RB.velocity.magnitude);
			Debug.DrawLine (Cp.transform.position, Cp.transform.position+(transform.up*10f),Color.blue,Mathf.Infinity);
			print (liftForce);
		}

		//weight force 
		weight=totalMass*3.711f;
		RB.AddForce (new Vector3(0, -weight, 0));
		Debug.DrawLine (Cg.transform.position, Cg.transform.position+new Vector3(0, -weight, 0),Color.green,Mathf.Infinity);

	}
}
