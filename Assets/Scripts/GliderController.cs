using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO add compressed gas thrusters (aerodynamics dont work on Mars!)

public class GliderController : MonoBehaviour {

	public float A = 0.1f;
	public float Cd = 0.03f;
	public GameObject Cg ;
	public GameObject Cp;
	public float rollStabiliseTorqueStrength=5f;
	public float maxAngularVelocity=1f;

	private float totalMass;
	private float solidRocketMass;
	private Vector3 gliderVelocity;
	private float dragForceMagnitude;
	private Vector3 dragForceDirection;
	private Vector3 dragForce;
	private float airDensity;
	private Vector3 weight;
	private Transform solidRocket;
	private Rigidbody RB;
	private GameObject atmosphere;


	// Use this for initialization
	void Start () {
		solidRocket = transform.Find ("SolidRocket");
		RB = GetComponent<Rigidbody> ();
		airDensity = FindObjectOfType<Atmosphere> ().airDensity;
		RB.maxAngularVelocity = maxAngularVelocity;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		solidRocketMass = solidRocket.GetComponent<Rigidbody> ().mass;
		totalMass = solidRocketMass + RB.mass;

		gliderVelocity = RB.velocity;

		//drag force
		dragForceMagnitude=(Mathf.Pow(gliderVelocity.magnitude,2f))*A*Cd*airDensity*0.5f;
		dragForceDirection = -RB.velocity.normalized;
		dragForce = dragForceMagnitude * dragForceDirection;
		RB.AddForceAtPosition (dragForce,Cp.transform.position);

		//weight force - apply to Cg
		weight=new Vector3(0,-totalMass*3.711f,0);
		RB.AddForceAtPosition (weight, Cg.transform.position);

		Debug.DrawLine (Cp.transform.position, Cp.transform.position+(dragForceDirection*10f),Color.red,Mathf.Infinity);
		Debug.DrawLine (Cg.transform.position,Cg.transform.position+(weight),Color.blue,Mathf.Infinity);

		//lift force

		//stabilising torque
		float roll=transform.localRotation.eulerAngles.y;
		float restoringTorqueMagnitude = 0;

		//position dependant rotational torque
		if (roll >=0  && roll <90) {
			restoringTorqueMagnitude=-restoringTorqueMagnitude*(Mathf.Sin(roll*Mathf.PI/180)+0.001f);
		}else if(roll>=90 && roll<180){
			restoringTorqueMagnitude=restoringTorqueMagnitude*(Mathf.Sin(roll*Mathf.PI/180)+0.001f);
		}else if(roll>=180 && roll<270){
			restoringTorqueMagnitude=-restoringTorqueMagnitude*(Mathf.Sin(roll*Mathf.PI/180)+0.001f);
		}else if(roll>=270 && roll<360){
			restoringTorqueMagnitude=restoringTorqueMagnitude*(Mathf.Sin(roll*Mathf.PI/180)+0.001f);
		} else {
			restoringTorqueMagnitude = 0;
		}

		//now do angular velocity dependant restoring torque - or just use inverse of velocity in above formulae as a product 
		
			RB.AddRelativeTorque (0, 0, -restoringTorqueMagnitude);
		

	}
}
