using UnityEngine;
using System.Collections;
using System.IO;

public struct IndexProfile
{
	public int iXorigin;
	public int iYorigin;
	public GameObject goObject;
	public bool bValid;
}

public class Grid : MonoBehaviour
{
	public float width = DHWorld.m_fWidth;
	public float height = DHWorld.m_fHeight;

	public int boundsX = DHWorld.m_iBoundsX;
	public int boundsY = DHWorld.m_iBoundsY;

	public string m_strLevelName;
	public string m_strTextureLevel;

	private PrefabProfile m_Profile;

	[SerializeField]
	public IndexProfile[,] m_arrIndexProfiles;
	
	private int m_sTileID = 0;

	public short TileID
	{
		get { return (short)m_sTileID; }
		set {
			if (value >= 0 && value <= short.MaxValue)
				m_sTileID = value;
			}
	}

	public GameObject SelectedPrefab
	{
		get { return m_Profile.m_goPrefab; }
	}
	public PrefabProfile SelectedProfile
	{
		get { return m_Profile; }
		set { m_Profile = value; }
	}

	void OnDrawGizmos()
	{

		//for (float y = pos.y - (float)boundsY; y < pos.y + (float)boundsY; y += height)
		for (float y = -(float)boundsY * height; y <= (float)boundsY * height; y += height)
		{
			Gizmos.DrawLine (new Vector3(-5000.0f, Mathf.Floor (y / height) * height, 0.0f),
			new Vector3(5000.0f, Mathf.Floor (y / height) * height, 0.0f));
		}

		//for (float x = pos.x - (float)boundsX; x < pos.x + (float)boundsX; x += width)
		for (float x = -(float)boundsX * width; x <= (float)boundsX * width; x += width)
		{
			Gizmos.DrawLine (new Vector3(Mathf.Floor (x / width) * width, -5000.0f, 0.0f),
			                 new Vector3(Mathf.Floor (x / width) * width, 5000.0f, 0.0f));
		}
	}
}
