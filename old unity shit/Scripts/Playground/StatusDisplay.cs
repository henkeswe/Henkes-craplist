using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusDisplay : MonoBehaviour
{
	GUIStyle fpsGUIstyle;
	GUIStyle fpsShadowGUIstyle;

	float scrH = Screen.height;
	float scrW = Screen.width;

	readonly float margin = 10f;

	readonly float textWidth = 100f;
	readonly float textHeight = 20f;

	readonly float sm = 1f;

	private void Start()
	{
		StartFPSGUI();
	}

	void OnGUI()
	{
		UpdateScreen();
		OnFPSGUI();
		OnStatusGUI();
	}

	void StartFPSGUI()
	{
		fpsGUIstyle = new GUIStyle();
		fpsGUIstyle.alignment = TextAnchor.MiddleRight;
		fpsGUIstyle.normal.textColor = Color.green;
		fpsGUIstyle.fontSize = 16;

		fpsShadowGUIstyle = new GUIStyle();
		fpsShadowGUIstyle.alignment = TextAnchor.MiddleRight;
		fpsShadowGUIstyle.normal.textColor = Color.black;
		fpsShadowGUIstyle.fontSize = 16;
	}

	void UpdateScreen()
	{
		if (scrW != Screen.width)
			scrW = Screen.width;

		if (scrH != Screen.height)
			scrH = Screen.height;
	}

	void OnFPSGUI()
	{
		float fps = PerfStats.FPS.GetCalculatedFPS();//PerfStats.GetFPS();

		string fpsText = fps + " FPS";

		float x = scrW - textWidth - margin;
		float y = 0f + margin;

		GUI.Label(new Rect(x + sm, y - sm, textWidth, textHeight), fpsText, fpsShadowGUIstyle);
		GUI.Label(new Rect(x - sm, y + sm, textWidth, textHeight), fpsText, fpsShadowGUIstyle);

		GUI.Label(new Rect(x + sm, y + sm, textWidth, textHeight), fpsText, fpsShadowGUIstyle);
		GUI.Label(new Rect(x - sm, y - sm, textWidth, textHeight), fpsText, fpsShadowGUIstyle);

		GUI.Label(new Rect(x, y, textWidth, textHeight), fpsText, fpsGUIstyle);
	}

	void OnStatusGUI()
	{

		List<string> texts = new List<string>
		{
			PerfStats.Memory.GetMonoHeapSizeMB() + "MB Heap",
			PerfStats.Memory.GetCurrentUsedMonoMB() + "MB Mono",
			PerfStats.Memory.GetAllocatedMemoryMB() + "MB Alloc",
			PerfStats.Memory.GetReservedMemoryMB() + "MB Res",
			PerfStats.Memory.GetUnusedMemoryMB() + "MB Unused",
		};

		float x = scrW - textWidth - margin;
		float y = textHeight + margin;

		int j = 0;

		for(int i = 0; i < texts.Count; i++)
		{
			j++;
			GUI.Label(new Rect(x + sm, y * j - sm, textWidth, textHeight), texts[i], fpsShadowGUIstyle);
			GUI.Label(new Rect(x - sm, y * j + sm, textWidth, textHeight), texts[i], fpsShadowGUIstyle);

			GUI.Label(new Rect(x + sm, y * j + sm, textWidth, textHeight), texts[i], fpsShadowGUIstyle);
			GUI.Label(new Rect(x - sm, y * j - sm, textWidth, textHeight), texts[i], fpsShadowGUIstyle);

			GUI.Label(new Rect(x, y * j, textWidth, textHeight), texts[i], fpsGUIstyle);
		}

	}

}

