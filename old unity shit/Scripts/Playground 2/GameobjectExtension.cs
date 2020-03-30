using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameObjectExtension
{
	public static T GetOrAddComponent<T>(this GameObject go) where T : Component
	{
		return go.GetComponent<T>() ? go.GetComponent<T>() : go.AddComponent<T>();
	}

	public static T GetComponentByChildName<T>(this GameObject go, string childName) where T : Component
	{
		Transform find = go.transform.Find(childName);

		return find ? find.gameObject.GetComponent<T>() : null;
	}

	public static T GetOrAddChildComponent<T>(this GameObject go, string childName) where T : Component
	{
		if (!go.GetComponentByChildName<T>(childName))
		{
			GameObject g = new GameObject(childName);
			g.transform.position = go.transform.position;
			g.transform.parent = go.transform;
			g.AddComponent<T>();
		}

		return go.GetComponentByChildName<T>(childName);
	}
}
