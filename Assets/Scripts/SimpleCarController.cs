using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SimpleCarController : MonoBehaviour {
	public List<AxleInfo> axleInfos; // the information about each individual axle
	public float maxMotorTorque; // maximum torque the motor can apply to wheel
	public float maxSteeringAngle; // maximum steer angle the wheel can have

	private Camera camera;
	private float speed;
	public float maxSpeed = 1f;
	private Rigidbody rigidBody;

	void Start(){
		rigidBody = GetComponent<Rigidbody> ();
		camera = FindObjectOfType<Camera> ();
	}

	public void FixedUpdate()
	{
		camera.transform.position = transform.position + new Vector3 (0, 20, -20);

		speed = rigidBody.velocity.magnitude;


		float motor = maxMotorTorque * Input.GetAxis("Vertical");
		float steering = maxSteeringAngle * Input.GetAxis("Horizontal");

		foreach (AxleInfo axleInfo in axleInfos) {
			if (axleInfo.steering) {
				axleInfo.leftWheel.steerAngle = steering;
				axleInfo.rightWheel.steerAngle = steering;
			}

			//add torque (only if below max speed)

			if (axleInfo.motor && speed <= maxSpeed) {
				axleInfo.leftWheel.motorTorque = motor;
				axleInfo.rightWheel.motorTorque = motor;
			} else {
				axleInfo.leftWheel.motorTorque = 0;
				axleInfo.rightWheel.motorTorque = 0;
			}
		}
	}
}

[System.Serializable]
public class AxleInfo {
	public WheelCollider leftWheel;
	public WheelCollider rightWheel;
	public bool motor; // is this wheel attached to motor?
	public bool steering; // does this wheel apply steer angle?

}