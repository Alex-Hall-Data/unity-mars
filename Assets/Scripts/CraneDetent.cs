using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraneDetent : MonoBehaviour {

	private Collider collider;

	// Use this for initialization
	void Start () {
		collider = GetComponent<Collider> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void CraneActive(){
		collider.enabled = false;
	}

	public void CraneInactive(){
		collider.enabled = true;
	}


}
