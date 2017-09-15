using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO
//make tower extend with messsagecall - no idea why nt working here
//removearay indexing method for finding components
//remove timer from crane detent and find a better way of doing it.

public class CraneTower : MonoBehaviour {

	//private Rigidbody towerRigidBody;
	//private float extendForce=1000f;

	private ConfigurableJoint towerJoint;
	private SoftJointLimit UpperXLimit;
	private SoftJointLimit LowerXLimit;

	// Use this for initialization
	void Start () {
		towerJoint = GetComponent<ConfigurableJoint> ();
		UpperXLimit = towerJoint.highAngularXLimit;
		LowerXLimit = towerJoint.lowAngularXLimit;
	}

	void Update(){
		
	
	}

	public void ExtendTower(){
	//	towerRigidBody.AddRelativeForce (0, extendForce, 0);
	}

	/*public void RetractTower(){
		towerRigidBody.AddRelativeForce (0, 0, 0);
	}*/

	public void LockCraneRotation(){
		UpperXLimit.limit =0f;
		LowerXLimit.limit = 0f;
	}

	public void UnlockCraneRotation(){
		UpperXLimit.limit = 90f;
		LowerXLimit.limit = 0f;
	}
}
