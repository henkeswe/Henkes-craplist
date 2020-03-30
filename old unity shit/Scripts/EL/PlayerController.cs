using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	[Header("Player Options")]
	public float playerHeight;

	[Header("Movement Options")]
	public float movementSpeed;
	public bool smooth;
	public float smoothSpeed;

	[Header("Jump Options")]
	public float jumpForce;
	public float jumpSpeed;
	public float jumpDecrease;
	public float incrementJumpFallSpeed = 0.1f;

	[Header("Gravity Options")]
	public float gravity = 2.5f;

	[Header("Physics")]
	public LayerMask discludePlayer;
	//Private Varibles

	[Header("References")]
	public SphereCollider sphereCol;

	//Movement Vectors;
	private Vector3 velocity;
	private Vector3 move;
	private Vector3 vel;

	private void Update()
	{
		Gravity();
		SimpleMove();
		Jump();
		FinalMove();
		GroundChecking();
		CollisionCheck();
	}

	#region Movement
	private void SimpleMove()
	{
		move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		velocity += move;

		if (rider.serverInput.IsDown(BUTTON.RIGHT))
		{
			npc.ServerRotation = Quaternion.Euler(npc.ServerRotation.eulerAngles + new Vector3(0, 10, 0));
		}

		if (rider.serverInput.IsDown(BUTTON.LEFT))
		{
			npc.ServerRotation = Quaternion.Euler(npc.ServerRotation.eulerAngles + new Vector3(0, -10, 0));
		}
	}

	private void FinalMove()
	{
		Vector3 vel = new Vector3(velocity.x, velocity.y, velocity.z) * movementSpeed;
		//velocity = (new Vector3(move.x, -currentGravity, move.z) + vel) * movementSpeed;
		//velocity = transform.TransformDirection(velocity);
		vel = transform.TransformDirection(vel);
		transform.position += vel * Time.deltaTime;

		velocity = Vector3.zero;
		//vel = Vector3.zero;
	}
	#endregion
	
	#region Gravity & Grounding
	//Gravity Private Variables
	private bool grounded;

	//Grounded Private Variables
	private RaycastHit groundHit;
	private Vector3 liftPoint = new Vector3(0, 1.2f, 0);
	private Vector3 groundCheckPoint = new Vector3(0, -0.87f, 0);


	private void Gravity()
	{
		if(grounded == false)
		{
			velocity.y -= gravity;
			//currentGravity = gravity;
		}
	}

	private void GroundChecking()
	{
		Ray ray = new Ray(transform.TransformPoint(liftPoint), Vector3.down);
		RaycastHit tempHit = new RaycastHit();

		if (Physics.SphereCast(ray, 0.17f, out tempHit, 20f, discludePlayer))
		{
			GroundConfirm(tempHit);
			//grounded = true;
		}
		else
		{
			grounded = false;
		}
	}

	private void GroundConfirm(RaycastHit tempHit)
	{
		Collider[] col = new Collider[3];
		int num = Physics.OverlapSphereNonAlloc(transform.TransformPoint(groundCheckPoint), 0.57f, col, discludePlayer);

		grounded = false;

		for(int i = 0; i < num; i++)
		{
			if (col[i].transform == tempHit.transform)
			{
				groundHit = tempHit;
				grounded = true;
				if(inputJump == false)
				{
					Vector3 avg = new Vector3(transform.position.x, (groundHit.point.y + playerHeight / 2), transform.position.z);

					if (!smooth)
					{
						transform.position = avg;
					}
					else
					{
						transform.position = Vector3.Lerp(transform.position, avg, smoothSpeed * Time.deltaTime);
					}
				}
				break;
			}
		}

		if(num <= 1 && tempHit.distance <= 3.1f && inputJump == false)
		{
			if (col[0] != null)
			{
				Ray ray = new Ray(transform.TransformPoint(liftPoint), Vector3.down);
				RaycastHit hit;

				if(Physics.Raycast(ray, out hit, 3.1f, discludePlayer))
				{
					if(hit.transform != col[0].transform)
					{
						grounded = false;
						return;
					}
				}
			}
		}

	}
	#endregion

	#region Collision
	private void CollisionCheck()
	{
		Collider[] overlaps = new Collider[4];
		Collider myCollider = new Collider();
		//Physics.OverlapCapsule
		//int num = Physics.OverlapCapsuleNonAlloc(transform.TransformPoint(sphereCol.center), sphereCol.radius, overlaps, discludePlayer, QueryTriggerInteraction.UseGlobal);
		//int num = Physics.OverlapSphereNonAlloc(transform.TransformPoint(sphereCol.center), sphereCol.radius, overlaps, discludePlayer, QueryTriggerInteraction.UseGlobal);
		int num = 0;

		if(sphereCol != null)
		{
			num = Physics.OverlapSphereNonAlloc(transform.TransformPoint(sphereCol.center), sphereCol.radius, overlaps, discludePlayer, QueryTriggerInteraction.UseGlobal);
			myCollider = sphereCol;
		}

		for(int i = 0; i < num; i++)
		{
			Transform t = overlaps[i].transform;
			Vector3 dir;
			float dist;
											//spherecol
			if (Physics.ComputePenetration(myCollider, transform.position, transform.rotation, overlaps[i], t.position, t.rotation, out dir, out dist))
			{
				Vector3 penetrationVector = dir * dist;
				Vector3 velocityProjected = Vector3.Project(velocity, -dir);
				transform.position = transform.position + penetrationVector;
				vel -= velocityProjected;
			}
		}
	}
	#endregion

	private float jumpHeight = 0;
	private bool inputJump = false;

	private float fallMultiplier = -1;

	#region Jumping
	private void Jump()
	{
		bool canJump = true;

		canJump = !Physics.Raycast(new Ray(transform.position, Vector3.up), playerHeight, discludePlayer);

		if (grounded && jumpHeight > 0.2f || jumpHeight <= 0.2f && grounded)
		{
			jumpHeight = 0;
			inputJump = false;
			fallMultiplier = -1;
		}

		if(grounded && canJump)
		{
			if(Input.GetKeyDown(KeyCode.Space))
			{
				inputJump = true;
				transform.position += Vector3.up * 0.5f;
				jumpHeight += jumpForce;
			}
		}
		else
		{
			if (!grounded)
			{
				jumpHeight -= (jumpHeight * jumpDecrease * Time.deltaTime) + fallMultiplier * Time.deltaTime;
				fallMultiplier += incrementJumpFallSpeed;
			}

		}

		velocity.y += jumpHeight;
	}
	#endregion


}
