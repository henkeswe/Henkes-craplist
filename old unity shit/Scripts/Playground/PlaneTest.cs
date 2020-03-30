using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneTest : MonoBehaviour
{
	int size = 10;
	float dist = 10;

	float scaleModifier = 1;

    void Start()
    {
		CreatePlane(size, dist, scaleModifier);
	}

	void CreatePlane(int planeSize, float planeDistance = 1f, float planeScale = 1f)
	{
		float centerMargin = 0f;

		if (size % 2 == 0)
		{
			centerMargin = -planeDistance / 2f;
		}

		for (int y = 0; y < planeSize; y++)
		{
			for (int x = 0; x < planeSize; x++)
			{
				GameObject ob = GameObject.CreatePrimitive(PrimitiveType.Plane);
				ob.transform.localScale = new Vector3(planeScale, planeScale, planeScale);
				ob.transform.parent = this.transform;

				Vector3 cent = new Vector3(centerMargin + (planeSize / 2) * planeDistance, 0, centerMargin + (planeSize / 2) * planeDistance);
				Vector3 pos = new Vector3(y * planeDistance, 0, x * planeDistance);
				ob.transform.position = this.transform.position - cent + pos;
			}
		}
	}
   
	// Update is called once per frame
    void Update()
    {
        
    }
}
