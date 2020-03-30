using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseNPC_old : MonoBehaviour
{
	enum State {
		Idle,
		Wander,
		Chase,
		Flee,
		Falling
	}

	//float health = 50f;		//clear away warning

	State state;
	CharacterController cc;
	Animation anim;

	//Pos to go to
	Vector3 nextMovePos;

	public GameObject model;

	//Is this NPC friendly?
	public bool friendly = true;

	//Times between wander idles
	public float[] wanderUpdateInterval = new float[] { 2, 5 };
	private float nextWanderTick;

	//Animation
	public AnimationClip idleClip;
	public AnimationClip walkClip;
	public float walkClipSpeed = 2f;
	public AnimationClip runClip;
	public float runClipSpeed = 2f;

	//Movement
	public float walkSpeed = 1f;
	public float runSpeed = 2f;
	readonly float downForce = 0.1f;

	//Checking enviroment, like chase targets
	public float senseRadius;

	void Start()
    {
		Initialize();
	}

	void Update()
	{
		UpdateStates();
		HandleStates();
	}

	private void Initialize()
	{


		Random.InitState(System.DateTime.Now.Millisecond);

		nextMovePos = GetTransformTopPos();
		nextWanderTick = Time.time + Random.Range(wanderUpdateInterval[0], wanderUpdateInterval[1]);

		cc = GetComponent<CharacterController>();
		anim = GetComponentInChildren<Animation>();
		state = State.Idle;

		if (!cc)
			Debug.LogError("No 'CharacterController Component' found!");

		if (!anim)
			Debug.LogError("No 'Animation Component' found!");
		else
			anim.Stop();
	}

	private void UpdateStates()
	{
		if (!cc.isGrounded)
		{
			state = State.Falling;
		}
		else
		{
			if (Vector3.Distance(GetTransformTopPos(), nextMovePos) > 0.1f)
				state = State.Wander;

			if (Vector3.Distance(GetTransformTopPos(), nextMovePos) <= 0.1f)
				state = State.Idle;
		}

		if(state == State.Idle)
		{
			if (Time.time > nextWanderTick)
			{
				nextMovePos = new Vector3(
					Random.Range(-10, 10),
					0,
					Random.Range(-10f, 10f)
				);
			}
		}
		else if(state == State.Wander)
		{
			nextWanderTick = Time.time + Random.Range(wanderUpdateInterval[0], wanderUpdateInterval[1]);
		}
	}

	private void HandleStates()
	{
		switch(state)
		{
			case State.Idle:
				Debug.Log("Idle..");
				LoopClip(idleClip);

				cc.Move(Vector3.down * downForce);

				break;
			case State.Falling:
				Debug.Log("Falling!");
				cc.Move(Vector3.down * downForce);

				break;
			case State.Wander:
				Debug.Log("Wandering..");
				transform.rotation = Quaternion.RotateTowards(
					transform.rotation,
					Quaternion.LookRotation(nextMovePos - GetTransformTopPos()),
					1f
				);

				cc.Move((transform.forward * Time.deltaTime * walkSpeed) + Vector3.down * downForce);

				LoopClip(walkClip, walkClipSpeed);

				break;
			default:
				Debug.LogWarning("Unsupported state: " + state);
				break;
		}
	}

	void LoopClip(AnimationClip clip, float speed = 1f)
	{
		if (clip != null)
		{
			if (anim.clip != clip)
			{
				anim.clip = clip;
				anim.Stop();
			}

			if (!anim.isPlaying)
				anim.Play();

			if (anim[clip.name].speed != speed)
				anim[clip.name].speed = speed;
		}
		else
		{
			anim.Stop();
		}
	}

	Vector3 GetTransformTopPos()
	{
		return new Vector3(transform.position.x, 0, transform.position.z);
	}

	//Vector3 GetTargetTransformTopPos()
	//{
	//	return new Vector3(chaseTarget.transform.position.x, 0, chaseTarget.transform.position.z);
	//}
	private void OnControllerColliderHit(ControllerColliderHit hit)
	{
		//Thanks bigmisterb
		//https://forum.unity.com/threads/setting-rotation-from-hit-normal.166587/

		//Works so far
		//Update model rotation after surface
		model.transform.rotation = Quaternion.LookRotation(Vector3.Cross(-hit.normal, transform.right), hit.normal);
		//Debug.DrawRay(hit.point, hit.normal, Color.green, 2f);
	}
}
