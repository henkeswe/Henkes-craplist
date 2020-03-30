using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElephantNPC : BaseNPC
{

	public Animation anim;
	public AnimationClip walkClip;
	public AnimationClip idleClip;
	//public AnimationClip idleClip;

	public override void Start()
	{
		base.Start();

		useRandomMovePos = true;

		walkSpeed = 0.2f;
		runSpeed = 0.2f;

		idleClip.wrapMode = WrapMode.Loop;
		anim[walkClip.name].speed = 1.2f;
		walkClip.wrapMode = WrapMode.Loop;
		anim.clip = walkClip;
		anim.Play();
		//anim[idleClip.name].speed = 1f;
	}
	
	private void Update()
	{
		
	}

	public override void FixedUpdate()
	{
		base.FixedUpdate();
	}
}
