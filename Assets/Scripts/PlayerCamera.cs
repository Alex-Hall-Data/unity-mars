using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO use trig identities to move camera around - can use rotation of gameobject

public class PlayerCamera : MonoBehaviour {

	private GameObject player;
	private static float defaultZOffset = -20f;
	private static float defaultYOffset = 20f;
	private static float defaultXOffset = 0f;
	private float zOffset;
	private float xOffset;
	private Vector3 offset = new Vector3 (defaultXOffset, defaultYOffset, defaultZOffset);
	private float tiltAngle = 30f;
	private float yRotation;


	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Active");
	}
	
	// Update is called once per frame
	void Update () {

		yRotation = player.transform.eulerAngles.y;

		zOffset = defaultZOffset * Mathf.Cos (yRotation*Mathf.PI/180);
		xOffset = defaultZOffset * Mathf.Sin (yRotation*Mathf.PI/180);
		offset = new Vector3 (xOffset, defaultYOffset, zOffset);

		transform.position = player.transform.position + offset;
		transform.eulerAngles=new Vector3(tiltAngle,yRotation,0f);
	}
}
