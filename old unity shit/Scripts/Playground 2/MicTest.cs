using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MicTest : MonoBehaviour
{

    public AudioSource source;
	string micDevice;

	// Use this for initialization
	void Start()
	{
		micDevice = Microphone.devices[1];
		source.clip = Microphone.Start(micDevice, true, 10, 44100 / 2); //Devide by 2 to lower latency
		source.clip.name = "Unknown";
		source.loop = true;

		while(!(Microphone.GetPosition(micDevice) > 0)) { }
		source.Play();

	}


}
