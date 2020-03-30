using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsGroundHit : MonoBehaviour
{

	public AudioClip soundClip;

	private new AudioSource audio;
    // Start is called before the first frame update
    void Start()
    {
        if(soundClip != null)
		{
			audio = this.gameObject.AddComponent<AudioSource>();
			audio.clip = soundClip;
			audio.loop = false;

		}
    }

	private void OnCollisionEnter(Collision collision)
	{
		audio.volume = 0.5f;
		audio.pitch = Random.Range(0.65f, 1f);
		audio.Play();
	}
}
