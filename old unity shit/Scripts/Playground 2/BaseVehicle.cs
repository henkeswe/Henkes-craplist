using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseVehicle : MonoBehaviour
{
	readonly KeyCode forwardKey = KeyCode.W;
	readonly KeyCode backwardKey = KeyCode.S;
	readonly KeyCode leftKey = KeyCode.A;
	readonly KeyCode rightKey = KeyCode.D;
	readonly KeyCode brakeKey = KeyCode.Space;

	Rigidbody rb;
	Camera cam;

	//Vehicle components
	public Transform[] frontTires;
	public Transform[] rearTires;

	WheelCollider[] frontWC;
	WheelCollider[] rearWC;

	Transform[] frontTireParents;
	Transform[] rearTireParents;

	int frontTireCount;
	int rearTireCount;

	//Vehicle basic
	float avgRPM;
	readonly float vehicleWeight = 1500f;
	bool isDrive;
	bool isSteer;
	bool isBrake;

	float steerAxis;
	float driveAxis;

	//Vehicle Engine & Wheels
	readonly float wheelRadius = 0.5f;
	readonly float wheelDamping = 5f;
	readonly float suspensionDistance = 0.1f;

	readonly float maxSteerAngle = 35f;
	readonly float steerRate = 100f;
	float curSteerAngle;

	readonly float torqueRate = 100f;
	readonly float maxTorquePower = 300f;
	public float curTorque;

	readonly float brakeRate = 500f;
	readonly float maxBrakePower = 2000f;
	float curBrake;

	//camera
	float camX;
	float camY;
	float curCamDist = 5f; //also works for start distance, but will obviously clamp to max distance anyway;
	readonly float camSpeed = 2f; //speed of mouse input
	readonly float minCamDist = 2f; //how close can the camera go
	readonly float maxCamDist = 5f; //how far can the camera go
	readonly float scrollSpeed = 2f; //how fast does scrolling affect distance

	void Start()
    {
		Setup();
		SetupWheels();
	}

    void Update()
    {
		UpdateCamera();
		UpdateInput();
		UpdateDrive();
		UpdateWheelVisuals();
	}

	void Setup()
	{
		rb = gameObject.GetOrAddComponent<Rigidbody>();
		cam = gameObject.GetOrAddChildComponent<Camera>("Camera");

		cam.SetFieldOfView(90f);

		rb.mass = vehicleWeight;
		Debug.Log("Rigidbody mass set to " + vehicleWeight);

		frontTireCount = frontTires.Length;
		rearTireCount = rearTires.Length;
	}

	void SetupWheels()
	{
		frontWC = new WheelCollider[frontTireCount];
		rearWC = new WheelCollider[rearTireCount];

		frontTireParents = new Transform[frontTireCount];
		rearTireParents = new Transform[rearTireCount];

		Transform tireContainer = new GameObject("Tires").transform;
		tireContainer.parent = transform;
		tireContainer.position = transform.position;

		Transform wcContainer = new GameObject("WheelColliders").transform;
		wcContainer.parent = transform;
		wcContainer.position = transform.position;

		for (int i = 0; i < frontTireCount; i++)
		{
			frontTireParents[i] = new GameObject("Front TireParent " + i).transform;
			frontTireParents[i].parent = tireContainer;
			frontTireParents[i].position = frontTires[i].position;
			frontTires[i].parent = frontTireParents[i];

			GameObject go = new GameObject("Front WheelCollider " + i);
			go.transform.parent = wcContainer;
			frontWC[i] = go.AddComponent<WheelCollider>();
			frontWC[i].transform.position = frontTires[i].position;
			frontWC[i].radius = wheelRadius;
			frontWC[i].wheelDampingRate = wheelDamping;
			frontWC[i].suspensionDistance = suspensionDistance;
		}
		Debug.Log("Setup " + frontTireCount + " front tires");

		for(int i = 0; i < rearTireCount; i++)
		{
			rearTireParents[i] = new GameObject("Front TireParent [" + i + "]").transform;
			rearTireParents[i].parent = tireContainer;
			rearTireParents[i].position = rearTires[i].position;
			rearTires[i].parent = rearTireParents[i];

			GameObject go = new GameObject("Rear WheelCollider [" + i + "]");
			go.transform.parent = wcContainer;
			rearWC[i] = go.AddComponent<WheelCollider>();
			rearWC[i].transform.position = rearTires[i].position;
			rearWC[i].radius = wheelRadius;
			rearWC[i].wheelDampingRate = wheelDamping;
			rearWC[i].suspensionDistance = suspensionDistance;
		}
		Debug.Log("Setup " + rearTireCount + " rear tires");
	}

	void UpdateInput()
	{
		driveAxis = (Input.GetKey(forwardKey) && Input.GetKey(backwardKey)) ? 0 : Input.GetKey(forwardKey) ? 1 : Input.GetKey(backwardKey) ? -1 : 0;
		steerAxis = (Input.GetKey(leftKey) && Input.GetKey(rightKey)) ? 0 : Input.GetKey(rightKey) ? 1 : Input.GetKey(leftKey) ? -1 : 0;

		isDrive = driveAxis != 0;
		isSteer = steerAxis != 0;
		isBrake = Input.GetKey(brakeKey);
	}


	void UpdateDrive()
	{
		if(isDrive)
		{
			curTorque += torqueRate * driveAxis * Time.deltaTime;
			curTorque = Mathf.Clamp(curTorque, -maxTorquePower, maxTorquePower);
		}
		else
		{
			curTorque = 0;
		}

		if(isSteer)
		{
			curSteerAngle += steerRate * steerAxis * Time.deltaTime;
			curSteerAngle = Mathf.Clamp(curSteerAngle, -maxSteerAngle, maxSteerAngle);
		}
		else
		{
			if(curSteerAngle != 0)
			{
				if (curSteerAngle > 0)
					curSteerAngle -= steerRate * Time.deltaTime;

				if (curSteerAngle < 0)
					curSteerAngle += steerRate * Time.deltaTime;

				if (curSteerAngle >= -1 && curSteerAngle <= 1)
					curSteerAngle = 0;
			}
		}

		if(isBrake)
		{
			if (isDrive && avgRPM > 10)
				curTorque = 0;

			if(curBrake < maxBrakePower)
				curBrake += brakeRate * Time.deltaTime;
		}
		else
		{
			curBrake = 0;
		}

		float rpmSum = 0;

		for (int i = 0; i < frontTireCount; i++)
		{
			frontWC[i].motorTorque = curTorque;
			frontWC[i].brakeTorque = curBrake;
			frontWC[i].steerAngle = curSteerAngle;

			rpmSum += frontWC[i].rpm;
		}

		for (int i = 0; i < rearTireCount; i++)
		{
			rearWC[i].motorTorque = curTorque;
			rearWC[i].brakeTorque = curBrake;

			rpmSum += rearWC[i].rpm;
		}

		avgRPM = rpmSum / 4;
	}

	void UpdateWheelVisuals()
	{
		for(int i = 0; i < frontTireCount; i++)
		{
			frontWC[i].GetWorldPose(out Vector3 pos, out Quaternion rot);
			frontTireParents[i].transform.position = pos;
			frontTireParents[i].transform.rotation = rot;
		}

		for(int i = 0; i < rearTireCount; i++)
		{
			rearWC[i].GetWorldPose(out Vector3 pos, out Quaternion rot);
			rearTireParents[i].transform.position = pos;
			rearTireParents[i].transform.rotation = rot;
		}
	}


	void UpdateCamera()
	{
		camX += Input.GetAxis("Mouse X") * camSpeed;
		camY -= Input.GetAxis("Mouse Y") * camSpeed;

		curCamDist = Mathf.Clamp(curCamDist - Input.GetAxis("Mouse ScrollWheel") * scrollSpeed, minCamDist, maxCamDist);

		Quaternion rotation = Quaternion.Euler(camY, camX, 0);
		Vector3 position = rotation * new Vector3(0, 0, -curCamDist) + transform.position;

		cam.transform.rotation = rotation;
		cam.transform.position = position;
	}

	float guiRPM;
	float guiUpdate;

	//simple gui
	void OnGUI()
	{
		if(Time.time >= guiUpdate)
		{
			guiRPM = Mathf.Round(avgRPM);
			guiUpdate = Time.time + 0.1f;
		}

		//speed
		string speedGuiText = "RPM: " + guiRPM;
		float speedGuiWidth = speedGuiText.Length * 10f;
		float speedGuiHeight =20f;

		Rect speedRect = new Rect(
			Screen.width - speedGuiWidth,
			Screen.height - speedGuiHeight,
			speedGuiWidth, speedGuiHeight
		);

		GUI.Box(speedRect, speedGuiText);

		
	}
}
