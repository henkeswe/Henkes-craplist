using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Ball : MonoBehaviour
{
	public enum ChaseMode { Follow, Flee, Attack, Wander };

	public string targetTag = "Player";

	[Header("Behaviour")]
	public ChaseMode behaviourMode;
	public BehaviourSettings[] behaviourSettings;



	private bool useConstantSpeed = false;
	private float constantSpeed = 5f;
	private float range = 5f;

	private Color targetColor = Color.white;
	private Color noTargetColor = Color.black;

	private Rigidbody rb;
	private MeshRenderer mr;

	private float colorLerpTime = 0;

	private float decreaseTickChance = 0;

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
		mr = GetComponent<MeshRenderer>();

		SetupBehaviour();

		mr.material.color = noTargetColor;
	}

	void SetupBehaviour()
	{
		for (int i = 0; i < behaviourSettings.Length; i++)
		{
			if (behaviourMode == behaviourSettings[i].chaseMode)
			{
				targetColor = behaviourSettings[i].targetColor;
				noTargetColor = behaviourSettings[i].noTargetColor;
				useConstantSpeed = behaviourSettings[i].useConstantSpeed;
				constantSpeed = behaviourSettings[i].constantSpeed;
				range = behaviourSettings[i].range;
				decreaseTickChance = behaviourSettings[i].decreaseTick;
			}
		}
	}

	private void FixedUpdate()
	{
		float tick = Random.Range(-10.0f - decreaseTickChance, 10.0f + decreaseTickChance);

		if (behaviourMode == ChaseMode.Wander)
		{
			if((int)tick == 5f)
			{
				Vector3 dir = new Vector3(Random.Range(-10, 10), Random.Range(0, 5), Random.Range(-10, 10));
				rb.AddForce(dir, ForceMode.Impulse);
			}
		}
		else if (HasTarget())
		{
			if ((int)tick == 5f)
			{
				Vector3 dir = FindTargetInSphereByTag(targetTag, range).transform.position - transform.position;

				if (behaviourMode == ChaseMode.Flee)
					dir = -dir;

				if (useConstantSpeed)
					dir = (dir / Vector3.Distance(FindTargetInSphereByTag(targetTag, range).transform.position, transform.position) * constantSpeed);

				rb.AddForce(dir, ForceMode.Impulse);


			}

			if (mr.material.color != targetColor)
			{
				colorLerpTime += Time.deltaTime;
				mr.material.color = Color.Lerp(noTargetColor, targetColor, colorLerpTime);
			}
			else
			{
				if (colorLerpTime != 0)
					colorLerpTime = 0;
			}
		}
		else
		{
			//Color lerping
			if (mr.material.color != noTargetColor)
			{
				colorLerpTime += Time.deltaTime;
				mr.material.color = Color.Lerp(targetColor, noTargetColor, colorLerpTime);
			}
			else
			{
				if (colorLerpTime != 0)
					colorLerpTime = 0;
			}
		}
	}

	private GameObject FindTargetInSphereByTag(string tag, float range)
	{
		GameObject target = null;
		float lastDistance = Mathf.Infinity;

		Collider[] hitColliders = Physics.OverlapSphere(transform.position, range);

		foreach (Collider col in hitColliders)
		{
			Transform currentTransform = null;

			if (col.transform.tag == tag)
			{
				currentTransform = col.transform;
			}
			else if (col.transform.parent != null)
			{
				if (col.transform.parent.tag == tag)
					currentTransform = col.transform.parent;
			}

			if (currentTransform != null)
			{
				GameObject colGameObject = currentTransform.gameObject;

				if (Vector3.Distance(colGameObject.transform.position, transform.position) < lastDistance)
					target = colGameObject.gameObject;

				lastDistance = Vector3.Distance(colGameObject.transform.position, transform.position);
			}
		}

		return target;
	}

	private bool HasTarget()
	{
		return (FindTargetInSphereByTag(targetTag, range) != null);
	}

	[System.Serializable]
	public class BehaviourSettings
	{
		public ChaseMode chaseMode;
		public Color targetColor;
		public Color noTargetColor;

		public float range = 10f;
		public bool useConstantSpeed = false;
		public float constantSpeed = 5f;

		public float damage = 0;
		public float pushPower = 0;

		public float decreaseTick = 0;
	}
}



