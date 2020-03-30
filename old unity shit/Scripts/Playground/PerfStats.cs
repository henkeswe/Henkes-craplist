using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public static class PerfStats
{

	public static class FPS
	{
		//raw fps
		public static float GetFPS()
		{
			return (float)Math.Round(1.0f / Time.unscaledDeltaTime, 2);
		}


		static int cFrameCount = 0;
		static float cDt = 0.0f;
		static float cFps = 0.0f;
		static readonly float cUpdateRate = 4.0f;  // 4 updates per sec.

		//slower update, easier to see
		public static float GetCalculatedFPS()
		{
			cFrameCount++;
			cDt += Time.unscaledDeltaTime; //Time.deltaTime;//
			if (cDt > 1.0 / cUpdateRate)
			{
				cFps = cFrameCount / cDt;
				cFrameCount = 0;
				cDt -= 1.0f / cUpdateRate;
			}

			return (float)Math.Round((decimal)cFps, 2);
		}
	}
	
	public static class Net
	{

		//Placeholder
		public static float GetPing()
		{
			return 0;
		}
	}

	public static class Memory
	{
		//https://docs.unity3d.com/ScriptReference/Profiling.Profiler.html

		public static int GetGraphicsMemory()
		{
			return SystemInfo.graphicsMemorySize;
		}

		public static int GetSystemMemory()
		{
			return SystemInfo.systemMemorySize;
		}

		public static float GetMonoHeapSizeMB()
		{
			return (float)Math.Round(Profiler.GetMonoHeapSizeLong() / 1024f / 1024f, 2);
		}

		public static float GetCurrentUsedMonoMB()
		{
			return (float)Math.Round(Profiler.GetMonoUsedSizeLong() / 1024f / 1024f, 2);
		}

		public static float GetAllocatedMemoryMB()
		{
			return (float)Math.Round(Profiler.GetTotalAllocatedMemoryLong() / 1024f / 1024f, 2);
		}

		public static float GetReservedMemoryMB()
		{
			return (float)Math.Round(Profiler.GetTotalReservedMemoryLong() / 1024f / 1024f, 2);
		}

		public static float GetUnusedMemoryMB()
		{
			return (float)Math.Round(Profiler.GetTotalUnusedReservedMemoryLong() / 1024f / 1024f, 2);
		}


	}
}
