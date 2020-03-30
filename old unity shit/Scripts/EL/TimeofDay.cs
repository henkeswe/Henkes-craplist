using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeofDay : MonoBehaviour {

	Color dayAmbient = new Color(0.75f, 0.75f, 0.65f);
	Color nightAmbient = new Color(0.02f, 0.02f, 0.02f);

	Color currentAmbient;

	public Light lightSource;
	public float speedMultiplier = 24;

	public float nightMultiplier = 1;

	public float hours = 0;
	public float minutes = 0;
	private float seconds = 0;

	private float angle = 0;

	private float normalizedAngle;
	private float currentSpeed = 1;
	private bool night;


	void Start()
	{
	}

	void Update()
	{
		UpdateTime();
	}

	void UpdateTime()
	{
		//angle = (0.5f * (60f * hours + minutes)) / 2f;


		seconds += (Time.deltaTime * currentSpeed);

		angle = Mathf.Lerp(angle, (0.5f * (60f * hours + minutes)) / 2f, seconds / 60);
		normalizedAngle = angle / 360;

		if (night && currentSpeed != speedMultiplier * nightMultiplier)
		{
			currentSpeed = speedMultiplier * nightMultiplier;
		}
		else if(currentSpeed != speedMultiplier)
		{
			currentSpeed = speedMultiplier;
		}

		if (seconds >= 60)
		{
			seconds = 0;
			minutes += 1;
		}

		if (minutes >= 60)
		{
			minutes = 0;
			hours += 1;
		}

		if(hours >= 24)
		{
			hours = 0;
		}

		if(normalizedAngle >= 0f && normalizedAngle < 0.5f)
		{
			float lerp = normalizedAngle * 2f; //makes it 0 to 1 in time;
			currentAmbient = Color.Lerp(nightAmbient, dayAmbient, lerp);

			lightSource.intensity = Mathf.Lerp(0f, 1f, lerp);
		}

		if(normalizedAngle >= 0.5f)
		{
			float lerp = (normalizedAngle - 0.5f) * 2; //makes it 0 to 1 in time;
			currentAmbient = Color.Lerp(dayAmbient, nightAmbient, lerp);
			lightSource.intensity = Mathf.Lerp(1f, 0f, lerp);
		}

		if (normalizedAngle >= 0.25 && normalizedAngle < 0.75)
			night = false;
		else
			night = true;

		//if (hours < 6 || hours >= 18)
		//{
		//	currentAmbient = nightAmbient;
		//	lightSource.intensity = 0.05f;
		//}

		//if(hours >= 6 && hours < 18)
		//{
		//	currentAmbient = dayAmbient;
		//	lightSource.intensity = 1f;
		//}

		RenderSettings.ambientEquatorColor = currentAmbient;
		RenderSettings.ambientGroundColor = currentAmbient;
		RenderSettings.ambientLight = currentAmbient;



		lightSource.transform.localEulerAngles = new Vector3(-90 + angle, 0, 0);
	}

	void OnGUI()
	{
		GUI.Label(new Rect(10, 10, 250, 20), "Seconds : " + GetTime("seconds"));
		GUI.Label(new Rect(10, 20, 250, 20), "Minutes : " + GetTime("minutes"));
		GUI.Label(new Rect(10, 30, 250, 20), "Hours : " + GetTime("hours"));
		GUI.Label(new Rect(10, 40, 250, 20), "Angle ; " + angle);
	}

	public float GetTime(string name)
	{
		switch(name)
		{
			case "hours":
				return hours;
			case "minutes":
				return minutes;
			case "seconds":
				return seconds;
		}

		return 0;
	}
}


