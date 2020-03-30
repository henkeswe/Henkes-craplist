using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VehicleBase : MonoBehaviour
{
	public Text debugText;

	public Camera camera;
	public Transform fPivot;
	public AudioSource audio;

	public AudioClip idle;
	public AudioClip warmup;
	public AudioClip full;

	public List<WheelCollider> frontWheels;
	public List<WheelCollider> backWheels;

	public List<GameObject> frontWheelVisual;
	public List<GameObject> backWheelVisual;

	public List<Light> lights;

	float motorTorque = 150;
	float steeringAngle = 25f;

	float brake = 0;

	bool thirdperson = true;

	//ParticleSystem particles;
	// Use this for initialization
	void Start ()
	{
		//particles = GetComponentInChildren<ParticleSystem>();
	}

	bool hasWarmup = false;


	private void FixedUpdate()
	{
		debugText.text = "Velocity: " + GetComponent<Rigidbody>().velocity;

		float torque = motorTorque * Input.GetAxis("Vertical");
		float steering = steeringAngle * Input.GetAxis("Horizontal");

		if(torque > 0)
		{
			if(!hasWarmup)
			{
				audio.clip = warmup;
				audio.Play();

				hasWarmup = true;
			}

			if (hasWarmup && !audio.isPlaying)
			{
				audio.clip = full;
				audio.Play();
			}

		}
		else
		{
			hasWarmup = false;
			if(audio.clip != idle)
			{
				if(audio.isPlaying)
					audio.Stop();


				audio.clip = idle;
			}

			if(audio.clip == idle && !audio.isPlaying)
			{
				audio.Play();
			}
		}


		if (Input.GetKey(KeyCode.Space))
		{
			brake = 1000f;
		}
		else
		{
			brake = 0f;
		}

		for(int i = 0; i < frontWheels.Count; i++)
		{
			frontWheels[i].motorTorque = torque;
			frontWheels[i].steerAngle = steering;

			frontWheels[i].brakeTorque = brake;

			Vector3 position;
			Quaternion rotation;
			frontWheels[i].GetWorldPose(out position, out rotation);

			frontWheelVisual[i].transform.position = position;
			frontWheelVisual[i].transform.rotation = rotation;
		}

		for (int i = 0; i < backWheels.Count; i++)
		{
			backWheels[i].motorTorque = torque;

			Vector3 position;
			Quaternion rotation;
			backWheels[i].GetWorldPose(out position, out rotation);

			backWheelVisual[i].transform.position = position;
			backWheelVisual[i].transform.rotation = rotation;

			//if (brake > 0)
			//	particles.transform.position = position;
		}

	}

	float velocityX = 0.0f;
	float velocityY = 0.0f;
	float mouseSpeed = 6.0f;
	float rotationYAxis = 0.0f;
	float rotationXAxis = 0.0f;
	float distance = 5.0f;

	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyDown(KeyCode.F))
		{
			foreach (Light light in lights)
			{
				light.gameObject.SetActive(!light.gameObject.activeSelf);
			}
		}

		distance -= Input.GetAxis("Mouse ScrollWheel");

		velocityX = Input.GetAxis("Mouse X") * mouseSpeed;// * distance; //* 0.02f;
		velocityY = Input.GetAxis("Mouse Y") * mouseSpeed;// * distance;

		rotationYAxis += velocityX;
		rotationXAxis -= velocityY;

		if (distance <= 2)
		{
			distance = 2;
			thirdperson = false;
		}
		else
		{
			thirdperson = true;
		}
			//distance = 0;

		if (thirdperson)
		{

			//Quaternion fromRotation = Quaternion.Euler(camera.transform.rotation.eulerAngles.x, camera.transform.rotation.eulerAngles.y, 0);

			Quaternion toRotation = Quaternion.Euler(rotationXAxis, rotationYAxis, 0);
			Quaternion rotation = toRotation;

			Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
			Vector3 position = rotation * negDistance + transform.position;

			camera.transform.rotation = rotation;
			camera.transform.position = position;
		}
		else
		{
			camera.transform.position = fPivot.transform.position;
			camera.transform.rotation = Quaternion.Euler(rotationXAxis, rotationYAxis, 0);
		}


		
	}
}
