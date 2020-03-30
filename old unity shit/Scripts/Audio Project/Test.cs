using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CSCore;
using CSCore.SoundIn;
using CSCore.Streams;
using System;
using CSCore.DSP;

public class Test : MonoBehaviour
{

	const int SAMPLE_SIZE = 1024;
	const int DUAL_SAMPLE_SIZE = 512;
	int maxVisualScale = 5;
	private List<Transform> objects = new List<Transform>();
	private List<Transform> objectsRight = new List<Transform>();
	private List<Transform> objectsLeft = new List<Transform>();
	private List<float> blocks = new List<float>();
	//float[] blocks = new float[SAMPLE_SIZE];
	private List<float> leftBlocks = new List<float>();
	private List<float> rightBlocks = new List<float>();
	int visualMultiplier = 4;//8;
	int visualMultiplierDual = 4;//4;
	int amnVisuals = 90;
	int amnVisualsRight = 60;
	int amnVisualsLeft = 60;

	public GameObject bg;
	public Material pinMat;
	public GameObject speakerObject;

	public Color bgColor = Color.black, BgColorHigh = Color.blue, pinColor = Color.blue, pinColorHigh = Color.white, sidePins = Color.red, sidePinsHigh = Color.white;

	private WasapiLoopbackCapture loopbackCapture;
	private SoundInSource soundInSource;
	private IWaveSource realTimeSource;
	private SingleBlockNotificationStream singleBlockNotificationStream;

	void Start()
    {
		SetUpObjects();
		SetUpObjectsDual();
		StartListen();
	}

