using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplePlayer : MonoBehaviour {

	private Vector2 moveVector;
	private Vector2 mouseVector;
	private Rigidbody rb;
	private float moveSpeed = 10f;

	public Camera cam;

	public Transform vehicleTransform;

	//camera movement
	private float yaw;
	private float pitch;

	void Start ()
	{
		rb = GetComponent<Rigidbody>();
	}
	
	void Update ()
	{
		moveVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
		mouseVector = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

		UpdateCamera();

		if(Input.GetKeyDown(KeyCode.E))
		{
			if(InVehicle())
			{
				vehicleTransform.GetComponent<BaseMountable>().DropDriver();
			}
			else if(CameraLookEntity().GetComponent<BaseMountable>() != null)
			{
				BaseMountable vehicle = CameraLookEntity().GetComponent<BaseMountable>();

				if (!vehicle.IsUsed())
				{
					vehicle.EnterVehicle(transform);
				}
			}
		}
	}

	private void FixedUpdate()
	{
		if(!InVehicle())
			UpdateMovement();
	}

	private void UpdateMovement()
	{
		Vector3 horizontal = (rb.transform.right * moveVector.x);
		Vector3 forward = (rb.transform.forward * moveVector.y);

		rb.MovePosition(transform.position + (((horizontal + forward) * moveSpeed) * Time.deltaTime));
	}

	private void UpdateCamera()
	{
		yaw += mouseVector.x * 6f;
		pitch -= mouseVector.y * 6f;
		pitch = Mathf.Clamp(pitch, -90, 90);

		Vector3 currentRotation = new Vector3(pitch, yaw);

		cam.transform.eulerAngles = new Vector3(currentRotation.x, cam.transform.eulerAngles.y, cam.transform.eulerAngles.z);

		transform.eulerAngles = new Vector3(transform.eulerAngles.x, currentRotation.y, transform.eulerAngles.z);
	}

	Vector3 CameraLookPosition()
	{
		Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit))
		{
			return hit.point;
		}

		return Vector3.zero;
	}

	Transform CameraLookEntity()
	{
		Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit))
		{
			return hit.transform;
		}

		return null;
	}
	
	public bool InVehicle()
	{
		return (vehicleTransform != null);
	}
}
