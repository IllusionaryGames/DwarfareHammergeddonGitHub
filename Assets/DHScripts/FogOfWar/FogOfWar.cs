using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshCollider))]
public class FogOfWar : MonoBehaviour
{
	private Mesh m_Mesh;
	private MeshFilter m_MeshFilter;

	public Color m_Color;

	private int m_iLengthVerts;

	private int[] m_iTris;

	void Start()
	{
		m_MeshFilter = GetComponent<MeshFilter>();

		if (m_MeshFilter == null) return;

		m_Mesh = m_MeshFilter.mesh;
		
		Color[] colors = m_Mesh.colors;

		for (int i = 0; i < colors.Length; ++i)
		{
			colors[i] = m_Color;
		}

		m_Mesh.colors = colors;

		m_iLengthVerts = (int)Mathf.Sqrt(m_Mesh.vertices.Length);
		m_iTris = m_Mesh.triangles;
		//Debug.Log (m_iLengthVerts);
	}

	public void MakeTransparent(int iTriangle, int iSight)
	{
		Color col = new Color(0f, 0f, 0f, 0f);

		Color[] colors = m_Mesh.colors;

		int iStart = 0;
		for (int i = -iSight; i <= iSight; ++i)
		{
			iStart = m_iTris[iTriangle*3];
			for (int k = iStart - iSight; k <= iStart + iSight; ++k)
			{
				int index = k + i * m_iLengthVerts;
				if (index < colors.Length)
					colors[index] = col;
			}
		}

		m_Mesh.colors = colors;
	}

	public void MakeTransparent2(Vector3 _pos)
	{
		Color col = new Color(0f, 0f, 0f, 0f);
		for (int i = 0; i < m_Mesh.vertices.Length; ++i)
		{
			if ((m_Mesh.vertices[i] - _pos).magnitude < 100f)
			{
				m_Mesh.colors[i] = col;
			}
		}
	}
}
