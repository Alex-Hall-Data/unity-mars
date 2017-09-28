using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VacuumBalloon : MonoBehaviour {

	private float airDensity;
	private float localAirDensity;
	private float balloonVolume;
	private Rigidbody RB;

	// Use this for initialization
	void Start () {
		airDensity = FindObjectOfType<Atmosphere> ().airDensity;
		balloonVolume = Mathf.Pow (40f, 3f)* (4f / 3f) * Mathf.PI; //don't like this - 40 is the balloon radius. Cant figure out how to hard code this in
		RB = GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		//get local atmospheric density (ie at balloon altitude
		localAirDensity=airDensity*((10000-transform.position.y)/10000); //TODO -improve this formula

	
		//buoyant force
		float buoyantForce = balloonVolume*localAirDensity*3.711f;
		RB.AddForce(0,buoyantForce,0);
		print (balloonVolume);
	}
}
