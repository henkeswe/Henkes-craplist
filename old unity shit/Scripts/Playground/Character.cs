using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControllerSettings
{
	public readonly float Height = 1.8f;
	public readonly float WalkSpeed = 3f;
	public readonly float CrouchWalkSpeed = 2f;
	public readonly float JumpSpeed = 5f;
	public readonly float SprintSpeed = 5f;

	public float Weight = 80f; //rough weight "system"
	public readonly float DefaultWeight = 80f;
	public readonly float MinWeight = 30f;
	public readonly float MaxWeight = 200f;
}

public class CameraSettings
{
	public float CamMouseInputSpeed = 3f;
	public bool CamRawMouseInput = false;
	public readonly float MaxCamAngleY = 90f;

	public float CamFOV = 90f;
	public readonly float DefaultCamFOV = 90f;
	public readonly float MinCamFOV = 50f;
	public readonly float MaxCamFOV = 120f;
}

public class Character : MonoBehaviour
{
	//private enum Mode { Creative, Normal }
	//private Mode mode = Mode.Creative;

	public GameObject model;

	//Camera & Camera Settings
	public Camera cam;
	private Vector2 c_CamDir;
	private CameraDevSettings cs = new CameraDevSettings();

	//Character & Camera Settings
	private CharacterController cc;
	private Vector3 cc_MoveDir;
	private Vector2 cc_moveInputs;
	private CharacterDevControllerSettings ccs = new CharacterDevControllerSettings();

	private Vector3 originalScale;

	void Start ()
	{
		SetCameraFOV(cam, cs.CamFOV);
		cc = GetComponent<CharacterController>();
		cc.height = ccs.Height;
		model.transform.localScale = new Vector3(1, 1 - ccs.Height, 1);

		originalScale = transform.localScale;
	}

	private void SetCameraFOV(Camera cam, float fov)
	{
		if (fov > cs.MaxCamFOV)
		{
			Debug.Log("SetCameraFOV: Camera FOV must be between " + cs.MinCamFOV + " and " + cs.MaxCamFOV);
			fov = cs.DefaultCamFOV;
		}

		//Thanks zo1d
		float hFOVrad = fov * Mathf.Deg2Rad;
		float camH = Mathf.Tan(hFOVrad * 0.5f) / cam.aspect;
		float vFOVrad = Mathf.Atan(camH) * 2;
		cam.fieldOfView = vFOVrad * Mathf.Rad2Deg;
	}

	void UpdateCamera()
	{
		if(cs.CamRawMouseInput)
		{
			c_CamDir.x += Input.GetAxisRaw("Mouse X") * cs.CamMouseInputSpeed;
			c_CamDir.y -= Input.GetAxisRaw("Mouse Y") * cs.CamMouseInputSpeed;
		}
		else
		{
			c_CamDir.x += Input.GetAxis("Mouse X") * cs.CamMouseInputSpeed;
			c_CamDir.y -= Input.GetAxis("Mouse Y") * cs.CamMouseInputSpeed;
		}


		c_CamDir.y = Mathf.Clamp(c_CamDir.y, -cs.MaxCamAngleY, cs.MaxCamAngleY);
		cc.transform.eulerAngles = new Vector3(cc.transform.eulerAngles.x, c_CamDir.x, cc.transform.eulerAngles.z);
		cam.transform.eulerAngles = new Vector3(c_CamDir.y, cam.transform.eulerAngles.y, cam.transform.eulerAngles.z);
	}

	void PushController(Vector3 dir, float power)
	{
		cc_MoveDir += dir * power;
	}

