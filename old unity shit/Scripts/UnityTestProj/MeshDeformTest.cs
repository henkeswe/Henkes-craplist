using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MeshDeformTest : MonoBehaviour
{
	float delay = 0.5f;

	Vector3[] oldVerts;
	Vector2[] oldUVs;
	int[] oldTris;


	Vector3[] newVerts;

	Mesh newMesh;

	MeshFilter mf;
	MeshCollider mc;

	void Start ()
	{
		delay = Time.time;

		mf = GetComponent<MeshFilter>();
		mc = GetComponent<MeshCollider>();

		oldVerts = mf.mesh.vertices;
		oldUVs = mf.mesh.uv;
		oldTris = mf.mesh.triangles;

		newVerts = oldVerts;

		newMesh = new Mesh
		{
			vertices = oldVerts,
			uv = oldUVs,
			triangles = oldTris
		};

		newMesh.RecalculateNormals();

		mf.sharedMesh = newMesh;
		mc.sharedMesh = newMesh;
	}


	Dictionary<GameObject, float> touchedObjects = new Dictionary<GameObject, float>();

	private void OnCollisionEnter(Collision collision)
	{
		//if( Time.time > delay)
		//{
			foreach (ContactPoint cp in collision.contacts)
			{
				List<Vector3> verts = oldVerts.ToList<Vector3>();

				if(touchedObjects.ContainsKey(cp.otherCollider.gameObject))
				{
					if(Time.time < touchedObjects[cp.otherCollider.gameObject])
					{
						return;
					}
					else
					{
						touchedObjects[cp.otherCollider.gameObject] = Time.time + 1f;
					}
				}
				else
				{
					touchedObjects[cp.otherCollider.gameObject] = Time.time + 1;
				}

				for (int i = 0; i < verts.Count; i++)
				{
					if (Vector3.Distance(verts[i], cp.point) <= 2f)
					{
						newVerts[i] += new Vector3(0, -0.1f, 0);
					}
				}

				newMesh.SetVertices(newVerts.ToList());
				mf.sharedMesh = newMesh;
				mc.sharedMesh = newMesh;
			}

			//delay = Time.time + 1;
		//}

		
		//Debug.Log("MEOW");
	}
}
