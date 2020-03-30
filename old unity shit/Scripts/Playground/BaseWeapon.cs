using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseWeapon : MonoBehaviour
{
	public BaseWeapon()
	{
	}

	public bool equipped = true;

	//for getting aimpos, brok
	//public Camera viewPort;

	//Keys for firing
	public KeyCode primaryFireButton	= KeyCode.Mouse0;
	public KeyCode secondaryFireButton	= KeyCode.Mouse1;
	public KeyCode tertiaryFireButton	= KeyCode.Mouse2;

	//Is it automatic or not
	public bool primaryAutomatic	= true;
	public bool secondaryAutomatic	= true;
	public bool tertiaryAutomatic	= true;

	//The delay between firing
	public float primaryFireDelay	= 0f;
	public float secondaryFireDelay	= 0f;
	public float tertiaryFireDelay	= 0f;

	//next time it will fire
	private float nextPrimaryFire;
	private float nextSecondaryFire;
	private float nextTertiaryFire;

	public void Start()
    {
		/*
		nextPrimaryFire		= Time.time + primaryFireDelay;
		nextSecondaryFire	= Time.time + secondaryFireDelay;
		nextTertiaryFire	= Time.time + tertiaryFireDelay;
		*/
	}

	public void Update()
    {
		if (!equipped)
			return;

		if(primaryAutomatic)
		{
			if(Input.GetKey(primaryFireButton) && Time.time >= nextPrimaryFire)
			{
				OnPrimaryWeaponFire();
				nextPrimaryFire = Time.time + primaryFireDelay;
			}
		}
		else
		{
			if(Input.GetKeyDown(primaryFireButton))
			{
				OnPrimaryWeaponFire();
			}
		}

		if (secondaryAutomatic)
		{
			if (Input.GetKey(secondaryFireButton) && Time.time >= nextSecondaryFire)
			{
				OnSecondaryWeaponFire();
				nextSecondaryFire = Time.time + secondaryFireDelay;
			}
		}
		else
		{
			if (Input.GetKeyDown(secondaryFireButton))
			{
				OnSecondaryWeaponFire();
			}
		}

		if (tertiaryAutomatic)
		{
			if (Input.GetKey(tertiaryFireButton) && Time.time >= nextTertiaryFire)
			{
				OnTertiaryWeaponFire();
				nextTertiaryFire = Time.time + tertiaryFireDelay;
			}
		}
		else
		{
			if (Input.GetKeyDown(tertiaryFireButton))
			{
				OnTertiaryWeaponFire();
			}
		}
	}

	public virtual void OnPrimaryWeaponFire()
	{
		Debug.Log("OnPrimaryWeaponFire");
	}

	public virtual void OnSecondaryWeaponFire()
	{
		Debug.Log("OnSecondaryWeaponFire");
	}

	public virtual void OnTertiaryWeaponFire()
	{
		Debug.Log("OnTertiaryWeaponFire");
	}

	/*
	public Vector3 GetAimPos()
	{
		return viewPort.transform.forward;
	}
	*/
}
