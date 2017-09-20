using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerCamera : MonoBehaviour {

	private GameObject player;
	private float defaultZOffset;
	private float defaultYOffset;
	private float defaultXOffset;
	private float zOffset;
	private float yOffset;
	private float xOffset;
	private Vector3 offset;
	private float tiltAngle = 30f;
	private float xRotation;
	private float yRotation;
	private float zRotation;
	private Vector3 cameraPosition;
	float playerStartAltitude;


	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Active");
		playerStartAltitude= player.transform.position.y;
	}

	// Update is called once per frame
	void Update () {

		//controler for vehicle
		if (player.gameObject.GetComponent<SimpleCarController>() != null) {

			defaultZOffset = -50f;
			defaultYOffset = 50f;
			defaultXOffset = 0f;


			yRotation = player.transform.eulerAngles.y;

			zOffset = defaultZOffset * Mathf.Cos (yRotation * Mathf.PI / 180);
			xOffset = defaultZOffset * Mathf.Sin (yRotation * Mathf.PI / 180);
			offset = new Vector3 (xOffset, defaultYOffset, zOffset);

			transform.position = player.transform.position + offset;
			transform.eulerAngles = new Vector3 (tiltAngle, yRotation, 0f);
		}

		//controller for glider
		if (player.gameObject.GetComponent<GliderController>() != null) {
			defaultZOffset = 10f;
			defaultYOffset = 10f;
			defaultXOffset = -10f;

			float cameraLockAltitude = 200f;
			float currentAltitude = player.transform.position.y;

			//camera position corrections relative to altitude (below 50m they cause the camera to rotate above the glider
			float behindCorrectionFactor;
			float aboveCorrectionFactor;

			//camera distance correction factor - zooms out at higher altitude
			float distanceCorrectionFactor=Mathf.Log10(currentAltitude);

			if(currentAltitude<=cameraLockAltitude){
			behindCorrectionFactor = (currentAltitude - playerStartAltitude) / cameraLockAltitude; //close to zero at launch
				aboveCorrectionFactor = 1-behindCorrectionFactor;
			}else{
				behindCorrectionFactor=0.95f;
				aboveCorrectionFactor=0.05f;
			}

			xRotation = player.transform.eulerAngles.x;
			yRotation = player.transform.eulerAngles.y;
			zRotation = player.transform.eulerAngles.z;

			// Get the inverse of the players direction and above player too
			Vector3 directionbehind =  -(player.transform.forward );
			Vector3 directionabove = player.transform.up;

			//set camera target position
			cameraPosition =  player.transform.position + 
				(distanceCorrectionFactor*directionbehind * defaultZOffset*behindCorrectionFactor) + 
				(distanceCorrectionFactor*directionabove*defaultZOffset*aboveCorrectionFactor);
			transform.position = cameraPosition;

			//compensate for altitude dependant offset by rotating camera in x axis
			xRotation=xRotation-(Mathf.Atan(aboveCorrectionFactor/behindCorrectionFactor)*(180/Mathf.PI));


			//rotate camera with target 

			//transform.LookAt(player.transform);
			var relativeUp =player.transform.TransformDirection(Vector3.forward);
			var relativePos = player.transform.position - transform.position;
			transform.rotation = Quaternion.LookRotation (relativePos, relativeUp);


			//use inverse tan of altitude to get distanceabovecamera with alttude - use transform.up to move camera
	}
}
}