	void UpdateMovement()
	{

		//Resets dir y on grounded while also applying a small constant down force so grounded works propperly.
		//Could be put below, but this is so its easy to understand.
		if (cc.isGrounded)
			cc_MoveDir.y = -0.1f;
		
		//Check if input first, to not destroy momentums
		if (Input.GetAxisRaw("Vertical") != 0) //Always keep raw, or it wont detect propperly
			cc_moveInputs.y = Input.GetAxisRaw("Vertical"); //if not raw, movement wont be as snappy

		if (Input.GetAxisRaw("Horizontal") != 0) //Always keep raw, or it wont detect propperly
			cc_moveInputs.x = Input.GetAxisRaw("Horizontal"); //if not raw, movement wont be as snappy

		if (cc.isGrounded)
		{
			//Slide a bit if no movement
			float slideStart = 0.25f;
			float decreaseMargin = 0.9f; //Higher = more slide, max 0.99

			if (cc_moveInputs.x != 0)
			{
				cc_moveInputs.x *= decreaseMargin;

				if (cc_moveInputs.x > 0 && cc_moveInputs.x < slideStart || cc_moveInputs.x < 0 && cc_moveInputs.x > -slideStart)
					cc_moveInputs.x = 0;
			}

			if (cc_moveInputs.y != 0)
			{
				cc_moveInputs.y *= decreaseMargin;

				if (cc_moveInputs.y > 0 && cc_moveInputs.y < slideStart || cc_moveInputs.y < 0 && cc_moveInputs.y > -slideStart)
					cc_moveInputs.y = 0;
			}
		}
		
		//Movement speed adjust
		float curMoveSpeed = ccs.WalkSpeed;

		if (Input.GetButton("Sprint"))
			curMoveSpeed = ccs.SprintSpeed;

		if (Input.GetButton("Crouch"))
		{
			curMoveSpeed = ccs.CrouchWalkSpeed;
			cc.height = 1f;
		}

		if(Input.GetButtonUp("Crouch"))
		{
			cc.height = ccs.Height;
		}
		
		//Movedir from info above
		cc_MoveDir = (transform.forward * (cc_moveInputs.y * curMoveSpeed)) + (transform.right * (cc_moveInputs.x * curMoveSpeed)) + new Vector3(0, cc_MoveDir.y, 0);

		if (Input.GetButtonDown("Jump") && cc.isGrounded)
		{
			cc_MoveDir.y = 0; //Reset dir Y on jump so theres consistency.
			cc_MoveDir.y += ccs.JumpSpeed;
		}

		if (!cc.isGrounded)
		{
			//Rough weight method with gravity
			cc_MoveDir += (Physics.gravity / 32) * (ccs.Weight / ccs.DefaultWeight);
		}

		cc.Move(cc_MoveDir * Time.deltaTime);
	}


	void Update ()
	{
		//Implent later, perhaps helps with crouching..
		//https://docs.unity3d.com/ScriptReference/CollisionFlags.html

		UpdateCamera();
		UpdateMovement();
	}


	void OnControllerColliderHit(ControllerColliderHit hit)
	{
		//Unity documentation push object snipplet.
		Rigidbody rb = hit.collider.attachedRigidbody;

		if(hit != null)
		{


			transform.parent = hit.transform;

			Vector3 scaleTmp = originalScale;
			scaleTmp.x /= hit.transform.localScale.x;
			scaleTmp.y /= hit.transform.localScale.y;
			scaleTmp.z /= hit.transform.localScale.z;

			//Vector3 eulerTmp = transform.eulerAngles;
			//eulerTmp.x /= hit.transform.eulerAngles.x;
			//eulerTmp.y = transform.eulerAngles.y;
			//eulerTmp.z /= hit.transform.eulerAngles.z;

			transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f);
			//transform.eulerAngles = eulerTmp;
			transform.localScale = scaleTmp;

		}

		if (rb != null && rb.velocity != Vector3.zero)
		{
			Vector3 a = Vector3.Project(rb.velocity, cc.transform.position - rb.transform.position);
			PushController(Vector3.Project(rb.velocity, hit.point - hit.normal), 1f);
			Debug.DrawRay(hit.point, hit.normal, Color.red, 2f); //Shows dir actually
			Debug.DrawLine(hit.point - hit.normal, (hit.point - hit.normal) * 10);
		}


		// no propper rigidbody
		if (rb == null || rb.isKinematic)
		{
			return;
		}


		// We dont want to push objects below us
		if (hit.moveDirection.y < 0)
		{
			return;
		}

		// Calculate push direction from move direction,
		// we only push objects to the sides never up and down
		Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);

		// Apply the push
		float weight = (rb.mass * 0.1f);

		if (weight < 1)
			weight = 1;

		rb.velocity = (pushDir + hit.controller.velocity) / weight;

	}
}
