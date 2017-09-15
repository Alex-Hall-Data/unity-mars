using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraneDetent : MonoBehaviour {

	private Collider Detentcollider;

	// Use this for initialization
	void Start () {
		Detentcollider = GetComponent<Collider> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void ExtendSection(){
		Detentcollider.enabled = false;
	}

	public void DetentActive(){
		Invoke ("EnableDetent", 5);
	}

	void EnableDetent(){
		Detentcollider.enabled = true;
	}

}
