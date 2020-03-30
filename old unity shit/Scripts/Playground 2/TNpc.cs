using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TNpc : MonoBehaviour
{
	public AnimationClip walkClip;
	public AnimationClip idleClip;
	public AnimationClip runClip;
	public Animation anim;


	float walkSpeed = 1f;
	float runSpeed = 3f;

	float moveSpeed = 2f;

	Rigidbody rb;
	bool isGrounded;
	float groundAngle;

	Vector3 moveDir;
	Quaternion moveAngle;

	Vector3 nextMovePos;

	float nextRandomMoveUpdate;

	bool isMoving;

    void Start()
    {
		Random.InitState(System.DateTime.Now.Millisecond);

		moveAngle = transform.rotation;
		moveDir = new Vector3();

		rb = gameObject.GetOrAddComponent<Rigidbody>();
		rb.drag = Mathf.Infinity; //no slide
		rb.angularDrag = Mathf.Infinity; //no slide angle
		rb.useGravity = false; //has its own gravity

		SetupAnimation();

	}


	void SetupAnimation()
	{
		anim[walkClip.name].speed = 1.5f;
		anim[runClip.name].speed = 2f;

		anim[walkClip.name].wrapMode = WrapMode.Loop;
		anim[runClip.name].wrapMode = WrapMode.Loop;
		anim[idleClip.name].wrapMode = WrapMode.Loop;
	}

	void Update()
	{
		UpdateAnimation();
	}

	void LateUpdate()
	{
		LateAnimation();
	}

	void LateAnimation()
	{
		
	}

	void UpdateAnimation()
	{
		if(!isGrounded)
		{
			anim.CrossFade(idleClip.name,0.5f);
			return;
		}

		if(isMoving && moveSpeed == walkSpeed && anim.clip != walkClip)
		{
			anim.CrossFade(walkClip.name, 0.5f);
		}

		if(isMoving && moveSpeed == runSpeed && anim.clip != runClip)
		{
			anim.CrossFade(runClip.name, 0.5f);

		}

		if(!isMoving && anim != idleClip)
		{
			anim.CrossFade(idleClip.name, 0.5f);
		}

		if (!anim.isPlaying)
			anim.Play();
	}

	void FixedUpdate()
    {
		Vector3 curPos = new Vector3(transform.position.x, 0, transform.position.z);

		if (Time.time >= nextRandomMoveUpdate)
		{
			nextMovePos = curPos + new Vector3(Random.Range(-20, 20), 0, Random.Range(-20, 20));
			nextRandomMoveUpdate = Time.time + Random.Range(5, 20);
		}

		if (Vector2.Distance(nextMovePos, curPos) > 5f)
			moveSpeed = runSpeed;
		else
			moveSpeed = walkSpeed;

		if (Vector3.Distance(nextMovePos, curPos) > 0.5f && isGrounded)
		{
			moveDir = transform.forward;

			//calculate Y angle, could be done in a more simple way because it's one float..
			Quaternion moveRotY = Quaternion.RotateTowards(
				transform.rotation,
				Quaternion.LookRotation(nextMovePos - curPos),
				1f
			);

			//rotate towards move location, keeping other rotation in mind
			moveAngle.eulerAngles = new Vector3(
				moveAngle.eulerAngles.x,
				moveRotY.eulerAngles.y,
				moveAngle.eulerAngles.z
			);
			isMoving = true;

		}
		else
		{
			moveDir = Vector3.zero;
			isMoving = false;
		}

		if (!isGrounded)
			moveDir.y = -1f;

		rb.MoveRotation(moveAngle);
		rb.MovePosition(transform.position + (moveDir * (Time.deltaTime * moveSpeed)));
	}

	private void OnCollisionEnter(Collision collision)
	{
		ContactPoint contact = collision.contacts[0];
		groundAngle = Vector3.Angle(contact.normal, Vector3.up);

		if (groundAngle <= 45f)
			isGrounded = true;
	}

	private void OnCollisionStay(Collision collision)
	{
		ContactPoint contact = collision.contacts[0];
		groundAngle = Vector3.Angle(contact.normal, Vector3.up);

		if (groundAngle <= 45f)
		{
			isGrounded = true;

			moveAngle = Quaternion.RotateTowards(
				transform.rotation,
				Quaternion.LookRotation(Vector3.Cross(-contact.normal, transform.right), contact.normal),
				2f
			);
		}
	}

	private void OnCollisionExit(Collision collision)
	{
		isGrounded = false;
		groundAngle = 0;
	}


}
