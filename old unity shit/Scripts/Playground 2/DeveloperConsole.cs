using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//test cmds, bad implementation atm
public static class ConsoleCommands
{
	static void Error()
	{
		Debug.LogError("Test Error");
	}

	static void Warning()
	{
		Debug.LogWarning("Test Warning");
	}

	static void Exception()
	{
		Debug.LogException(new Exception("Test Exception"));
	}

	static void Test()
	{
		DeveloperConsole.ConsoleMessage("Hi");
	}

	static void RngLetter()
	{
		Debug.Log(RNGLetters.GetRandomLetters());
	}

	public static Dictionary<string, Action> cmds = new Dictionary<string, Action>
	{
		{ "error", Error },
		{ "warning", Warning },
		{ "exception", Exception },
		{ "test", Test},
		{ "rng", RngLetter }
	};


}

public class DeveloperConsole : MonoBehaviour
{
	public struct Log
	{
		public string message;
		public string stackTrace;
		public LogType type;
		public bool cleanText;
	}

	public KeyCode toggleKey = KeyCode.F1;

	static List<Log> logs = new List<Log>();
	Vector2 scrollPosition = Vector2.zero;
	bool show = false;
	bool collapse = false;
	bool showStack = true;
	string textInput = string.Empty;

	static readonly Dictionary<LogType, Color> logTypeColors = new Dictionary<LogType, Color>()
	{
		{ LogType.Assert, Color.white },
		{ LogType.Error, Color.red },
		{ LogType.Exception, Color.red },
		{ LogType.Log, Color.white },
		{ LogType.Warning, Color.yellow },
	};

	const int margin = 20;

	Rect windowRect = new Rect(margin, margin, Screen.width - (margin * 2), Screen.height - (margin * 2));
	Rect titleBarRect = new Rect(0, 0, 10000, 20);
	GUIContent clearLabel = new GUIContent("Clear", "Clear the contents of the console.");
	GUIContent collapseLabel = new GUIContent("Collapse", "Hide repeated messages.");
	GUIContent showStackLabel = new GUIContent("Show Stacktrace", "Show stacktrace.");

	void OnEnable()
	{
		Application.logMessageReceived += HandleLog;
	}

	void OnDisable()
	{
		Application.logMessageReceived -= HandleLog;
	}

	void Update()
	{
		if (Input.GetKeyDown(toggleKey))
		{
			show = !show;
		}
	}

	void OnGUI()
	{
		if (!show)
		{
			return;
		}

		windowRect = GUILayout.Window(123456, windowRect, ConsoleWindow, "Console");
	}

	void ConsoleWindow(int windowID)
	{
		scrollPosition = GUILayout.BeginScrollView(scrollPosition);
		// Iterate through the recorded logs.

		//int collapseAmnt = 0;
		for (int i = 0; i < logs.Count; i++)
		{
			var log = logs[i];

			// Combine identical messages if collapse option is chosen.
			if (collapse)
			{

				var messageSameAsPrevious = i > 0 && log.message == logs[i - 1].message;

				//for(int b = 0; b < logs.Count;)

				if (messageSameAsPrevious)
				{
					//collapseAmnt++;
					continue;
				}
			}

			string message = "[" + log.type + "] " + log.message;

			if(showStack)
				message += "\n" + log.stackTrace;

			if (log.cleanText)
			{
				message = log.message;
			}

			GUI.contentColor = logTypeColors[log.type];
			GUILayout.Label(message);

		}

		GUILayout.EndScrollView();

		GUI.contentColor = Color.white;

		GUILayout.BeginHorizontal();

		GUILayoutOption[] textOptions = new GUILayoutOption[] {
			GUILayout.ExpandWidth(false),
			GUILayout.ExpandHeight(false),
			GUILayout.Width(Screen.width - (margin * 20)),
		};

		if(textInput.Length > 100)
		{
			textInput = textInput.Remove(textInput.Length - 1);
		}

		GUI.SetNextControlName("textInput");
		textInput = GUILayout.TextField(textInput, textOptions);

		if (Event.current.isKey && Event.current.keyCode == KeyCode.Return && GUI.GetNameOfFocusedControl() == "textInput")
		{
			if(textInput != "")
			{
				OnTextEnter(textInput);
				textInput = "";
			}
		}

		if (GUILayout.Button(clearLabel))
		{
			textInput = "";
			logs.Clear();
		}

		showStack = GUILayout.Toggle(showStack, showStackLabel, GUILayout.ExpandWidth(false));
		collapse = GUILayout.Toggle(collapse, collapseLabel, GUILayout.ExpandWidth(false));

		GUILayout.EndHorizontal();
	}

	void HandleLog(string message, string stackTrace, LogType type)
	{
		logs.Add(new Log()
		{
			message = message,
			stackTrace = stackTrace,
			type = type,
		});
	}

	public static void ConsoleMessage(string message)
	{
		logs.Add(new Log()
		{
			message = message,
			type = LogType.Log,
			cleanText = true,
		});
	}

	void OnTextEnter(string text)
	{
		

		if (ConsoleCommands.cmds.ContainsKey(text.ToLower()))
		{
			ConsoleMessage("CMD: " + text);
			ConsoleCommands.cmds[text.ToLower()]();
		}
		else
		{
			ConsoleMessage("Couldn't find command \"" + text + "\"");
		}
	}
}
