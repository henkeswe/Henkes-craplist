using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponTest : BaseWeapon
{
	public WeaponTest()
	{
		primaryAutomatic = true;
		secondaryAutomatic = false;
		tertiaryAutomatic = false;

		primaryFireDelay = 1f;
	}

	public override void OnPrimaryWeaponFire()
	{
		/*
		GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		go.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
		go.transform.position = GetAimPos();
		Rigidbody rb = go.AddComponent<Rigidbody>();
		rb.AddForce(GetAimPos(), ForceMode.Impulse);
		*/

		Debug.Log("MEOW!");
	}

	public override void OnSecondaryWeaponFire()
	{
		Debug.Log("MEOW2!");
	}

	public override void OnTertiaryWeaponFire()
	{
		Debug.Log("MEOW3!");

	}
}
