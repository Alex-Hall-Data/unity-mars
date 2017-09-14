using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraneSegment : MonoBehaviour {

	private Rigidbody rigidBody;

	// Use this for initialization
	void Start () {
		rigidBody = GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void ExtendSection(){
		rigidBody.AddRelativeForce (0, 0,-200);
	}

	public void RetractSection(){
		rigidBody.AddRelativeForce ( 0, 0,200);
	}
}