	void Update()
	{
		UpdateObj();
		//UpdateObjects();
		//UpdateObjectsDual();
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

		singleBlockNotificationStream.SingleBlockRead += singleBlockNotificationStream_SingleBlockRead;
	}
	void StopListen()
	{
		singleBlockNotificationStream.SingleBlockRead -= singleBlockNotificationStream_SingleBlockRead;

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

	private void singleBlockNotificationStream_SingleBlockRead(object sender, SingleBlockReadEventArgs e)
	{
		if (blocks.Count > amnVisuals)
			blocks.RemoveAt(0);

		blocks.Add(e.Left + e.Right); //both is most even

		/*
		if (blocks.Count > SAMPLE_SIZE)
			blocks.RemoveAt(0);

		blocks.Add(e.Left + e.Right); //both is most even
		*/

		if (leftBlocks.Count > DUAL_SAMPLE_SIZE)
			leftBlocks.RemoveAt(0);

		if (rightBlocks.Count > DUAL_SAMPLE_SIZE)
			rightBlocks.RemoveAt(0);

		leftBlocks.Add(e.Left);
		rightBlocks.Add(e.Right);
	}

	private void SetUpObjectsDual()
	{
		GameObject rightContainer = new GameObject();
		rightContainer.transform.parent = transform;

		GameObject leftContainer = new GameObject();
		leftContainer.transform.parent = transform;

		rightContainer.transform.rotation = Quaternion.Euler(0, 0, 90f);
		rightContainer.transform.position = transform.position + Vector3.right * 25;

		leftContainer.transform.rotation = Quaternion.Euler(0, 0, -90f);
		leftContainer.transform.position = transform.position + Vector3.left * 25;

		for (int i = 0; i < amnVisualsRight; i++)
		{
			GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);

			go.transform.name = "Visual Object Right " + i;
			go.transform.parent = rightContainer.transform;
			if (go.GetComponent<BoxCollider>())
				Destroy(go.GetComponent<BoxCollider>());

			go.transform.localScale = new Vector3(0.25f, 1, 0.25f);
			go.transform.GetComponent<MeshRenderer>().material = pinMat;

			go.transform.position = (rightContainer.transform.position + -rightContainer.transform.right * i / 2) + rightContainer.transform.right * amnVisualsRight / 4f;

			objectsRight.Add(go.transform);
		}

		for (int i = 0; i < amnVisualsLeft; i++)
		{
			GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);

			go.transform.name = "Visual Object Left " + i;
			go.transform.parent = leftContainer.transform;
			if (go.GetComponent<BoxCollider>())
				Destroy(go.GetComponent<BoxCollider>());

			go.transform.localScale = new Vector3(0.25f, 1, 0.25f);
			go.transform.GetComponent<MeshRenderer>().material = pinMat;

			go.transform.position = (leftContainer.transform.position + -leftContainer.transform.right * i / 2) + leftContainer.transform.right * amnVisualsLeft / 4f;

			objectsLeft.Add(go.transform);
		}


	}

	private void SetUpObjects()
	{
		for(int i = 0; i < amnVisuals; i++)
		{
			GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
			go.transform.name = "Visual Object " + i;
			go.transform.parent = transform;
			if(go.GetComponent<BoxCollider>())
				Destroy(go.GetComponent<BoxCollider>());

			go.transform.localScale = new Vector3(0.25f, 1, 0.25f);
			go.transform.GetComponent<MeshRenderer>().material = pinMat;

			go.transform.position = (transform.position + Vector3.left * i / 2) + Vector3.right * amnVisuals / 4f;
			
			objects.Add(go.transform);
		}
	}

	private void UpdateObjectsDual()
	{
		float smoothTime = 15f;
		int objectIndexRight = 0;
		int objectIndexLeft = 0;

		int spectrumIndexLeft = 0;
		int spectrumIndexRight = 0;
		int averageSize = (int)(DUAL_SAMPLE_SIZE * 0.5 / amnVisuals);
		
		while (objectIndexRight < amnVisualsRight)
		{
			int index = 0;
			float sum = 0;

			while (index < averageSize && rightBlocks.Count > spectrumIndexRight)
			{
				sum += rightBlocks[spectrumIndexRight];
				spectrumIndexRight++;
				index++;
			}

			float curScale = objectsRight[objectIndexRight].localScale.x;
			float scaleY = sum / averageSize * visualMultiplierDual;

			float newScale = curScale -= Time.deltaTime * smoothTime;

			if (curScale < 0)
				newScale = 0;

			if (curScale < scaleY)
				newScale = scaleY;

			if (curScale > maxVisualScale)
				newScale = maxVisualScale;

			if (newScale < 0.25f)
				newScale = 0.25f;


			float posX = objectsRight[objectIndexRight].transform.localPosition.x;
			float posY = objectsRight[objectIndexRight].parent.position.y;
			float posZ = objectsRight[objectIndexRight].transform.localPosition.z;

			float scaleX = objectsRight[objectIndexRight].localScale.z;
			float scaleZ = objectsRight[objectIndexRight].localScale.z;
			objectsRight[objectIndexRight].localScale = new Vector3(newScale, scaleX, scaleZ);
			objectsRight[objectIndexRight].transform.localPosition = new Vector3(posX, posY + (newScale / 2), posZ);
			objectsRight[objectIndexRight].transform.GetComponent<MeshRenderer>().material.color = Color.Lerp(sidePins, sidePinsHigh, newScale / 15f);
			objectIndexRight++;

			
		}

		while (objectIndexLeft < amnVisualsLeft)
		{
			int index = 0;
			float sum = 0;

			while (index < averageSize && leftBlocks.Count > spectrumIndexLeft)
			{
				sum += leftBlocks[spectrumIndexLeft];
				spectrumIndexLeft++;
				index++;
			}

			float curScale = objectsLeft[objectIndexLeft].localScale.x;
			float scaleY = sum / averageSize * visualMultiplierDual;

			float newScale = curScale -= Time.deltaTime * smoothTime;

			if (curScale < 0)
				newScale = 0;

			if (curScale < scaleY)
				newScale = scaleY;

			if (curScale > maxVisualScale)
				newScale = maxVisualScale;

			if (newScale < 0.25f)
				newScale = 0.25f;

			float posX = objectsLeft[objectIndexLeft].transform.localPosition.x;
			float posY = objectsLeft[objectIndexLeft].parent.position.y;
			float posZ = objectsLeft[objectIndexLeft].transform.localPosition.z;

			float scaleX = objectsLeft[objectIndexLeft].localScale.z;
			float scaleZ = objectsLeft[objectIndexLeft].localScale.z;
			objectsLeft[objectIndexLeft].localScale = new Vector3(newScale, scaleX, scaleZ);
			objectsLeft[objectIndexLeft].transform.localPosition = new Vector3(posX, posY + (newScale / 2), posZ);
			objectsLeft[objectIndexLeft].transform.GetComponent<MeshRenderer>().material.color = Color.Lerp(sidePins, sidePinsHigh, newScale / 15f);
			objectIndexLeft++;

			
		}
	}

	float speakerObjectScale = 0;

	private void UpdateObj()
	{
		float minSize = 0.25f;
		for (int i = 0; i < amnVisuals; i++)
		{

			float curScale = objects[i].localScale.y;
			float scaleY = blocks[i] * 5f;
			float newScale = scaleY;



			objects[i].localPosition = new Vector3(objects[i].localPosition.x, newScale / 2f, objects[i].transform.localPosition.z);
			objects[i].localScale = new Vector3(objects[i].localScale.x, newScale, objects[i].localScale.z);
			objects[i].GetComponent<MeshRenderer>().material.color = pinColorHigh;

			if (newScale > speakerObjectScale)
				speakerObjectScale = newScale;

			//if (curScale > 0)
			//	newScale = curScale -= Time.deltaTime * 25f;

			//if (curScale < 0)
			//	newScale = curScale += Time.deltaTime * 25f;

		}

		//if(ballShit.transform.localScale.x > ballShitsBiggestScale)
		//	ballShitsBiggestScale -= Time.deltaTime * 20;


		float scaleMod = 0.2f;

		if (speakerObjectScale * scaleMod > 0.5f)
			speakerObject.transform.localScale = new Vector3(0.25f + speakerObjectScale * scaleMod, 1f, 0.25f + speakerObjectScale * scaleMod);
		//else
		//	ballShit.transform.localScale = new Vector3(0.5f + ballShitsBiggestScale * scaleMod, 0.1f, 0.5f + ballShitsBiggestScale * scaleMod);
		//ballShit.transform.localPosition = new Vector3(0, 8.5f, ballShitsBiggestScale * -1f);

		speakerObjectScale -= Time.deltaTime * 10f;

	}

	private void UpdateObjects()
	{
		int objectIndex = 0;
		int spectrumIndex = 0;
		int averageSize = (int)(SAMPLE_SIZE * 0.5 / amnVisuals);

		while (objectIndex < amnVisuals)
		{
			int j = 0;
			float sum = 0;

			while (j < averageSize && blocks.Count > spectrumIndex)
			{
				sum += blocks[spectrumIndex];
				spectrumIndex++;
				j++;
			}

			float curScale = objects[objectIndex].localScale.y;

			float scaleY = sum / averageSize * visualMultiplier;

			float newScale = curScale -= Time.deltaTime * 25f;

			if (curScale < 0)
				newScale = 0;

			if (curScale < scaleY)
				newScale = scaleY;

			if (curScale > maxVisualScale)
				newScale = maxVisualScale;

			if (newScale < 0.25f)
				newScale = 0.25f;

			float bgColorLerp = newScale / 15f;
			bg.transform.GetComponent<MeshRenderer>().material.color = Color.Lerp(bgColor, BgColorHigh, bgColorLerp);

			objects[objectIndex].localScale = new Vector3(objects[objectIndex].localScale.x, newScale, objects[objectIndex].localScale.z);
			Transform curOb = objects[objectIndex];

			curOb.transform.GetComponent<MeshRenderer>().material.color = Color.Lerp(pinColor, pinColorHigh, newScale / 15f);

			objectIndex++;
		}
	}

	bool paused = false;

	private void OnApplicationPause(bool pause)
	{ 
		if(pause)
		{
			paused = true;
			StopListen();
		}
	}

	private void OnApplicationFocus(bool focus)
	{
		if(focus && paused)
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
