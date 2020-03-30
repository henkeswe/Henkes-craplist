using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	public enum CameraMode { Thirdperson, ThirdpersonSimple, Firstperson, Orbital, Shoulder, Freecam, Flight };

	public CameraMode cameraMode;
	[Header("Follow & Hiding")]
	public Transform target;
	public Transform hide;

	[Header("Settings")]
	public float mouseSensitivity = 6f;
	public float distFromTarget = 4f;
	public float rotationSmoothTime = 8f;
	public float rotationSmoothTimeSimple = 0.12f;

	Vector2 pitchMinMax; //= new Vector2(-35f, 85f);
	Vector2 scrollMinMax; //= new Vector2(-2f, 10f);

	Vector3 rotationSmoothVelocity;
	Vector3 currentRotation;

	//Updating;
	float yaw;
	float pitch;
	float scroll;

	//Current;
	Vector2 mouseVector;
	float scrollValue;
	[Header("Freecam Settings")]
	public float flySpeed = 2;

	[Header("Transparency")]
	public bool changeTransparency = true;
	public MeshRenderer targetRenderer;

	[Header("Camera Thirdperson Speeds")]
	public float moveSpeed = 10;
	public float returnSpeed = 9;
	public float wallPush = 0.7f;

	[Header("Distances")]
	public float closestDistanceToPlayer = 2;
	public float evenCloserDistanceToPlayer = 1;

	[Header("Mask")]
	public LayerMask collisionMask;

	private bool pitchLock = false;

	private void Start()
	{
		scrollMinMax = new Vector2(-distFromTarget + 2f, 10f);
	}

	private void Update()
	{
		mouseVector = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
		scrollValue = Input.GetAxis("Mouse ScrollWheel");

		if (cameraMode == CameraMode.Firstperson)
		{
			UpdateFirstpersonCamera(mouseVector);
		}

		if (cameraMode == CameraMode.Freecam)
		{
			UpdateFreeCamera(mouseVector);
		}


		if (Input.GetKeyDown(KeyCode.F1))
		{
			ToggleLockState();
		}

		if (Input.GetKeyDown(KeyCode.F3))
		{
			cameraMode = CameraMode.Freecam;
			hide.gameObject.SetActive(true);
			//target.GetComponent<UnityStandardAssets.Characters.FirstPerson.RigidbodyFirstPersonController>().enabled = false;
		}

		if(Input.GetKeyDown(KeyCode.F2))
		{
			//target.GetComponent<UnityStandardAssets.Characters.FirstPerson.RigidbodyFirstPersonController>().enabled = true;

			if (cameraMode == CameraMode.Thirdperson)
			{
				cameraMode = CameraMode.Firstperson;
				hide.gameObject.SetActive(false);
			}
			else
			{
				cameraMode = CameraMode.Thirdperson;
				hide.gameObject.SetActive(true);
			}
		}
	}

	private void FixedUpdate()//private void LateUpdate()
	{
		if(cameraMode == CameraMode.Thirdperson)
		{
			UpdateLateThirdpersonCamera(mouseVector, scrollValue);
		}

		if(cameraMode == CameraMode.Flight)
		{
			UpdateFlightCamera(mouseVector, scrollValue);
		}

		if(cameraMode == CameraMode.ThirdpersonSimple)
		{
			UpdateLateThirdpersonCameraSimple(mouseVector, scrollValue);
		}
	}

	//Todo: better way
	void UpdateFirstpersonCamera(Vector2 inputVector)
	{
		pitchMinMax = new Vector2(-90f, 90f);

		yaw += inputVector.x * mouseSensitivity;  //Input.GetAxis("Mouse X") * mouseSensitivity;
		pitch -= inputVector.y * mouseSensitivity;	//Input.GetAxis("Mouse Y") * mouseSensitivity;
		pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);

		currentRotation = new Vector3(pitch, yaw);

		transform.eulerAngles = new Vector3(currentRotation.x, currentRotation.y, 0);

		target.transform.eulerAngles = new Vector3(0, currentRotation.y, 0);
		//target.transform.eulerAngles = transform.eulerAngles = currentRotation;;

		transform.position = target.position + (Vector3.up * 0.8f);
	}

	void UpdateFreeCamera(Vector2 inputVector)
	{
		yaw += inputVector.x * mouseSensitivity;	//Input.GetAxis("Mouse X") * mouseSensitivity;
		pitch -= inputVector.y * mouseSensitivity;	//Input.GetAxis("Mouse Y") * mouseSensitivity;

		currentRotation = new Vector3(pitch, yaw);

		Vector3 moveDir = (transform.forward * Input.GetAxis("Vertical")) + (transform.right * Input.GetAxis("Horizontal"));

		if (Input.GetKey(KeyCode.LeftShift))
			moveDir = moveDir * flySpeed;

		transform.position += moveDir;
		transform.eulerAngles = currentRotation;
	}

	void UpdateLateThirdpersonCameraSimple(Vector2 inputVector, float scrollValue)
	{
		pitchMinMax = new Vector2(-35f, 85f);

		yaw += inputVector.x * mouseSensitivity;    //Input.GetAxis("Mouse X") * mouseSensitivity;
		pitch -= inputVector.y * mouseSensitivity;  //Input.GetAxis("Mouse Y") * mouseSensitivity;
		pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);

		scroll -= scrollValue;  //Input.GetAxis("Mouse ScrollWheel");
		scroll = Mathf.Clamp(scroll, scrollMinMax.x, scrollMinMax.y);

		currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref rotationSmoothVelocity, rotationSmoothTimeSimple);
		transform.eulerAngles = currentRotation;

		Vector3 e = transform.eulerAngles;
		e.x = 0;

		//if this is above, it will be dragging after
		transform.position = target.position - transform.forward * (distFromTarget + scroll);
		target.eulerAngles = e;
	}

	void UpdateLateThirdpersonCamera(Vector2 inputVector, float scrollValue)
	{
		WallCheck();
		CollisionCheck(target.position - transform.forward * (distFromTarget + scroll));
		//transform.position = 

		pitchMinMax = new Vector2(-35f, 85f);
		scroll -= scrollValue;
		scroll = Mathf.Clamp(scroll, scrollMinMax.x, scrollMinMax.y);

		if (!pitchLock)
		{
			yaw += inputVector.x * mouseSensitivity;
			pitch -= inputVector.y * mouseSensitivity;
			pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);
			currentRotation = Vector3.Lerp(currentRotation, new Vector3(pitch, yaw), rotationSmoothTime * Time.deltaTime);
		}
		else
		{
			yaw += inputVector.x * mouseSensitivity;
			pitch = pitchMinMax.y;
			currentRotation = Vector3.Lerp(currentRotation, new Vector3(pitch, yaw), rotationSmoothTime * Time.deltaTime);
		}

		transform.eulerAngles = currentRotation;
		Vector3 e = transform.eulerAngles;
		e.x = 0;
		target.eulerAngles = e;
	}

	void UpdateFlightCamera(Vector2 inputVector, float scrollValue)
	{
		pitchMinMax = new Vector2(-35f, 85f);

		yaw += inputVector.x * mouseSensitivity;    //Input.GetAxis("Mouse X") * mouseSensitivity;
		pitch -= inputVector.y * mouseSensitivity;  //Input.GetAxis("Mouse Y") * mouseSensitivity;
		pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);

		scroll -= scrollValue;  //Input.GetAxis("Mouse ScrollWheel");
		scroll = Mathf.Clamp(scroll, scrollMinMax.x, scrollMinMax.y);

		//currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref rotationSmoothVelocity, rotationSmoothTimeSimple);
		//transform.eulerAngles = target.eulerAngles ;//currentRotation;

		//Vector3 e = transform.eulerAngles;
		//e.x = 0;

		//if this is above, it will be dragging after
		transform.position = target.position - transform.forward * (distFromTarget * 2 + scroll);

		//transform.LookAt(target.transform);
		//target.eulerAngles = e;
	}

	private void CollisionCheck(Vector3 retPoint)
	{
		RaycastHit hit;

		if (Physics.Linecast(target.position, retPoint, out hit, collisionMask))
		{
			Vector3 norm = hit.normal * wallPush;
			Vector3 p = hit.point + norm;

			TransparencyCheck();

			if(Vector3.Distance(Vector3.Lerp(transform.position, p, moveSpeed * Time.deltaTime), target.position) <= evenCloserDistanceToPlayer)
			{
				//WallCheck();
			}
			else
			{
				transform.position = Vector3.Lerp(transform.position, p, moveSpeed * Time.deltaTime);
			}
			return;
		}

		FullTransparency();

		transform.position = Vector3.Lerp(transform.position, retPoint, returnSpeed * Time.deltaTime);
		pitchLock = false;
	}

	private void WallCheck()
	{
		Ray ray = new Ray(target.position, -target.forward);
		RaycastHit hit;

		if(Physics.SphereCast(ray, 0.5f, out hit, 0.7f, collisionMask))
		{
			pitchLock = true;
		}
		else
		{
			pitchLock = false;
		}
	}

	private void TransparencyCheck()
	{
		if(changeTransparency)
		{
			if(Vector3.Distance(transform.position, target.position) <= closestDistanceToPlayer)
			{
				Color temp = targetRenderer.sharedMaterial.color;

				temp.a = Mathf.Lerp(temp.a, 0.2f, moveSpeed * Time.deltaTime);

				targetRenderer.sharedMaterial.color = temp;
			}
			else
			{
				if(targetRenderer.sharedMaterial.color.a <= 0.99f)
				{
					Color temp = targetRenderer.sharedMaterial.color;

					temp.a = Mathf.Lerp(temp.a, 1f, moveSpeed * Time.deltaTime);

					targetRenderer.sharedMaterial.color = temp;
				}
			}
		}
	}

	private void FullTransparency()
	{
		if(changeTransparency)
		{
			if (targetRenderer.sharedMaterial.color.a <= 0.99f)
			{
				Color temp = targetRenderer.sharedMaterial.color;

				temp.a = Mathf.Lerp(temp.a, 1f, moveSpeed * Time.deltaTime);

				targetRenderer.sharedMaterial.color = temp;
			}
		}

	}

	void ToggleLockState()
	{
		if (Cursor.lockState == CursorLockMode.None)
		{

			Cursor.lockState = CursorLockMode.Locked;
			//Cursor.visible = false;
		}
		else
		{
			Cursor.lockState = CursorLockMode.None;
			//Cursor.visible = true;
		}
	}

}
