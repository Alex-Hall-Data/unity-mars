using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabberController : MonoBehaviour {

	private float grabberMinPosition = -0.035f;
	private float grabberMaxPosition = -0.001f;
	private float xpos;
	private float ypos;
	private float zpos;
	// Use this for initialization
	void Start () {
		

		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void LowerGrabber(){

		xpos=transform.position.x;
		zpos=transform.position.z;
		ypos=transform.position.y;

		if (transform.localPosition.y >= grabberMinPosition) {
			ypos -=   Time.deltaTime;
			transform.position = new Vector3 (xpos, ypos, zpos);

		}
	}

	public void RaiseGrabber(){
		xpos=transform.position.x;
		zpos=transform.position.z;
		ypos=transform.position.y;

		if (transform.localPosition.y <= grabberMaxPosition) {
			ypos +=   Time.deltaTime;
			transform.position = new Vector3 (xpos, ypos, zpos);

		}
	}
}
