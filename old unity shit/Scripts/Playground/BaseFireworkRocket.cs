using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseFireworkRocket : MonoBehaviour
{
	public float fuseTime = 1f;
	public float flightTime = 2f;
	public float soundDelay = 0.2f;
	private readonly float flightSpeed = 100f;

	public Rigidbody rBody;
	public AudioSource aSource;

	public ParticleSystem explosionParticlesPrefab;
	public ParticleSystem fuseParticlePrefab;

	private ParticleSystem explosionParticles;
	private ParticleSystem fuseParticles;

	private void Start()
	{
		if (explosionParticlesPrefab != null)
			explosionParticles = Instantiate(explosionParticlesPrefab, transform);

		if (fuseParticlePrefab != null)
			fuseParticles = Instantiate(fuseParticlePrefab, transform);
	}

	public void LightFulse(float fuseTime)
	{
		StartCoroutine(StartFuse(fuseTime));
	}

	private IEnumerator StartFuse(float fuseTime)
	{
		if(fuseParticles == null)
		{
			Debug.LogWarning("Fuse Particles not found!");
		}
		else
		{
			fuseParticles.Play();
		}

		yield return new WaitForSeconds(fuseTime);

		if (fuseParticles != null)
			fuseParticles.Pause();

		flightTime = Time.time + flightTime;

		if (rBody == null)
		{
			Debug.LogError("Failed to \"StartCoroutine(StartRocket(flightTime))\" because Rigidbody not found!!");
		}
		else
		{
			StartCoroutine(StartRocket(flightTime));
		}
		
		StopCoroutine(StartFuse(fuseTime));
	}

	private IEnumerator StartRocket(float flightTime)
	{
		while (Time.time <= flightTime)
		{
			rBody.AddForce(transform.up * flightSpeed, ForceMode.Acceleration);
			yield return null;
		}

		Explode();

		yield return new WaitForSeconds(soundDelay);

		if (aSource == null)
		{
			Debug.LogWarning("AudioSource not found!");
		}
		else
		{
			aSource.pitch = Random.Range(0.5f, 1.25f);
			aSource.Play();
		}

		StopCoroutine(StartRocket(flightTime));
	}

	Color red		= new Color(255f, 0f, 0f);
	Color green		= new Color(0, 255f, 0f);
	Color lightBlue = new Color(0f, 230f, 255f);

	Color yellow	= new Color(255f, 145f, 0f);
	Color purple	= new Color(255f, 0f, 255f);
	
	

	private void Explode()
	{
		if (explosionParticles == null)
		{
			Debug.LogWarning("Explosion Particles not found!");
			return;
		}

		Color[] colors = new Color[] { lightBlue, yellow, purple, green, red };

		Color expColor;

		expColor = colors[Random.Range(0, colors.Length)];

		Gradient grad = new Gradient();
		grad.SetKeys(new GradientColorKey[] { new GradientColorKey(yellow, 0.0f), new GradientColorKey(expColor, 0.5f) }, new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) });

		var col = explosionParticles.colorOverLifetime;
		col.color = grad;

		explosionParticles.Play();
	}
}
