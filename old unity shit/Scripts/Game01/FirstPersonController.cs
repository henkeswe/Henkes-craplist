using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
	public float mouseSensitivity = 6f;

	private bool clampVerticalRotation = true;
	private float MinimumX = -90f;
	private float MaximumX = 90f;

	private Quaternion characterTargetRot;
	private Quaternion cameraTargetRot;

	public AnimationCurve SlopeCurveModifier = new AnimationCurve(new Keyframe(-90.0f, 1.0f), new Keyframe(0.0f, 1.0f), new Keyframe(90.0f, 0.0f));

	private float forwardSpeed = 6.0f;
	private float backwardSpeed = 4.0f;
	private float strafeSpeed = 4.0f;
	private float runMultiplier = 2.0f;
	private float jumpForce = 50.0f;

	private float targetSpeed = 8.0f;

	private float shellOffset = 0.1f;

	private bool airControl = true;
	private bool airControlVelRotation = false;

	private bool isRunning;
	private bool isJumping;
	private bool isGrounded;
	private bool previouslyGrounded;
	private bool jumped;

	private KeyCode runKey = KeyCode.LeftShift;
	private KeyCode jumpKey = KeyCode.Space;

	private Vector3 groundContactNormal;
	private float groundCheckDistance = 0.1f;
	private float stickToGroundHelperDistance = 0.6f;

	//Components
	private Rigidbody rb;
	private CapsuleCollider col;

	public Camera cam;

	private void UpdateDesieredSpeed(Vector2 input)
	{
		if (input == Vector2.zero)
			return;

		if (input.x > 0 || input.x < 0)
			targetSpeed = strafeSpeed;

		if (input.y < 0)
			targetSpeed = backwardSpeed;

		if (input.y > 0)
			targetSpeed = forwardSpeed;

		if(Input.GetKey(runKey) && isGrounded)
		{
			targetSpeed *= runMultiplier;
			isRunning = true;
		}
		else
		{
			isRunning = false;
		}
	}

	/*
	public Vector3 GetVelocity()
	{
		return rb.velocity;
	}

	public bool IsGrounded()
	{
		return isGrounded;
	}

	public bool IsJumping()
	{
		return isJumping;
	}

	public bool IsRunning()
	{
		return isRunning;
	}
	*/

	private Vector2 GetInput()
	{
		Vector2 input = new Vector2(
			Input.GetAxisRaw("Horizontal"), 
			Input.GetAxisRaw("Vertical")
		);

		UpdateDesieredSpeed(input);
		return input;
	}

	void Start()
	{
		rb = GetComponent<Rigidbody>();
		col = GetComponent<CapsuleCollider>();

		if(rb.mass != 10f)
			rb.mass = 10f;

		characterTargetRot = transform.localRotation;
		cameraTargetRot = cam.transform.localRotation;
	}

	void Update()
	{
		RotateView();

		if (Input.GetKeyDown(jumpKey))
			jumped = true;
	}

	private void RotateView()
	{
		//avoids the mouse looking if the game is effectively paused
		if (Mathf.Abs(Time.timeScale) < float.Epsilon) return;

		// get the rotation before it's changed
		float oldYRotation = transform.eulerAngles.y;

		LookRotation(transform, cam.transform);

		if (isGrounded || airControl && airControlVelRotation)
		{
			//Rotate the rigidbody velocity to match the new direction that the character is looking
			Quaternion velRotation = Quaternion.AngleAxis(transform.eulerAngles.y - oldYRotation, Vector3.up);
			rb.velocity = velRotation * rb.velocity;
		}
	}

	private void LookRotation(Transform character, Transform camera)
	{
		float yRot = Input.GetAxis("Mouse X") * mouseSensitivity;
		float xRot = Input.GetAxis("Mouse Y") * mouseSensitivity;

		characterTargetRot *= Quaternion.Euler(0f, yRot, 0f);
		cameraTargetRot *= Quaternion.Euler(-xRot, 0f, 0f);


		if (clampVerticalRotation)
		{
			Quaternion q = cameraTargetRot;

			q.x /= q.w;
			q.y /= q.w;
			q.z /= q.w;
			q.w = 1.0f;

			float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

			angleX = Mathf.Clamp(angleX, MinimumX, MaximumX);

			q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

			cameraTargetRot = q;
		}

		character.localRotation = characterTargetRot;
		camera.localRotation = cameraTargetRot;
	}

	private void FixedUpdate()
	{
		GroundCheck();
		Vector2 input = GetInput();

		if (Mathf.Abs(input.x) > float.Epsilon || Mathf.Abs(input.y) > float.Epsilon)
		{

			if (!isGrounded && !airControl)
				return;

			// always move along the player forward as it is the direction that it being aimed at //RBFPC
			Vector3 desiredMove = (transform.forward * input.y) + (cam.transform.right * input.x);
			desiredMove = Vector3.ProjectOnPlane(desiredMove, groundContactNormal).normalized;

			desiredMove.x = desiredMove.x * targetSpeed;
			desiredMove.z = desiredMove.z * targetSpeed;
			desiredMove.y = desiredMove.y * targetSpeed;

			if (!isGrounded && airControl)
				desiredMove *= 0.25f;

			if (rb.velocity.sqrMagnitude < (targetSpeed * targetSpeed))
			{
				rb.AddForce(desiredMove * SlopeMultiplier(), ForceMode.Impulse);
			}
		}
		else
		{
			//Cancel out x, z movement on ground (TEMP)
			if(isGrounded) 
				rb.velocity = new Vector3(0, rb.velocity.y, 0);
		}

		if(isGrounded)
		{
			rb.drag = 5f;

			if (jumped)
			{
				rb.drag = 0f;
				rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
				rb.AddForce(new Vector3(0f, jumpForce, 0f), ForceMode.Impulse);
				isJumping = true;
			}

			if (!isJumping && Mathf.Abs(input.x) < float.Epsilon && Mathf.Abs(input.y) < float.Epsilon && rb.velocity.magnitude < 1f)
			{
				rb.Sleep();
			}
		}
		else
		{
			rb.drag = 0f;
			if (previouslyGrounded && !isJumping)
			{
				StickToGroundHelper();
			}
		}

		jumped = false;

		if (rb.velocity.y < -10f)
			Debug.Log("Falling!");
	}

	private float SlopeMultiplier()
	{
		float angle = Vector3.Angle(groundContactNormal, Vector3.up);
		return SlopeCurveModifier.Evaluate(angle);
	}

	private void StickToGroundHelper()
	{
		RaycastHit hitInfo;
		if (Physics.SphereCast(transform.position, col.radius * (1.0f - shellOffset), Vector3.down, out hitInfo,
							   ((col.height / 2f) - col.radius) +
							   stickToGroundHelperDistance, Physics.AllLayers, QueryTriggerInteraction.Ignore))
		{
			if (Mathf.Abs(Vector3.Angle(hitInfo.normal, Vector3.up)) < 85f)
			{
				rb.velocity = Vector3.ProjectOnPlane(rb.velocity, hitInfo.normal);
			}
		}
	}

	private void GroundCheck()
	{
		previouslyGrounded = isGrounded;
		RaycastHit hitInfo;
		if (Physics.SphereCast(transform.position, col.radius * (1.0f - shellOffset), Vector3.down, out hitInfo,
							   ((col.height / 2f) - col.radius) + groundCheckDistance, Physics.AllLayers, QueryTriggerInteraction.Ignore))
		{
			isGrounded = true;
			groundContactNormal = hitInfo.normal;
		}
		else
		{
			isGrounded = false;
			groundContactNormal = Vector3.up;
		}
		if (!previouslyGrounded && isGrounded && isJumping)
		{
			isJumping = false;
		}
	}

	//Prevents sticking to wall (TEMP)
	private void OnCollisionStay(Collision collision)
	{
		Vector3 normal = collision.contacts[0].normal;

		if (Vector2.Angle(Vector3.up, normal) > 80f)
		{
			rb.AddForce(normal, ForceMode.Impulse);
			

			Debug.Log("Hitting wall");
		}

	}
}
