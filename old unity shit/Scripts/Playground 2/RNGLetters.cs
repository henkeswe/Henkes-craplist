using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RNGLetters
{
	public static string GetRandomLetters()
	{
		return GenerateRandomLetters(Random.Range(2, 10));
	}

	public static string GetRandomLetters(int amnt)
	{
		return GenerateRandomLetters(amnt);
	}

	public static string GetRandomName()
	{
		int r = (int)Random.Range(2, 10);
		return GenerateName(r);
	}

	public static string GetRandomName(int amnt)
	{
		return GenerateName(amnt);
	}


	private static string GenerateRandomLetters(int amnt)
	{
		char[] alphaBeta = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

		string letters = "";
		for (int i = 0; i < amnt; i++)
		{
			int r = (int)Random.Range(0, alphaBeta.Length);

			letters += alphaBeta[r];
		}

		return letters;
	}

	private static string GenerateName(int amnt)
	{
		char[] alphaBeta = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

		string name = "Not Implented";

		return name;
	}
}
