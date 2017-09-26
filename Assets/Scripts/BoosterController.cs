using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoosterController : MonoBehaviour {

	public float burnTime=10f;
	public float thrust = 2000;

	public bool boosterUsed;
	public bool boosterActive;
	private float elapsedBurnTime=0f;
	private Rigidbody RB;

	// Use this for initialization
	void Start () {
		RB = transform.GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		//ignite booster
		if (!boosterUsed && Input.GetKeyDown ("space")) {
			boosterActive = true;
			boosterUsed = true;
		}

		if (boosterActive && elapsedBurnTime < burnTime) {
			RB.AddForce (transform.forward * thrust);
			elapsedBurnTime += Time.deltaTime;
		} else {
			boosterActive = false;
		}
	}
}
