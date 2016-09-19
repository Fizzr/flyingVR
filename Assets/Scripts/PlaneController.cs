using UnityEngine;
using System.Collections;

public class PlaneController : MonoBehaviour {

	private static float ROLLMOD= 0.2f * 2.8f;
	private static float PITCHMOD = ROLLMOD;
	private static float YAWMOD = ROLLMOD * 0.3f;
	private static float MOTORPOWER = 20f;
	private static float BANKMOD = 50f;
	private static float ANGLE_RANGE = 30f;

	public AudioSource explosionAudio;
	public Camera cam;
	private Rigidbody rb;
	private Vector3 offset;
	bool spun = false;
	bool stop = false;
	// Use this for initialization
	void Start () 
	{
		rb = GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		rb.AddRelativeForce (0, 0, MOTORPOWER);

		//Torque

		//DEBUG

		rb.AddRelativeTorque(Input.GetAxis("Vertical")* ANGLE_RANGE *PITCHMOD , 0 , Input.GetAxis("Horizontal")* ANGLE_RANGE *ROLLMOD*-1f);

		// VR
		if(!(Input.touchCount > 0))
		{
			Vector3 headRot = cam.transform.localRotation.eulerAngles;

			float xrot = headRot.x;
			if (xrot >180)
				xrot = xrot - 360;

			float yrot = headRot.y;
			if(yrot > 180)
				yrot = yrot - 360;

			float zrot = headRot.z;
			if(zrot > 180)
				zrot = zrot - 360;

			headRot = new Vector3(xrot, yrot, zrot);

			Vector3 torqueToAdd = new Vector3 (0,0,0);

			if(Mathf.Abs(xrot) >ANGLE_RANGE)
			{
				float sign = (xrot > 0? 1: -1);
				torqueToAdd.x = ANGLE_RANGE * sign;
			}
			else
			{
				torqueToAdd.x = xrot;
			}
			torqueToAdd.x = torqueToAdd.x * PITCHMOD;

			if(Mathf.Abs(yrot) >ANGLE_RANGE)
			{
				float sign = (yrot > 0? 1: -1);
				torqueToAdd.y = ANGLE_RANGE * sign;
			}
			else
			{
				torqueToAdd.y = yrot;
			}
			torqueToAdd.y = torqueToAdd.z * YAWMOD;

			if(Mathf.Abs(zrot) >ANGLE_RANGE)
			{
				float sign = (zrot > 0? 1: -1);
				torqueToAdd.z = ANGLE_RANGE * sign;
			}
			else
			{
				torqueToAdd.z = zrot;
			}
			torqueToAdd.z = torqueToAdd.z * ROLLMOD;

			rb.AddRelativeTorque(torqueToAdd);				////// Make more advanced!
		}
		// Lift
		Vector3 rot = transform.rotation.eulerAngles;
		Vector3 vel = transform.InverseTransformVector(rb.velocity);
		float forwardVel = vel.z;

		//pitch
		float pitch = rot.x;
		if(pitch > 180)
		{
			pitch = Mathf.Abs(pitch - 360);
			if(pitch > 90)
			{
				pitch = Mathf.Abs(pitch - 180);
			}
		}
		rb.AddForce(0f, -MOTORPOWER * 1.2f * Mathf.Pow((pitch/90f),2f), 0f);


		////Roll
		if(pitch < 70)				//fucky shit happens with euler angles if pitched too far up. so lets just ignore it. 
		{
			float roll = rot.z;
			float sign = -1;
			if(roll > 180)
			{
				sign = 1f;
				roll = Mathf.Abs(roll - 360); // <- 180, -180 ->
			}
			if(roll > 90)
				roll= Mathf.Abs(roll -180); // 0 to 90, 90 to 0			//Doesn't matter which side, plane is tilting, or if plane is upside down (kinda. simplified)
			
			rb.AddForce(0f, -rb.mass*9f * 0.5f * Mathf.Pow(roll/90f, 3f),0f);  //Gravity pulling down
			rb.AddTorque(0, sign * (roll/90f)/2 * BANKMOD, 0);
		}
	}

	void OnCollisionEnter(Collision collision)
	{
		rb.AddExplosionForce(1000, collision.contacts[0].point,200);
		print("Collider: "+ collision.gameObject.ToString());
		explosionAudio.transform.position = collision.contacts[0].point;
		explosionAudio.Play();
	}

}
