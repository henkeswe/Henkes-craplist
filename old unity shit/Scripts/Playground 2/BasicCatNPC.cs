using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicCatNPC : MonoBehaviour
{

	enum State { Idle, Walking, Running };

	State state;

	Animation anim;
	Rigidbody rb;
	public AnimationClip idleClip;
	public AnimationClip walkClip;
	public AnimationClip runClip;

	Vector3 lastPosition;

	Vector3 nextMovePos;

	float[] updateTimes = new float[2] { 1f, 5f };
	float nextTick;

	void Start()
    {
		nextTick = Time.time + 5f;
		anim = GetComponentInChildren<Animation>();
		rb = GetComponent<Rigidbody>();
    }

	void UpdateStates()
	{
		switch(state)
		{
			case State.Idle:
				Debug.Log("Idling...");

				if (anim.clip != idleClip)
				{
					anim.Stop();
					anim.clip = idleClip;
				}


				if (!anim.isPlaying)
					anim.Play();

				break;

			case State.Walking:
				Debug.Log("Walking..");

				//Rotate first
				if(transform.rotation != Quaternion.LookRotation(nextMovePos - transform.position))
				{
					rb.MoveRotation(Quaternion.RotateTowards(
						transform.rotation, 
						Quaternion.LookRotation(nextMovePos - transform.position), 
						1f
					));

					//Play the animation slower then usually
					anim[walkClip.name].speed = 0.2f;
					if (anim.clip != walkClip)
					{
						anim.Stop();
						anim.clip = walkClip;
					}

					if (!anim.isPlaying)
						anim.Play();
				}
				else
				{
					rb.MovePosition(Vector3.MoveTowards(
						transform.position, 
						nextMovePos, 
						0.01f
					));

					//Fix the animation again after rotating slowly
					anim[walkClip.name].speed = 1f;
					if (anim.clip != walkClip)
					{
						anim.Stop();
						anim.clip = walkClip;
						anim[walkClip.name].speed = 1f;
					}

					if (!anim.isPlaying)
						anim.Play();
				}

				break;

			case State.Running:
				Debug.Log("Running!");
				break;

			default:
				break;
		}
	}
	
	void UpdateBehaviour()
	{
		if (Vector3.Distance(transform.position, nextMovePos) <= 0.5f)
			state = State.Idle;

		if (Vector3.Distance(transform.position, nextMovePos) > 0.5f)
			state = State.Walking;

		//If she walks, let her walk
		if(state == State.Idle)
		{
			if (Time.time > nextTick)
			{
				nextMovePos = new Vector3(Random.Range(-10, 10), transform.position.y, Random.Range(-10f, 10f));
			}
		}
		else
		{
			//Keep refreshing the interval until she idles
			nextTick = Time.time + Random.Range(updateTimes[0], updateTimes[1]);
		}

	}

    void Update()
    {
		UpdateBehaviour();
		UpdateStates();


    }


}
