using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntyNPC : MonoBehaviour
{
	enum State {
		Idle,
		Falling,
		Walking,
		Chase,
		LookAround
	};

	State state = State.Idle;

	Animation anim;
	CharacterController con;

	GameObject chaseTarget;

	public AnimationClip walkClip;
	public GameObject model;

	float walkSpeed = 0.5f;
	float downForce = 0.1f;

	Vector3 nextMovePos;
	Vector3 lastPosition;
	Quaternion lastRotation;

	float nextMoveTick;
	readonly float[] nextMoveTime = new float[2]{2, 3};

    void Start()
	{
		Initialize();
	}

	private void Initialize()
	{
		Random.InitState(System.DateTime.Now.Millisecond);

		state = State.Idle;
		nextMovePos = GetTransformTopPos();
		nextMoveTick = Time.time + Random.Range(nextMoveTime[0], nextMoveTime[1]);

		anim = GetComponentInChildren<Animation>();
		con = GetComponent<CharacterController>();

		//ant only has 1 clip
		anim.clip = walkClip;
		anim[walkClip.name].speed = 2f;
		

	}

	Vector3 GetTransformTopPos()
	{
		return new Vector3(transform.position.x, 0, transform.position.z);
	}

	Vector3 GetTargetTransformTopPos()
	{
		return new Vector3(chaseTarget.transform.position.x, 0, chaseTarget.transform.position.z);
	}

	void UpdateStates()
	{
		switch (state)
		{
			case State.Idle:
				Debug.Log("Idling...");
				anim.Stop();
				con.Move(Vector3.down * downForce);
			break;

			case State.Falling:
				Debug.Log("Falling!");
				con.Move(Vector3.down * downForce);

				if (transform.position == lastPosition && lastRotation == transform.rotation)
				{
					OnStuck();
				}

				lastPosition = transform.position;
				lastRotation = transform.rotation;
			break;

			case State.Walking:
				Debug.Log("Walking..");

				if (anim[walkClip.name].speed != 2)
				{
					anim[walkClip.name].speed = 2;
				}

				transform.rotation = Quaternion.RotateTowards(
					transform.rotation,
					Quaternion.LookRotation(nextMovePos - GetTransformTopPos()),
					1f
				);

				con.Move((transform.forward * Time.deltaTime * walkSpeed) + Vector3.down * downForce);

				if (transform.position == lastPosition && lastRotation == transform.rotation)
				{
					OnStuck();
				}

				lastPosition = transform.position;
				lastRotation = transform.rotation;

				if (!anim.isPlaying)
					anim.Play();
			break;

			case State.Chase:
				Debug.Log("The ant is chasing " + chaseTarget);

				if (anim[walkClip.name].speed != 3)
				{
					anim[walkClip.name].speed = 3;
				}

				transform.rotation = Quaternion.RotateTowards(
					transform.rotation,
					Quaternion.LookRotation(GetTargetTransformTopPos() - GetTransformTopPos()),
					1f
				);

				con.Move((transform.forward * Time.deltaTime * walkSpeed * 2) + Vector3.down * downForce);

				if (transform.position == lastPosition && lastRotation == transform.rotation)
				{
					OnStuck();
				}

				lastPosition = transform.position;
				lastRotation = transform.rotation;

				if (!anim.isPlaying)
					anim.Play();
			break;

			default:
			break;
		}
	}

	void AntBrain()
	{
		if (!con.isGrounded)
		{
			state = State.Falling;
		}
		else
		{
			if(HasTarget())
			{
				state = State.Chase;
			}
			else
			{
				if (Vector3.Distance(GetTransformTopPos(), nextMovePos) <= 0.1f)
					state = State.Idle;

				if (Vector3.Distance(GetTransformTopPos(), nextMovePos) > 0.1f)
					state = State.Walking;
			}
		}

		//If she walks, let her walk
		if (state == State.Idle)
		{
			if (Time.time > nextMoveTick)
			{
				nextMovePos = new Vector3(
					Random.Range(-10, 10), 
					0, 
					Random.Range(-10f, 10f)
				);
			}
		}
		else if(state == State.Walking)
		{
			//Keep refreshing the interval until she idles
			nextMoveTick = Time.time + Random.Range(nextMoveTime[0], nextMoveTime[1]);
		}

	}

    void Update()
    {
		SearchForTargets();
		AntBrain();
		UpdateStates();
	}

	bool HasTarget()
	{
		return chaseTarget != null;
	}

	void SearchForTargets()
	{
		//change this, 
		//temp: keep nulling to make ant not care if target is out of range.
		chaseTarget = null;

		//distToClosest
		float closestTargetDist = Mathf.Infinity;

		Collider[] hitColliders = Physics.OverlapSphere(transform.position, 5f);
		int i = 0;
		while (i < hitColliders.Length)
		{
			string name = hitColliders[i].name;
			if (name != transform.name && !name.Contains("Plane") && !name.Contains("Anty") && !name.Contains("MainGround"))
			{
				if (Vector3.Distance(transform.position, hitColliders[i].transform.position) < closestTargetDist)
				{
					closestTargetDist = Vector3.Distance(transform.position, hitColliders[i].transform.position);
					chaseTarget = hitColliders[i].gameObject;
				}

				Debug.Log(name);
			}

			i++;
		}

	}

	void OnStuck()
	{
		Debug.Log("Is anty stuck?");
		nextMovePos = GetTransformTopPos();
	}

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
