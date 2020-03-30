using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCTest : MonoBehaviour
{
	enum State { Walk, Idle, Run };
	CharacterController c;
	State state;
    void Start()
    {
		state = State.Idle;
		c = gameObject.AddComponent<CharacterController>();
		c.radius = 0.2f;
		c.height = 0.2f;

		//clear away warning
		if (state == State.Idle)
		{

		}
    }

	void UpdateStates()
	{
		foreach(State s in Enum.GetValues(typeof(State)))
		{
			OnState(s);
		}
	}

	void OnState(State s)
	{
		Debug.Log("State!" + s.ToString());
	}

    void Update()
    {
		UpdateStates();
	}
}
