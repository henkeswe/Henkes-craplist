using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCharacterController : MonoBehaviour
{
	public bool hasGun = true;
	public GameObject[] scaleInFirstPerson;
	public GameObject[] scaleInFirstPersonGun;

	public GameObject gun;

	private CursorLockMode curLockState = CursorLockMode.None;

	private float movSpeed = 2f;
	private float runSpeed = 4f;
	private float jmpSpeed = 5f;

	private Rigidbody rb;
	private Animator anim;

	private float mouseX;
	private float mouseY;
	private float scroll;

	private int maxCameraAngle = 90;
	private int maxScroll = 0;
	private int minScroll = -10;

	private int mouseSpeed = 6;

	public Camera cam;

	bool isGrounded;
	bool inThirdperson;

	Vector3 movDir;

	public GameObject model;
	Vector3 oldModelPos;

	public GameObject pelvisL;
	public GameObject pelvisR;
	public GameObject neck;

	float lerpValue;

	private void Start()
	{
		oldModelPos = model.transform.localPosition;

		SetFieldOfView(90f);
		rb = GetComponent<Rigidbody>();
		anim = GetComponentInChildren<Animator>();

		//TakeDamage(100);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.F1))
		{
			if (curLockState == CursorLockMode.Locked)
				curLockState = CursorLockMode.None;
			else if (curLockState == CursorLockMode.None)
				curLockState = CursorLockMode.Locked;

			Cursor.lockState = curLockState;
		}

		if(Input.GetKeyDown(KeyCode.F2))
		{
			hasGun = !hasGun;

			gun.gameObject.SetActive(!gun.gameObject.activeSelf);
		}

		mouseY -= Input.GetAxis("Mouse Y") * mouseSpeed;
		mouseX += Input.GetAxis("Mouse X") * mouseSpeed;
		scroll += Input.GetAxis("Mouse ScrollWheel");

		mouseY = Mathf.Clamp(mouseY, -maxCameraAngle, maxCameraAngle);
		scroll = Mathf.Clamp(scroll, minScroll, maxScroll);

		transform.localRotation = Quaternion.Euler(new Vector3(0, mouseX, 0));

		if (!inThirdperson)
			cam.transform.localRotation = Quaternion.Euler(new Vector3(mouseY, 0, 0));
		else
		{
			cam.transform.LookAt(transform);
			//cam.transform.localPosition = new Vector3(0, Mathf.Clamp(mouseY * Time.deltaTime, -1, 1) , 0);
		}


		if (scroll < 0)
			inThirdperson = true;
		else
			inThirdperson = false;

		if (!inThirdperson)
			model.transform.localPosition = new Vector3(oldModelPos.x, oldModelPos.y, -0.4f);
		else
			model.transform.localPosition = oldModelPos;
		//Debug.Log("3rdP" + inThirdperson);

		movDir = (transform.forward * Input.GetAxisRaw("Vertical")) + (transform.right * Input.GetAxisRaw("Horizontal"));//new Vector3(Input.GetAxisRaw("Horizontal") * movSpeed, 0, Input.GetAxisRaw("Vertical") * movSpeed);


		cam.transform.localPosition = new Vector3(cam.transform.localPosition.x, cam.transform.localPosition.y, scroll);

		anim.SetFloat("Forward", Input.GetAxisRaw("Vertical"));
		anim.SetFloat("Right", Input.GetAxisRaw("Horizontal"));

		if (Input.GetKey(KeyCode.LeftShift))
		{
			movDir *= runSpeed;
			anim.SetFloat("Forward", anim.GetFloat("Forward") * 2);
		}
		else
			movDir *= movSpeed;
	}

	private void FixedUpdate()
	{
		//Vector3

		//if (movDir != Vector3.zero)
		//	anim.SetFloat("Forward", 1);
		//else
		//	anim.SetFloat("Forward", 0);

		if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
		{
			rb.AddForce(transform.up * jmpSpeed, ForceMode.VelocityChange);
			anim.SetTrigger("Jump");
		}


		rb.MovePosition(rb.position + (movDir * Time.deltaTime));

		RaycastHit hit;

		//better
		if (Physics.SphereCast(transform.position, 0.4f, -transform.up, out hit, 0.65f))
		{
			isGrounded = true;
			anim.SetBool("Grounded", true);
		}
		else
		{
			isGrounded = false;
			anim.SetBool("Grounded", false);
		}

		Debug.Log(isGrounded);

		//isGrounded = false;
	}

	private void LateUpdate()
	{
		//START: Legs
		float legAngle = 35f;
		Vector3 desieredVector = Vector3.zero;

		if (anim.GetFloat("Forward") > 0)
		{
			if (anim.GetFloat("Right") > 0)
			{
				desieredVector = new Vector3(0, legAngle, 0);
			}
			else if (anim.GetFloat("Right") < 0)
			{
				desieredVector = new Vector3(0, -legAngle, 0);
			}

			pelvisL.transform.localEulerAngles = Vector3.Lerp(pelvisL.transform.localEulerAngles, desieredVector, lerpValue);
			pelvisR.transform.localEulerAngles = Vector3.Lerp(pelvisR.transform.localEulerAngles, desieredVector, lerpValue);
		}

		if (desieredVector == Vector3.zero)
		{
			lerpValue = 0;
		}
		else
			lerpValue += 0.1f;
		//END: Legs


		if (!inThirdperson)
		{
			foreach(GameObject go in scaleInFirstPerson)
			{
				go.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
				//go.transform.localScale = new Vector3(0.1, 0.1, 0.1);
				//go.transform.localPosition = new Vector3(0, 0, -5);
			}

			if(hasGun)
			{
				foreach (GameObject go in scaleInFirstPersonGun)
				{
					go.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
				}
			}
			else
			{
				foreach (GameObject go in scaleInFirstPersonGun)
				{
					go.transform.localScale = new Vector3(1f, 1f, 1f);
				}
			}

		}
		else
		{
			foreach (GameObject go in scaleInFirstPerson)
			{
				go.transform.localScale = new Vector3(1f, 1f, 1f);
				//go.transform.localScale = new Vector3(0.1, 0.1, 0.1);
				//go.transform.localPosition = new Vector3(0, 0, -5);
			}

			foreach (GameObject go in scaleInFirstPersonGun)
			{
				go.transform.localScale = new Vector3(1f, 1f, 1f);
			}
		}
		

		neck.transform.localEulerAngles = new Vector3(Mathf.Clamp(mouseY, -35, 35), 0, 0);
	}
	
	//angles weird
	void OnCollisionStay(Collision collisionInfo)
	{
		foreach (ContactPoint contact in collisionInfo.contacts)
		{
			//Debug.DrawRay(contact.point, contact.normal, Color.white);

			//Debug.DrawRay(transform.position, -contact.normal, Color.green);

			//float angle2 = Vector2.Angle(transform.position, transform.position - contact.normal);

			//Debug.Log(angle2);

			//float angle = Vector3.Angle(contact.point, contact.normal);

			//if (angle <= 135 && angle >= 0)
			//	isGrounded = true;

			//if (angle > 135)
				//Debug.Log(angle);
			
			//Debug.Log(Vector3.Angle(contact.point, contact.normal));
		}

	}

	IEnumerator CameraShake(float rotation)
	{
		float speed = 5f;

		for(float f = 0; f <= rotation; f += speed)
		{
			cam.transform.localEulerAngles = new Vector3(cam.transform.localEulerAngles.x, cam.transform.localEulerAngles.y, f);
			yield return null;
		}

		for(float f = rotation; f >= -rotation; f -= speed)
		{
			cam.transform.localEulerAngles = new Vector3(cam.transform.localEulerAngles.x, cam.transform.localEulerAngles.y, f);
			yield return null;
		}

		for(float f = -rotation; f <= 0; f += speed)
		{
			cam.transform.localEulerAngles = new Vector3(cam.transform.localEulerAngles.x, cam.transform.localEulerAngles.y, f);
			yield return null;
		}

		//for (float f = 80f; f < fov; f += 1f)
		//{
		//	cam.transform.localEulerAngles = new Vector3(0, 0, f);
		//	//SetFieldOfView(f);
		//	yield return null;
		//}
	}

	public void TakeDamage(float damage, Vector3 dir = new Vector3(), float force = 100f)
	{
		StartCoroutine("CameraShake", 25f);


		rb.AddForce(dir * force, ForceMode.Impulse);
	}

	private void SetFieldOfView(float fov)
	{
		//Thanks zo1d
		float hFOVrad = fov * Mathf.Deg2Rad;
		float camH = Mathf.Tan(hFOVrad * 0.5f) / cam.aspect;
		float vFOVrad = Mathf.Atan(camH) * 2;
		cam.fieldOfView = vFOVrad * Mathf.Rad2Deg;
	}

}
