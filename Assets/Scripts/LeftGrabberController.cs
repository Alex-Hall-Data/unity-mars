using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftGrabberController : MonoBehaviour {


	private bool grabberOpen;
	private bool grabberClosed;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}

	public void OpenGrabber(){

		if (!grabberOpen) {
			transform.Rotate (0, 0, 45);
			grabberOpen = true;
		}
	}

	public void CloseGrabber(){
		if (grabberOpen) {
			transform.Rotate (0, 0, -45);
			grabberOpen = false;
		}
	}
}
