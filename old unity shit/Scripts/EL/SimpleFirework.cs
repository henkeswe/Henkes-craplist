using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleFirework : MonoBehaviour {

	public float fuseTime = 0.5f;
	public float thrustTime = 2;
	public float force = 50f;
	public float delayTime = 1f;

	public int amountOfParticles = 1000;

	public Color fireworkColor = Color.red;
	private Rigidbody rb;
	private ParticleSystem part;

	private AudioSource audioS;

	public ParticleSystem fusePart;
	public ParticleSystem explosionPart;

	void Start()
	{
		rb = GetComponent<Rigidbody>();
		audioS = GetComponent<AudioSource>();
		SetUpExplosionColor();

		StartFuse();
	}

	void StartFuse()
	{
		StartCoroutine("Fuse");
	}

	void Explode()
	{
		explosionPart.Emit(1000);
		Debug.Log("boom!");
		audioS.Play();
	}

	void SetUpExplosionColor()
	{
		var col = explosionPart.colorOverLifetime;
		col.enabled = true;

		Gradient grad = new Gradient();
		grad.SetKeys(new GradientColorKey[] { new GradientColorKey(Color.yellow, 0.0f), new GradientColorKey(fireworkColor, 0.5f) }, new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 1.0f), new GradientAlphaKey(1.0f, 1.0f) });

		col.color = grad;
	}
	
	IEnumerator Fuse()
	{
		float fuse = 0;
		while(fuse < fuseTime)
		{
			fusePart.Emit(1);
			fuse += Time.deltaTime;
			yield return null;
		}

		float thrust = 0;
		while(thrust < thrustTime)
		{
			rb.AddForce(transform.up * force, ForceMode.Acceleration);
			fusePart.Emit(10);
			thrust += Time.deltaTime;
			yield return null;
		}

		float delay = 0;
		while (delay < delayTime)
		{
			yield return null;
		}

		Explode();
	}
	
}
