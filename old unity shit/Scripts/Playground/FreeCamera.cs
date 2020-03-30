using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeCamera : MonoBehaviour
{
	/*
	private KeyCode keyJump = KeyCode.Space;
	private KeyCode keyCrouch = KeyCode.LeftControl;
	private KeyCode keySprint = KeyCode.LeftShift;

	private string axisForward = "Vertical";
	private string axisSideway = "Horizontal";
	private string axisMouseUP = "Mouse Y";
	private string axisMouseSideway = "Mouse X";
	*/
	public Material laserMaterial;
	private Camera cam;
	private Vector2 axisXY;

	private float fov = 90f;

	private float mouseSpeed = 5f;
	private float moveSpeed = 10f;

	private bool mouseLock = true;

	private LineRenderer lineLaser;

	void Start()
    {
		cam = this.GetComponent<Camera>();
		cam.transform.eulerAngles = Vector3.zero;
		cam.transform.localEulerAngles = Vector3.zero;

		CameraEX.SetHorizontalFOV(fov, cam);

		if (mouseLock)
		{
			Cursor.lockState = CursorLockMode.Locked;
			//Cursor.visible = true;
		}

		lineLaser = gameObject.AddComponent<LineRenderer>();
		lineLaser.material = laserMaterial;
		lineLaser.material.color = Color.green;
		lineLaser.startWidth = 0.2f;
		lineLaser.endWidth = 0.2f;
		lineLaser.enabled = false;
	}

	void UpdateFreeCamera()
	{
		float upMove = 0;
		float speed = moveSpeed;

		if (Input.GetKey(KeyCode.Space))
		{
			upMove = 1;
		}
		else
		if (Input.GetKey(KeyCode.LeftControl))
		{
			upMove = -1;
		}

		if (Input.GetKey(KeyCode.LeftShift))
		{
			speed = moveSpeed * 2f;
		}

		axisXY.y += Input.GetAxis("Mouse Y") * mouseSpeed;
		axisXY.x += Input.GetAxis("Mouse X") * mouseSpeed;

		Vector3 moveDir = (this.transform.up * upMove) + (this.transform.forward * Input.GetAxis("Vertical")) + (this.transform.right * Input.GetAxisRaw("Horizontal"));

		transform.eulerAngles = new Vector3(-axisXY.y, axisXY.x, 0);

		cam.transform.position += moveDir * Time.deltaTime * speed;
	}


	float distance;
	bool hasObject = false;
	GameObject objectt;

	void UpdateCameraGun()
	{
		Vector3 laserStartPos = (transform.position + transform.forward) + (transform.right * 0.1f);

		if (Input.GetKey(KeyCode.Mouse0))
		{
			lineLaser.enabled = true;

			lineLaser.SetPosition(0, laserStartPos);
			lineLaser.SetPosition(1, transform.position + (transform.forward * 10f));

			RaycastHit hit;

			if (!hasObject && Physics.Raycast(transform.position, transform.forward * 10f, out hit))
			{
				lineLaser.SetPosition(0, laserStartPos);
				lineLaser.SetPosition(1, transform.position + (transform.forward * hit.distance));

				Rigidbody rb = hit.transform.GetComponent<Rigidbody>();
				if (rb != null)
				{
					distance = hit.distance;
					hasObject = true;
					objectt = hit.transform.gameObject;
				}
			}
			else
			if(hasObject == true)
			{
				lineLaser.SetPosition(0, laserStartPos);
				lineLaser.SetPosition(1, transform.position + (transform.forward * distance));

				Rigidbody rb = objectt.transform.GetComponent<Rigidbody>();
				rb.isKinematic = true;
				rb.transform.position = transform.position + (transform.forward * distance);
				rb.transform.rotation = cam.transform.rotation;
				//rb.MovePosition(transform.position + (transform.forward * distance));
				//rb.MoveRotation(cam.transform.rotation);

			}
		}
		else
		{
			if(objectt != null)
			{
				distance = 0f;
				Rigidbody rb = objectt.transform.GetComponent<Rigidbody>();
				rb.isKinematic = false;
				objectt = null;
			}

			lineLaser.enabled = false;
			hasObject = false;
		}
	}

    void Update()
    {
		UpdateFreeCamera();
		//UpdateCameraGun();


	}
}
