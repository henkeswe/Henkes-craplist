using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
	public AudioClip laserBuzz;
	public GameObject laserHead;
	public Material laserMaterial;

	private LineRenderer laser;
	private AudioSource source;

	//private Vector3 direction;
	private bool laserEnabled = true;
	private float laserWidth = 0.2f;
	private Color laserColor = Color.green;
	private float laserMaxDistance = 100f;
	private bool laserUseCollision = true;
	private bool laserPushRigidbody = true;
	private bool laserDamaged = false;

	private float laserAlpha = 1f;

	private float nextDamage;

	float randomDamageSpeed = 1f;

	// Start is called before the first frame update
	void Start()
    {
		nextDamage = Time.time + Random.Range(1f,2f);

		source = gameObject.AddComponent<AudioSource>();
		source.playOnAwake = false;
		source.loop = true;

		laser = gameObject.AddComponent<LineRenderer>();
		laser.material = laserMaterial;
		laser.material.color = Color.magenta;
		laser.startWidth = laserWidth;
		laser.endWidth = laserWidth;
		laser.enabled = laserEnabled;
		laser.positionCount = 10;
	}

    // Update is called once per frame
    void Update()
    {
		Color defColor = laser.material.color;

		if (laserDamaged)
		{
			if (Time.time >= nextDamage)
			{
				randomDamageSpeed = Random.Range(-0.25f, 0.75f);
				nextDamage = Time.time + Random.Range(1f, 2f + randomDamageSpeed);
			}

			source.pitch = randomDamageSpeed;
			
			laserHead.transform.Rotate(transform.up, randomDamageSpeed * 10f);

			if(randomDamageSpeed > 0.4f)
			{
				laserEnabled = true;
			}
			else
			{
				laserEnabled = false;
			}
		}
		else
		{
			if (source.pitch != 1)
				source.pitch = 1f;

			laserHead.transform.Rotate(transform.up, 5f);
		}

		if (!laser)
			return;

        if(laserEnabled)
		{
			if(!laser.enabled)
			{
				laser.enabled = true;
			}

			if(!source.isPlaying)
			{
				if(source.clip != laserBuzz)
					source.clip = laserBuzz;

				source.Play();
			}

			if(source.isPlaying)
			{
				if (source.volume < 1)
					source.volume += 0.05f;

				if (source.volume >= 1)
					source.volume = 1f;

				laserAlpha = Mathf.Clamp(source.pitch, 0.1f, 1f);

				laser.material.color = new Color(defColor.r, defColor.g, defColor.b, laserAlpha);
			}

			RaycastHit hit;

			if(laserUseCollision && Physics.Raycast(transform.position, transform.forward * laserMaxDistance, out hit))
			{

				if(laserPushRigidbody)
				{
					Rigidbody hitRB = hit.transform.GetComponent<Rigidbody>();

					if (hitRB)
					{
						//brok..?
						hitRB.AddForce(-hit.normal, ForceMode.Impulse);
					}
				}

				laser.positionCount = 2;
				laser.SetPosition(0, transform.position + transform.forward * 0.1f);
				laser.SetPosition(1, transform.position + (transform.forward * hit.distance));

				/*
				for(int i = 2; i < laser.positionCount; i = i + 2)
				{
					if (Physics.Raycast(laser.GetPosition(i-1), Vector3.Reflect(hit.point, hit.normal), out hit))
					{
						laser.SetPosition(i, laser.GetPosition(i-1));
						laser.SetPosition(i+1, hit.point);
					}
					else
					{
						laser.SetPosition(i, laser.GetPosition(i-1) + laser.GetPosition(i-2));
					}
				}
				*/

				/*
				bool temp = false;

				if(temp)
				{
					//Reflect
					laser.SetPosition(0, transform.position + transform.forward * 0.1f);


					laser.SetPosition(1, transform.position + (transform.forward * hit.distance));

					List<RaycastHit> a = new List<RaycastHit>(laser.positionCount);
					RaycastHit curHit = hit;

					Vector3 pos = Vector3.Reflect(curHit.point, curHit.normal);
					for (int i = 1; i < laser.positionCount - 1; i++)
					{
						//RaycastHit curHit;
						
						if (Physics.Raycast(laser.GetPosition(i), pos, out curHit))
						{
							pos = Vector3.Reflect(curHit.point, curHit.normal);
							laser.SetPosition(i, pos);
						}
						else
						{
							laser.SetPosition(i, laser.GetPosition(i));
						}

						a.Add(curHit);
					}
				}
				else
				{
					laser.SetPosition(0, transform.position + transform.forward * 0.1f);
					laser.SetPosition(1, transform.position + (transform.forward * hit.distance));
				}

				*/
			}
			else
			{
				laser.SetPosition(0, transform.position + transform.forward * 0.1f);
				laser.SetPosition(1, transform.position + (transform.forward * laserMaxDistance));
			}
		}
		else
		{


			if (source.volume > 0)
				source.volume -= 0.01f;



			if (source.isPlaying && source.volume <= 0)
			{
				source.volume = 0f;
				source.Stop();

				if(laser.enabled)
					laser.enabled = false;
			}

			laserAlpha = source.volume;
			laser.material.color = new Color(defColor.r, defColor.g, defColor.b, laserAlpha);
		}
    }
}