/*
class NPC_Hunter
{

	public string targetTag = "Player";
	public bool reverseDir = false;

	public bool useConstantSpeed = false;
	public float constantSpeed = 5f;

	public Color hasNoTargetColor = Color.yellow;
	public Color hasTargetColor = Color.green;

	public bool useSprite = true;

	private Transform sprite;

	public Sprite hasTargetSprite;
	public Sprite hasNoTargetSprite;

	private float colorLerpTime = 0;

	private Rigidbody rb;
	private MeshRenderer mr;

	private SpriteRenderer sp;

	void Start()
	{
		rb = GetComponent<Rigidbody>();
		mr = GetComponentInChildren<MeshRenderer>();

		if (useSprite)
		{
			sprite = new GameObject().transform;
			sprite.name = "SpriteObject";
			sprite.parent = transform;
			sprite.gameObject.AddComponent<SpriteRenderer>();

			sp = sprite.GetComponent<SpriteRenderer>();
		}
	}

	void UpdateSprite()
	{
		if (sprite != null)
		{
			float scale = Vector3.Distance(Camera.main.transform.position, sprite.transform.position);
			scale = Mathf.Clamp(scale, 2, 10);
			sprite.transform.localScale = new Vector3(scale, scale, scale);
			sprite.transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
			sprite.LookAt(Camera.main.transform);
		}
	}

	void SetSprite(Sprite spriteTex)
	{
		if (sprite != null && sp.sprite != spriteTex)
		{
			sp.sprite = spriteTex;
		}
	}

	void FixedUpdate()
	{
		if (useSprite)
			UpdateSprite();

		float tick = Random.Range(-10.0f, 10.0f);

		if (HasTarget())
		{
			if ((int)tick == 5f)
			{
				Vector3 dir = FindTargetByTag(targetTag).transform.position - transform.position;

				if (reverseDir)
					dir = -dir;

				if (useConstantSpeed)
					dir = (dir / Vector3.Distance(FindTargetByTag(targetTag).transform.position, transform.position) * constantSpeed);

				rb.AddForce(dir, ForceMode.Impulse);
			}

			//Color lerping
			if (mr.material.color != hasTargetColor)
			{
				colorLerpTime += Time.deltaTime;
				mr.material.color = Color.Lerp(hasNoTargetColor, hasTargetColor, colorLerpTime);
			}
			else
			{
				if (colorLerpTime != 0)
					colorLerpTime = 0;
			}

			//Sprite
			SetSprite(hasTargetSprite);
		}
		else
		{
			if (rb.velocity != Vector3.zero)
			{
				rb.velocity = rb.velocity * 0.98f;
			}

			//Color lerping
			if (mr.material.color != hasNoTargetColor)
			{
				colorLerpTime += Time.deltaTime;
				mr.material.color = Color.Lerp(hasTargetColor, hasNoTargetColor, colorLerpTime);
			}
			else
			{
				if (colorLerpTime != 0)
					colorLerpTime = 0;
			}

			SetSprite(hasNoTargetSprite);
		}
	}

	private bool HasTarget()
	{
		return (FindTargetByTag(targetTag) != null);
	}

	private GameObject FindTargetByTag(string tag)
	{
		GameObject target = null;

		Collider[] hitColliders = Physics.OverlapSphere(transform.position, 10f);

		float lastDistance = Mathf.Infinity;

		foreach (Collider col in hitColliders)
		{
			if (col.transform.parent != null)
			{
				if (col.transform.parent.tag == targetTag)
				{
					GameObject colGameObject = col.transform.parent.gameObject;

					if (Vector3.Distance(colGameObject.transform.position, transform.position) < lastDistance)
						target = colGameObject.gameObject;

					lastDistance = Vector3.Distance(colGameObject.transform.position, transform.position);
				}
			}
			//else
			//{
			//	if (col.transform.tag == targetTag)
			//		target = col.transform.gameObject;
			//
			//}
		}

		return target;
	}

}

*/