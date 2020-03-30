using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Weather { Normal, Rain, Cloud, Thunder };
public enum PartOfDay { Day, Night, Morning, Evening };

public class Climate : MonoBehaviour
{

	#region Day&Night cycle
	private PartOfDay pod;
	private TimeOfDay tod = new TimeOfDay(12, 0);

	private int timeMultiplier = 3600; //3600 ~1h/sec

	public TimeOfDay GetTimeOfDay()
	{
		return tod;
	}

	public PartOfDay GetPartOfDay()
	{
		return pod;
	}

	private void UpdateTime()
	{
		//Rough timer, not accurate, but fine
		tod.floatySeconds += Time.deltaTime * timeMultiplier;

		tod.Second = (int)tod.floatySeconds;

		if(tod.Second >= 60)
		{
			tod.floatySeconds = 0;
			tod.Second = 0;
			tod.Minute += 1;
		}

		if(tod.Minute >= 60)
		{
			tod.Minute = 0;
			tod.Hour += 1;
		}

		if(tod.Hour >= 24)
		{
			tod.Hour = 0;
			tod.Day += 1;
		}


		if (tod.Hour >= 6 && tod.Hour < 12)
		{
			pod = PartOfDay.Morning;
		}
		else if (tod.Hour >= 12 && tod.Hour < 18)
		{
			pod = PartOfDay.Day;
		}
		else if (tod.Hour >= 18 && tod.Hour < 22)
		{
			pod = PartOfDay.Evening;
		}
		else
		{
			pod = PartOfDay.Night;
		}
	}
	#endregion

	#region WeatherCycle

	private WeatherInfo wi = new WeatherInfo();

	float nextWeatherUpdate;

	public WeatherInfo GetWeatherInfo()
	{
		return wi;
	}

	private void UpdateWeather()
	{
		//set update the next update if its 0 (not using it in start)
		if(nextWeatherUpdate == 0)
		{
			nextWeatherUpdate = Time.time + wi.updateInterval;
		}

		if(Time.time >= nextWeatherUpdate)
		{
			float a = UnityEngine.Random.Range(0, 100);

			foreach (Weather w in wi.weatherChances.Keys)
			{
				if (a <= wi.weatherChances[w])
				{
					wi.currentWeather = w;

					//set temp by weather temp + random margin
					wi.temperature = wi.weatherTemperatures[w] + UnityEngine.Random.Range(-5, 5);
				}
			}

			nextWeatherUpdate = Time.time + wi.updateInterval;
		}
	}
	#endregion

	void Start()
	{

	}

	void Update()
    {
		UpdateWeather();
		UpdateTime();
		Debug.Log(tod + "  " + pod + "  " + wi.currentWeather);
	}
}


//idk
public class WeatherInfo
{
	public WeatherInfo()
	{
		this.updateInterval = 5;
		this.windSpeed = 50f;
		this.temperature = 20f;

		this.currentWeather = Weather.Normal;

	}
	
	public int updateInterval;
	public float windSpeed;
	public float temperature;

	public Weather currentWeather;

	public Dictionary<Weather, int> weatherChances = new Dictionary<Weather, int>
	{
		{ Weather.Normal, 75 },
		{ Weather.Rain, 40 },
		{ Weather.Cloud, 30 },
		{ Weather.Thunder, 5 }
	};

	public Dictionary<Weather, float> weatherTemperatures = new Dictionary<Weather, float>
	{
		{ Weather.Normal, 22f },
		{ Weather.Cloud, 14f },
		{ Weather.Rain, 10f },
		{ Weather.Thunder, 5f },
	};
}


public class TimeOfDay
{
	public TimeOfDay()
	{
		this.Hour = 0;
		this.Minute = 0;
		this.Second = 0;
	}

	public TimeOfDay(int h = 0, int m = 0)
	{
		this.Hour = h;
		this.Minute = m;
	}

	public TimeOfDay(int h = 0, int m = 0, int s = 0)
	{
		this.Hour = h;
		this.Minute = m;
		this.Second = s;
	}

	public TimeOfDay(int h = 0, int m = 0, int s = 0, int d = 0)
	{
		this.Day = d;
		this.Hour = h;
		this.Minute = m;
		this.Second = s;
	}

	public float floatySeconds { get; set; }
	public int Day { get; set; }
	public int Hour { get; set; }
	public int Minute { get; set; }
	public int Second { get; set; }

	public override string ToString()
	{
		return String.Format(
			"Day: {00:00} - Time: {1:00}:{2:00}:{3:00}",
			this.Day, this.Hour, this.Minute, this.Second);
	}
}