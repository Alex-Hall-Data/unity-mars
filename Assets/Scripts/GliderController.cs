using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO add compressed gas thrusters (aerodynamics dont work on Mars!)
//clamp the lift to a max

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
	public float thrusterActiveThreshold=0.01f; //threshold angular acceleration for thrusters to activate
	public float thrusterSpeed=200f;

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
	private ParticleSystem leftThruster;
	private ParticleSystem rightThruster;
	private ParticleSystem frontThruster;
	private Vector3 gliderAV;



	// Use this for initialization
	void Start () {
		solidRocket = transform.Find ("SolidRocket");
		RB = GetComponent<Rigidbody> ();
		airDensity = FindObjectOfType<Atmosphere> ().airDensity;
		RB.maxAngularVelocity = maxAngularVelocity;

		rightThruster = GameObject.Find ("Right").GetComponent<ParticleSystem>();
		leftThruster = GameObject.Find ("Left").GetComponent<ParticleSystem>();
		frontThruster = GameObject.Find ("Front").GetComponent<ParticleSystem>();
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



		//weight force 
		weight=totalMass*3.711f;
		RB.AddForce (new Vector3(0, -weight, 0));
		Debug.DrawLine (Cg.transform.position, Cg.transform.position+new Vector3(0, -weight, 0),Color.green,Mathf.Infinity);


		//lift force - only apply if booster is inactive
		float liftForce;
		if (!boosterState) {
			float Cl = 2 * Mathf.PI * attack;

			if (Input.GetKey ("s")) {
				liftForce = Mathf.Clamp(Cl * airDensity * Mathf.Pow (RB.velocity.magnitude, 2) *wingArea / 2,-weight,2*weight);
			}else if(Input.GetKey ("w")){
				liftForce = Mathf.Clamp(Cl * airDensity * Mathf.Pow (RB.velocity.magnitude, 2) *wingArea / 2,-2*weight,weight);
			}else{
				liftForce = Mathf.Clamp (Cl * airDensity * Mathf.Pow (RB.velocity.magnitude, 2) * wingArea / 2, -weight, weight);
			}
			RB.AddForce (liftForce*transform.up);
			//print (RB.velocity.magnitude);
			Debug.DrawLine (Cp.transform.position, Cp.transform.position+(transform.up*10f),Color.blue,Mathf.Infinity);
			//print (liftForce);
		}

		//TODO - inverted thrusters look weird - fix
		//get angular acceleration
		ParticleSystem.VelocityOverLifetimeModule leftThrusterSpeed=leftThruster.velocityOverLifetime;
		ParticleSystem.VelocityOverLifetimeModule rightThrusterSpeed=rightThruster.velocityOverLifetime;
		ParticleSystem.VelocityOverLifetimeModule frontThrusterSpeed=frontThruster.velocityOverLifetime;

		Vector3 newGliderAV=RB.angularVelocity;
		//print (newGliderAV.z - gliderAV.z);
		if ( gliderAV.z - newGliderAV.z >= thrusterActiveThreshold) { //on roll fire thrusters (each wing in opposite directions)

			ParticleSystem.MinMaxCurve Rrate = new ParticleSystem.MinMaxCurve();
			Rrate.constantMax = thrusterSpeed; 
			rightThrusterSpeed.y=Rrate;
			rightThruster.Emit (20);

			//invert left thruster direction and fire
			ParticleSystem.MinMaxCurve Lrate = new ParticleSystem.MinMaxCurve();
			Lrate.constantMax = -thrusterSpeed;
			Lrate.constantMin=-thrusterSpeed;
			leftThrusterSpeed.y=Lrate;
			leftThruster.Emit(20);
		}

		if (  newGliderAV.z - gliderAV.z>= thrusterActiveThreshold) {
			ParticleSystem.MinMaxCurve Lrate = new ParticleSystem.MinMaxCurve();
			Lrate.constantMax = thrusterSpeed; 
			leftThrusterSpeed.y=Lrate;
			leftThruster.Emit (20);

			//invert right thruster direction and fire
			ParticleSystem.MinMaxCurve Rrate = new ParticleSystem.MinMaxCurve();
			Rrate.constantMax = -thrusterSpeed; 
			Rrate.constantMin =-thrusterSpeed;
			rightThrusterSpeed.y=Rrate;
			rightThruster.Emit(20);
		}


		//pitch thruster
		if (newGliderAV.x - gliderAV.x >= thrusterActiveThreshold) {
			ParticleSystem.MinMaxCurve Frate = new ParticleSystem.MinMaxCurve ();
			Frate.constantMax = thrusterSpeed; 
			frontThrusterSpeed.y = Frate;
			frontThruster.Emit (20);
		}

			if (gliderAV.x - newGliderAV.x  >= thrusterActiveThreshold) {
				ParticleSystem.MinMaxCurve Frate = new ParticleSystem.MinMaxCurve();
				Frate.constantMax = -thrusterSpeed; 
				frontThrusterSpeed.y=Frate;
				frontThruster.Emit (20);
			
	}
		gliderAV = RB.angularVelocity;

			
	}
}
