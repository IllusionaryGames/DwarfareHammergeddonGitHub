using UnityEngine;
using System.Collections;

public class Example : MonoBehaviour {
	void Start() {
		Mesh mesh = GetComponent<MeshFilter>().mesh;
		Vector3[] vertices = mesh.vertices;
		Color[] colors = new Color[vertices.Length];
		int i = 0;
		while (i < vertices.Length) {
			colors[i] = Color.Lerp(Color.red, Color.green, vertices[i].y);
			//colors[i] = new Color(1, 0, 0);
			i++;
		}

		mesh.colors = colors;
		
		for (int k = 0; k < mesh.colors.Length; ++k)
		{
			Debug.Log (mesh.colors[k]);
		}
	}
}
