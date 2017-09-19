using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO - the crane movements currently run from public bools - need to make a method which carries out the whole sequence on a key press
		//should alsomoveall physics to fiedupdate = not update

public class CraneController : MonoBehaviour {


	private Rigidbody rigidBody;
	private Rigidbody[] rigidBodies;
	private ConfigurableJoint[] slidingJoints;
	private bool craneRotationLocked =true;
	public bool craneActive;
	public bool craneRotatingOut;
	public bool craneRotatingBack;
	public bool craneExtending;
	public bool craneRetracting;
	public bool towerExtending;
	public bool towerRetracting;
	public bool grabberLowering;
	public bool grabberRaising;
	public bool grabberOpening;
	public bool grabberClosing;
	private Rigidbody towerRB;

	// Use this for initialization
	void Start () {
		rigidBodies = GetComponentsInChildren<Rigidbody> ();
		towerRB = rigidBodies [0];
		rigidBody = rigidBodies [1];
		slidingJoints = GetComponentsInChildren<ConfigurableJoint> ();
	}


	void Update () {



		if (craneActive) {
			if (craneRotationLocked) {
				BroadcastMessage ("UnlockCraneRotation");
				craneRotationLocked = false;
			}

			if (craneRotatingOut) {
				RotateCraneOut ();
			}

			if (craneRotatingBack) {
				RotateCraneBack ();
			}

			if (craneExtending) {
				BroadcastMessage ("ExtendSection");
			} else {
				BroadcastMessage ("DetentActive");
			}

			if (craneRetracting) {
				BroadcastMessage ("RetractSection");
			}

			if (towerExtending) {
				towerRB.AddRelativeForce (0, 1500f, 0);
				BroadcastMessage ("ExtendTower");
			}

			if (towerRetracting) {
				towerRB.AddRelativeForce (0, -1000f, 0);
				BroadcastMessage ("ExtendTower");
			}

			if (grabberLowering) {
				BroadcastMessage ("LowerGrabber");
			}

			if (grabberRaising) {
				BroadcastMessage ("RaiseGrabber");
			}

			if (grabberOpening) {
				BroadcastMessage ("OpenGrabber");
			}

			if (grabberClosing) {
				BroadcastMessage ("CloseGrabber");
			}




		} else {
			//BroadcastMessage ("RetractTower");

			//unlock crane rotation
			if (!craneRotationLocked) {
				BroadcastMessage ("LockCraneRotation");
				craneRotationLocked = true;
			}

		}


}

	void RotateCraneOut(){
		rigidBody.AddRelativeTorque (new Vector3 (0, -1000, 0));
	}

	void RotateCraneBack(){
		rigidBody.AddRelativeTorque (new Vector3 (0, 1000, 0));
	}
}
