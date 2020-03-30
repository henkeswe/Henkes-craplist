using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CSCore;
using CSCore.SoundIn;
using CSCore.Streams;

public class WaveSpectrum : MonoBehaviour
{
	private List<Transform> objects1 = new List<Transform>();
	private List<Transform> objects2 = new List<Transform>();
	private List<float> blocks1 = new List<float>();
	private List<float> blocks2 = new List<float>();

	int amnVisuals = 100;
	int visualMultiplier = 10;

	private WasapiLoopbackCapture loopbackCapture;
	private SoundInSource soundInSource;
	private IWaveSource realTimeSource;
	private SingleBlockNotificationStream singleBlockNotificationStream;

	void Start()
    {
		SetupObjects();
		StartListen();
	}

    void Update()
    {
		UpdateObjects();
	}

	void UpdateObjects()
	{
		for(int i = 0; i < blocks1.Count; i++)
		{
			float scaleY = blocks1[i] * visualMultiplier;

			Vector3 objectPos = objects1[i].localPosition;
			Vector3 objectScale = objects1[i].localScale;

			float newScale = objectScale.y -= Time.deltaTime * 25f;
			
			if (scaleY > objectScale.y)
				newScale = scaleY;

			if (newScale < 0.25f)
				newScale = 0.25f;

			objects1[i].localPosition = new Vector3(objectPos.x, newScale / 2f, objectPos.z);
			objects1[i].localScale = new Vector3(objectScale.x, newScale, objectScale.z);
		}

		for (int i = 0; i < blocks2.Count; i++)
		{
			//float margin = 0.25f;
			float scaleY = blocks2[i] * visualMultiplier;

			Vector3 objectPos = objects2[i].localPosition;
			Vector3 objectScale = objects2[i].localScale;

			float newScale = objectScale.y += Time.deltaTime * 25f;

			if (scaleY < objectScale.y)
				newScale = scaleY;

			if (newScale > 0.25f)
				newScale = 0.25f;

			objects2[i].localPosition = new Vector3(objectPos.x, newScale / 2f, objectPos.z);
			objects2[i].localScale = new Vector3(objectScale.x, newScale, objectScale.z);
		}
	}

	void SetupObjects()
	{
		float objScale = 0.25f;
		float margin = 0.5f;
		Vector3 startPos = transform.position +  (Vector3.left * (amnVisuals / 2f) * margin) + (Vector3.right * margin / 2f);

		Transform container1 = new GameObject().transform;
		Transform container2 = new GameObject().transform;
		container1.parent = transform;
		container2.parent = transform;
		container1.name = "Upper Blocks";
		container2.name = "Lower Block";

		for (int i = 0; i < amnVisuals; i++)
		{
			Transform go = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
			go.parent = container1;
			go.name = "Visual [" + i + "]";
			Destroy(go.GetComponent<Collider>());
			go.localScale = new Vector3(objScale, objScale, objScale);
			go.position = startPos + (Vector3.right * margin * i) + Vector3.up * margin;

			objects1.Add(go);
		}

		for (int i = 0; i < amnVisuals; i++)
		{
			Transform go = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
			go.parent = container2;
			go.name = "Visual [" + i + "]";
			Destroy(go.GetComponent<Collider>());
			go.localScale = new Vector3(objScale, objScale, objScale);
			go.position = startPos + (Vector3.right * margin * i) + Vector3.down * margin;

			objects2.Add(go);

		}

	}

	void StartListen()
	{
		loopbackCapture = new WasapiLoopbackCapture();
		loopbackCapture.Initialize();

		soundInSource = new SoundInSource(loopbackCapture);

		loopbackCapture.Start();

		singleBlockNotificationStream = new SingleBlockNotificationStream(soundInSource.ToSampleSource());
		realTimeSource = singleBlockNotificationStream.ToWaveSource();

		soundInSource.DataAvailable += DataAvailable;

		singleBlockNotificationStream.SingleBlockRead += SingleBlockNotificationStream_SingleBlockRead;
	}

	void StopListen()
	{
		singleBlockNotificationStream.SingleBlockRead -= SingleBlockNotificationStream_SingleBlockRead;

		soundInSource.Dispose();
		realTimeSource.Dispose();
		loopbackCapture.Stop();
		loopbackCapture.Dispose();
	}

	void DataAvailable(object o, DataAvailableEventArgs data)
	{
		//byte[] buffer = new byte[realTimeSource.WaveFormat.BytesPerSecond / 2];

		//while (realTimeSource.Read(buffer, 0, buffer.Length) > 0)
		//{
		//}

		//byte[] buffer = data.Data; //seems like buffer is something like the hz constant value, like 48000 works, low value no input, high value lags
		byte[] buffer = new byte[realTimeSource.WaveFormat.BytesPerSecond];
		realTimeSource.Read(buffer, 0, buffer.Length);
	}


	//float n = 0;
	//float a;
	void SingleBlockNotificationStream_SingleBlockRead(object sender, SingleBlockReadEventArgs e)
	{
		float value = e.Left + e.Right;

		//n++;

		//a = a + (value - a) / n;

		//Debug.Log(a);

		//Remove first if full
		if (blocks1.Count > amnVisuals - 1f)
			blocks1.RemoveAt(0);

		//Add positive values
		if (value > 0)
		{
			blocks1.Add(value);
		}
		else
		{
			//blocks1.Add(0);
		}

		//Remove first if full
		if (blocks2.Count > amnVisuals - 1f)
			blocks2.RemoveAt(0);

		//Add negative values;
		if (value < 0)
		{
			blocks2.Add(value);
		}
		else
		{
			//blocks2.Add(0);
		}




		//if (blocks.Count > amnVisuals)
		//	blocks.RemoveAt(0);

		//blocks.Add(e.Left + e.Right); //both is most even
	}

	bool paused = false;

	private void OnApplicationPause(bool pause)
	{
		if (pause)
		{
			paused = true;
			StopListen();
		}
	}

	private void OnApplicationFocus(bool focus)
	{
		if (focus && paused)
		{
			paused = false;
			StartListen();
		}
	}

	private void OnApplicationQuit()
	{
		StopListen();
	}
}
