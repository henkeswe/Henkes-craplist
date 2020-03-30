using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireworkBattery : MonoBehaviour
{

	public ParticleSystem explosionParticles;

	private float dist = 0.1f;
	private float scale = 0.01f;

	private float yModifier = 0.1f;
	private void Start()
	{
		for(int i = 0; i < 10; i++)
		{
			for (int j = 0; j < 10; j++)
			{
				GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
				go.transform.localScale = new Vector3(scale, scale, scale);
				go.transform.position = transform.position + new Vector3(-0.5f + (dist / 2f), 0, -0.5f - (dist / 2f)) + new Vector3(i * dist, yModifier, (j * dist) + dist);

				Rigidbody rb = go.gameObject.AddComponent<Rigidbody>();

				Transform expParent = new GameObject().transform;
				expParent.transform.parent = this.transform;
				expParent.localScale = new Vector3(10, 10, 10);

				BaseFireworkRocket r = expParent.gameObject.AddComponent<BaseFireworkRocket>();
				r.explosionParticlesPrefab = explosionParticles;
				r.rBody = rb;

				r.LightFulse(i * j);

			}


			
		}
	}

	private void Update()
	{
		
	}

}
