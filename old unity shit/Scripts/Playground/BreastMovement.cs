using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreastMovement : MonoBehaviour
{
	public Transform parent;
	public Transform leftBreast;
	public Transform rightBreast;

	private Vector3 leftBreastLastPos;
	private Vector3 rightBreastLastPos;

	private Transform leftBreastAnchor;
	private Transform rightBreastAnchor;

	private float updato;
	void Start()
    {
		if (!leftBreast)
			return;

		if (!rightBreast)
			return;

		leftBreastAnchor = new GameObject().transform;
		leftBreastAnchor.parent = parent;
		leftBreastAnchor.position = leftBreast.transform.position;
		rightBreastAnchor = new GameObject().transform;
		rightBreastAnchor.parent = parent;
		rightBreastAnchor.position = rightBreast.transform.position;

		leftBreastLastPos = leftBreast.transform.position;
		rightBreastLastPos = rightBreast.transform.position;
	}

	void Update()
    {

    }

	//Crude code
	private void LateUpdate()
	{
		leftBreast.transform.position = leftBreastLastPos;
		rightBreast.transform.position = rightBreastLastPos;

		//too far wiggle = imediate update on next frame
		if (Vector3.Distance(leftBreastLastPos, leftBreastAnchor.position) > 0.2f || Vector3.Distance(rightBreastLastPos, rightBreastAnchor.position) > 0.2f)
			updato = 1f;


		leftBreastLastPos = Vector3.Lerp(leftBreastLastPos, leftBreastAnchor.position, updato);
		rightBreastLastPos = Vector3.Lerp(rightBreastLastPos, rightBreastAnchor.position, updato);

		updato += 0.001f;

		if (updato >= 1)
			updato = 0;

	}
}
