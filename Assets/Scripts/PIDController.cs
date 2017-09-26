namespace VacuumBreather{

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PIDController : MonoBehaviour {


	private readonly PidQuaternionController pidController = new PidQuaternionController(8.0f, 0.0f, 0.05f);


	private Transform currentTransform;
	private Rigidbody RB;

	public float Kp;
	public float Ki;
	public float Kd;


	private Quaternion DesiredOrientation;



	private void Awake()
	{
			this.currentTransform = transform;
			this.RB= GetComponent<Rigidbody>();
	}

	
	// Update is called once per frame
	void FixedUpdate () {
			DesiredOrientation = Quaternion.LookRotation (RB.velocity);

			this.pidController.Kp = this.Kp;
			this.pidController.Ki = this.Ki;
			this.pidController.Kd = this.Kd;

			// The PID controller takes the current orientation of an object, its desired orientation and the current angular velocity
			// and returns the required angular acceleration to rotate towards the desired orientation.
			Vector3 requiredAngularAcceleration = this.pidController.ComputeRequiredAngularAcceleration(this.currentTransform.rotation,
				DesiredOrientation,
				this.RB.angularVelocity,
				Time.fixedDeltaTime);

		//only apply PID correction if velocity is sufficiently high

			if (RB.velocity.magnitude > 10) { 
				this.RB.AddTorque (requiredAngularAcceleration, ForceMode.Acceleration);
			}
	}
}
}