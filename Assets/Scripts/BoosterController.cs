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
	private ParticleSystem nozzleParticles;
	private ParticleSystem.EmissionModule em;

	// Use this for initialization
	void Start () {
		RB = transform.GetComponent<Rigidbody> ();
		nozzleParticles = GetComponent<ParticleSystem> ();
		em= nozzleParticles.emission;
	}
	
	// Update is called once per frame
	void FixedUpdate () {



		//ignite booster
		if (!boosterUsed && Input.GetKeyDown ("space")) {//if booster activated
			boosterActive = true;
			boosterUsed = true;
			em.enabled = true;
			nozzleParticles.Play ();
		}

		if (boosterActive && elapsedBurnTime < burnTime) {//if booster active
			RB.AddForce (transform.forward * thrust);
			elapsedBurnTime += Time.deltaTime;
		} else { //if booster spent or not active
			boosterActive = false;
			em.enabled = false;
		}
	}
}
