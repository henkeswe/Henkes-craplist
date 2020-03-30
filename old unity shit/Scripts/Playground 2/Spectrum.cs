using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spectrum : MonoBehaviour
{
    const int SAMPLE_SIZE = 1024;

    public AudioSource source;
    public Material customBackgroundMaterial;
    public Material customVisualMaterial;
    public Material customBassBackgroundMaterial;
    Material defaultMat;

    Light dirLight;
    LineRenderer rend;
    LineRenderer rend2;

    float maxVisualScale = 15f;
    float visualModifier = 50f;
    float smoothSpeed = 20f;
    float keepPercent = 0.1f;

    float rmsValue;
    float dbValue;
    //float pitchValue;
    float[] samples;
    float[] spectrum;
    float sampleRate;

    Transform bgBass;
    Transform bg;
    Transform[] visualList;
    //Transform[] lineList;
    float[] visualScale;
    int amnVisual = 100;

    Color bgColor = new Color(0.1f, 0.1f, 0.1f);

    void Start()
    {
		GameObject lightObject = new GameObject();
		lightObject.transform.parent = transform;

		lightObject.transform.rotation = Quaternion.Euler(0, 180, 0);
		defaultMat = new Material(Shader.Find("Diffuse"));
        dirLight = lightObject.AddComponent<Light>();
        dirLight.type = LightType.Directional;


       //light.intensity = 1;
        

        samples = new float[SAMPLE_SIZE];
        spectrum = new float[SAMPLE_SIZE];
        sampleRate = AudioSettings.outputSampleRate;

        SpawnVisualLine();
    }

    void Update()
    {
        AnalyzeSound();
        UpdateVisual();
    }

    void AnalyzeSound()
    {
        source.GetOutputData(samples, 0);

        //RMS
        int i = 0;
        float sum = 0;

        for (; i < SAMPLE_SIZE; i++)
        {
            sum = samples[i] * samples[i];
        }

        rmsValue = Mathf.Sqrt(sum / SAMPLE_SIZE);

        dbValue = 20 * Mathf.Log10(rmsValue / 0.1f);

        source.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);
    }

    void UpdateVisual()
    {
        int visualIndex = 0;
        int spectrumIndex = 0;
        int averageSize = (int)(SAMPLE_SIZE * keepPercent / amnVisual);

        while (visualIndex < amnVisual)
        {
            int j = 0;
            float sum = 0;

            while (j < averageSize)
            {
                sum += spectrum[spectrumIndex];
                spectrumIndex++;
                j++;
            }

            float scaleY = sum / averageSize * visualModifier;
            visualScale[visualIndex] -= Time.deltaTime * smoothSpeed;

            if (visualScale[visualIndex] < 0)
                visualScale[visualIndex] = 0;

            if (visualScale[visualIndex] < scaleY)
                visualScale[visualIndex] = scaleY;

            if (visualScale[visualIndex] > maxVisualScale)
                visualScale[visualIndex] = maxVisualScale;

            visualList[visualIndex].localScale = new Vector3(0.25f, 0.5f + visualScale[visualIndex], 0.1f);
            visualList[visualIndex].GetComponent<MeshRenderer>().material.color = Color.Lerp(Color.white, Color.blue, visualScale[visualIndex] / maxVisualScale);

            visualIndex++;
        }


        float lerpValue = rmsValue * 25f;//rmsValue * 1000f * Time.deltaTime;//rmsValue * 25f; // visualScale[1] / maxVisualScale;
        dirLight.color = Color.Lerp(Color.white, new Color(0.25f, 0, 0.5f), lerpValue);
        dirLight.intensity = Mathf.Clamp(lerpValue, 0.5f, 1f);

        bg.transform.GetComponent<MeshRenderer>().material.color = Color.Lerp(new Color(0.05f, 0.05f, 0.05f), new Color(0.25f, 0, 0.5f), lerpValue);



        //bgBall.transform.localScale = new Vector3(bgBall.transform.localScale.x - Time.deltaTime * smoothSpeed, 0.01f, bgBall.transform.localScale.z - Time.deltaTime * smoothSpeed);
        bgBass.transform.localScale = new Vector3(2.5f + rmsValue * 10, 0.1f, 2.5f +  rmsValue * 10);
        bgBass.transform.GetComponent<MeshRenderer>().material.color = Color.Lerp(new Color(0.25f, 0.25f, 0.25f), Color.magenta, lerpValue);

        UpdateLine();
        rend.material.color = Color.Lerp(Color.white, Color.magenta, lerpValue);
        rend2.material.color = Color.Lerp(Color.white, Color.magenta, lerpValue);
    }
    
    void SpawnVisualLine()
    {
        visualScale = new float[amnVisual];
        visualList = new Transform[amnVisual];

        //Line renderer
        rend = new GameObject().gameObject.AddComponent<LineRenderer>();
        Transform t = rend.transform;
        rend.startWidth = 0.1f;
        rend.endWidth = 0.1f;
        t.parent = transform;
        rend.positionCount = amnVisual;
        rend.material = defaultMat;

        rend2 = new GameObject().gameObject.AddComponent<LineRenderer>();
        Transform t2 = rend2.transform;
        rend2.startWidth = 0.1f;
        rend2.endWidth = 0.1f;
        t2.parent = transform;
        rend2.positionCount = amnVisual;
        rend2.material = defaultMat;

        //Background
        bg = GameObject.CreatePrimitive(PrimitiveType.Plane).transform;
        bg.parent = transform;
        bg.eulerAngles = new Vector3(-90, 0, 0);
        bg.position = transform.position + transform.forward;
        bg.localScale = new Vector3(10, 1, 10);
        Destroy(bg.GetComponent<MeshCollider>());
        MeshRenderer bgMr = bg.GetComponent<MeshRenderer>();
        bgMr.material = defaultMat;
        bgMr.material.color = new Color(0.1f, 0.1f, 0.1f);

        if (customBackgroundMaterial != null)
            bgMr.material = customBackgroundMaterial;

        bgBass = GameObject.CreatePrimitive(PrimitiveType.Plane).transform;
        bgBass.parent = transform;
        bgBass.eulerAngles = new Vector3(90, 180, 0);
        bgBass.position = transform.position + transform.forward / 2f + transform.up * 7.5f;
        bgBass.localScale = new Vector3(1, 1, 1);
        Destroy(bgBass.GetComponent<MeshCollider>());
        MeshRenderer bgBassMr = bgBass.GetComponent<MeshRenderer>();
        bgBassMr.material = defaultMat;
        bgBassMr.material.color = new Color(0.1f, 0.1f, 0.1f);

        if (customBassBackgroundMaterial != null)
            bgBassMr.material = customBassBackgroundMaterial;

        //Loop for visual line bars
        for (int i = 0; i < amnVisual; i++)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube) as GameObject;
            Destroy(go.GetComponent<BoxCollider>());
            visualList[i] = go.transform;
            visualList[i].parent = transform;

            if (customVisualMaterial != null)
                visualList[i].GetComponent<MeshRenderer>().material = customVisualMaterial;

            Vector3 increment = Vector3.right / 4;
            Vector3 visualIncrement = (Vector3.left / 2 * amnVisual / 2f);
            Vector3 nextPos = (transform.right / 2 * i);

            visualList[i].position = transform.position + increment + visualIncrement + nextPos;
        }
    }

    void UpdateLine()
    {
        for(int i = 0; i < visualList.Length; i++)
        {
            rend.SetPosition(i, visualList[i].transform.up + (visualList[i].transform.up * visualList[i].localScale.y / 2) + visualList[i].transform.position);// - transform.forward * 0.01f));
            rend2.SetPosition(i, -visualList[i].transform.up + (-visualList[i].transform.up * visualList[i].localScale.y / 2) + visualList[i].transform.position);// - transform.forward * 0.01f));
        }

    }


    private void OnGUI()
    {
        int textHeight = 20;
        int textWidth = Screen.width / 3;

        TimeSpan sDur = TimeSpan.FromSeconds(source.time);
        TimeSpan sEnd = TimeSpan.FromSeconds(source.clip.length);

        string soundTime = string.Format("{0:D2}:{1:D2}:{2:D2}",
            sDur.Hours,
            sDur.Minutes,
            sDur.Seconds
        );

        string soundEnd = string.Format("{0:D2}:{1:D2}:{2:D2}",
            sEnd.Hours,
            sEnd.Minutes,
            sEnd.Seconds
        );


        GUI.TextArea(new Rect(0, Screen.height - textHeight * 2, textWidth, textHeight), "SAMPLE RATE: " + sampleRate);
        GUI.TextArea(new Rect(textWidth, Screen.height - textHeight * 2, textWidth, textHeight), "FPS: " + (1 / Time.deltaTime));
        GUI.TextArea(new Rect(textWidth * 2, Screen.height - textHeight * 2, textWidth, textHeight), "TIME: " + soundTime + " / " + soundEnd);

        GUI.TextArea(new Rect(0, Screen.height - textHeight, textWidth, textHeight), "RMS: " + rmsValue);
        GUI.TextArea(new Rect(textWidth, Screen.height - textHeight, textWidth, textHeight), "DB: " + dbValue);
        GUI.TextArea(new Rect(textWidth * 2, Screen.height - textHeight, textWidth, textHeight), source.clip.name);
    }
}
