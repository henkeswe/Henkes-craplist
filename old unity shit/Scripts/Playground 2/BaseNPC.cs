using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseNPC : MonoBehaviour
{
	public float walkSpeed = 1f;
	public float runSpeed = 3f;

	public float curMoveSpeed = 2f;

	public Rigidbody rb;
	public bool isGrounded;
	public float groundAngle;

	public Vector3 moveDir;
	public Quaternion moveAngle;

	public Vector3 nextMovePos;

	public bool useRandomMovePos = true;
	public float nextRandomMoveUpdate;

	public bool isMoving;

	public virtual void Start()
	{
		Random.InitState(System.DateTime.Now.Millisecond);

		moveAngle = transform.rotation;
		moveDir = new Vector3();

		rb = gameObject.GetOrAddComponent<Rigidbody>();
		rb.drag = Mathf.Infinity; //no slide
		rb.angularDrag = Mathf.Infinity; //no slide angle
		rb.useGravity = false; //has its own gravity
	}

	public virtual void FixedUpdate()
	{
		Vector3 curPos = new Vector3(transform.position.x, 0, transform.position.z);

		if (Time.time >= nextRandomMoveUpdate && useRandomMovePos)
		{
			nextMovePos = curPos + new Vector3(Random.Range(-20, 20), 0, Random.Range(-20, 20));
			nextRandomMoveUpdate = Time.time + Random.Range(5, 20);
		}

		if (Vector2.Distance(nextMovePos, curPos) > 5f)
			curMoveSpeed = runSpeed;
		else
			curMoveSpeed = walkSpeed;

		if (Vector3.Distance(nextMovePos, curPos) > 0.5f && isGrounded)
		{
			moveDir = transform.forward * curMoveSpeed;

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
		rb.MovePosition(transform.position + (moveDir * (Time.deltaTime)));
	}

	public virtual void OnCollisionEnter(Collision collision)
	{
		ContactPoint contact = collision.contacts[0];
		groundAngle = Vector3.Angle(contact.normal, Vector3.up);

		if (groundAngle <= 45f)
			isGrounded = true;
	}

	public virtual void OnCollisionStay(Collision collision)
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

	public virtual void OnCollisionExit(Collision collision)
	{
		isGrounded = false;
		groundAngle = 0;
	}


}
