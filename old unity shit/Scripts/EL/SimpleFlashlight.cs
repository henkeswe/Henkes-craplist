using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleFlashlight : MonoBehaviour {
	Camera cam;
	GameObject lightGameObject;
	Light spotLight;
	void Start ()
	{
		cam = GetComponent<Camera>();
		lightGameObject = new GameObject();
		lightGameObject.SetActive(false);
		lightGameObject.transform.position = cam.transform.position;
		lightGameObject.transform.parent = cam.transform;

		spotLight = lightGameObject.AddComponent<Light>();
		spotLight.type = LightType.Spot;
		spotLight.spotAngle = 45f;
		spotLight.range = 500f;
	}

	void Update ()
	{
		if(Input.GetKeyDown(KeyCode.F))
		{
			lightGameObject.SetActive(!lightGameObject.activeSelf);
		}
	}
}
