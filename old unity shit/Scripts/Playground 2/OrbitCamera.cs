using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitCamera : MonoBehaviour
{
	float camX;
	float camY;
	public float camSpeed = 2f;

	float curCamDist;
	float minCamDist = 1f;
	float maxCamDist = 10f;
	float scrollSpeed = 1f;

	Camera cam;

	private void Start()
	{
		cam = gameObject.GetOrAddChildComponent<Camera>("Camera");
	}

	private void Update()
	{
		UpdateCamera();
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
}
