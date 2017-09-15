using UnityEngine;

public class RotationFreezer : MonoBehaviour
{
	[Header("Freeze Local Rotation")]

	public bool x;

	public bool y;

	public bool z;

	Vector3 localRotation0;    //original local position

	private void Start()
	{
		SetOriginalLocalRotation();
	}

	private void Update ()
	{
		float x, y, z;


		if (this.x)
			x = localRotation0.x;
		else
			x = transform.localRotation.eulerAngles.x;

		if (this.y)
			y = localRotation0.y;
		else
			y = transform.localRotation.eulerAngles.y;

		if (this.z)
			z = localRotation0.z;
		else
			z = transform.localRotation.eulerAngles.z;


		transform.localEulerAngles = new Vector3(x, y, z);

	}

	public void SetOriginalLocalRotation()
	{
		localRotation0 = transform.localRotation.eulerAngles;
	}

}