using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatAnim : MonoBehaviour
{
	Rigidbody rb;
	public Animation anim;

	public AnimationClip idleClip;
	public AnimationClip slowWalkClip;
	public AnimationClip walkClip;
	public AnimationClip runClip;
	public AnimationClip jumpClip;

	float walkSpeed = 0.5f;
	float runSpeed = 3f;
	float jumpSpeed = 3f;

	bool isWalking;
	bool isRunning;
	bool jumped;
	bool hasJumped;

	Vector3 moveDir;

	void Start()
    {
		rb = GetComponent<Rigidbody>();
    }

	void KeyCheck()
	{
		if (Input.GetAxis("Vertical") > 0)
		{
			if(Input.GetKey(KeyCode.LeftShift))
			{
				isRunning = true;
			}
			else
			{
				isRunning = false;
			}

			isWalking = true;
		}
		else
		{
			isWalking = false;
		}

		if(Input.GetKeyDown(KeyCode.Space) && Grounded())
		{
			jumped = true;
		}

	}

	bool Grounded()
	{

		RaycastHit hit;

		if (Physics.Raycast(transform.position + new Vector3(0,0.1f,0), Vector3.down, out hit, 0.2f))
		{
			return true;

		}
		
		return false;
	}

    void Update()
    {

		KeyCheck();
		Debug.Log(Grounded());

		if (isWalking)
		{
			float speed = walkSpeed;
			if (isRunning)
			{
				Debug.Log("Running");
				speed = runSpeed;

				if (anim.clip != runClip)
				{
					anim.Stop();
					anim.clip = runClip;
				}

				if (!anim.isPlaying)
				{
					anim.Play();
				}

			}
			else
			{
				Debug.Log("Walking");


				if (anim.clip != walkClip)
				{
					anim.Stop();
					anim.clip = walkClip;

				}

				if (!anim.isPlaying)
				{
					anim.Play();
				}

			}

			moveDir = transform.forward * speed;
		}
		else
		{
			if (anim.clip == walkClip || anim.clip == runClip)
			{
				anim.Stop();
			}
		}

		if (jumped && Grounded())
		{
			if(!hasJumped)
			{
				rb.AddForce(transform.up * jumpSpeed, ForceMode.VelocityChange);
				hasJumped = true;

				if (anim.clip != jumpClip)
				{
					anim.Stop();
					anim.clip = jumpClip;
					anim.Play();
				}
			}


			Debug.Log("Jumping");
		}
		
		if (Grounded())
		{
			hasJumped = false;
			jumped = false;
		}

		rb.velocity = new Vector3(moveDir.x, rb.velocity.y, moveDir.z);
		moveDir = new Vector3(0, 0, 0);

		if(!anim.isPlaying && !isWalking && !isRunning && !jumped)
		{
			anim.clip = idleClip;
			anim.Play();
		}
	}
}
