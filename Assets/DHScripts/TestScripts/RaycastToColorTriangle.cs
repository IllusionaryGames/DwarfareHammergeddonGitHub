using UnityEngine;
using System.Collections;

public class RaycastToColorTriangle : MonoBehaviour
{
	int counter = 0;
	FogOfWar fow;

	public GameObject plane;

	void Start()
	{
		//fow = plane.GetComponent<FogOfWar>();
	}

	void Update ()
	{
		if (Input.GetMouseButtonDown (0))
		{
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit))
			{
				Debug.Log ("hit");
				fow = hit.collider.gameObject.GetComponent<FogOfWar>();
				Debug.Log (hit.triangleIndex);
				//Debug.Log (hit.triangleIndex);
				//if (fow != null)
					//fow.MakeTransparent(hit.triangleIndex);
				//int tI = hit.triangleIndex;
				//Debug.Log (tI);
				//Mesh hitMesh = hit.collider.gameObject.GetComponent<MeshFilter>().mesh;
				//Debug.Log (hitMesh.triangles[3*tI]);
				//Debug.Log (3*tI);
			}
		}
		//Debug.Log (fow);
		//Debug.Log (fow + " " + counter);
		//if (fow != null)
		//{
		//	fow.MakeTransparent(counter);
		//	counter ++ ;
		//}
	}
}
