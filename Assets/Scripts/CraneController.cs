using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraneController : MonoBehaviour {

	private HingeJoint[] joints;
	private HingeJoint joint;
	private JointLimits jointLimits;
	private Rigidbody rigidBody;
	private Rigidbody[] rigidBodies;
	private ConfigurableJoint[] slidingJoints;
	public bool craneActive;
	public bool craneRotating;
	public bool craneExtending;
	public bool craneRetracting;

	// Use this for initialization
	void Start () {
		rigidBodies = GetComponentsInChildren<Rigidbody> ();
		rigidBody = rigidBodies [0];
		joints = GetComponentsInChildren<HingeJoint> ();
		joint = joints [0];
		jointLimits = joint.limits;
		slidingJoints = GetComponentsInChildren<ConfigurableJoint> ();
	}


	void Update () {
		if (craneActive) {
			BroadcastMessage ("CraneActive");
			jointLimits.min = -90;
			jointLimits.max = 90;
			joint.limits = jointLimits;

			if (craneRotating) {
				RotateCrane ();
			}

			if (craneExtending) {
				BroadcastMessage ("ExtendSection");
			}

			if (craneRetracting) {
				BroadcastMessage ("RetractSection");
			}


		} else {
			BroadcastMessage ("CraneInactive");
			jointLimits.min = 0;
			jointLimits.max = 0;
			joint.limits = jointLimits;


		}


}

	void RotateCrane(){
		rigidBody.AddRelativeTorque (new Vector3 (0, -100, 0));
	}
}
