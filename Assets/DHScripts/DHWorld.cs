using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class DHWorld : MonoBehaviour
{
	public static float m_fWidth = 1.6f;
	public static float m_fHeight = 1.6f;

	public static int m_iBoundsX = 32;
	public static int m_iBoundsY = 32;

	private LevelXMLSaver m_LvlXMLSaver;

	public GameObject DebugCube;


	// TEST TEST TEST
	Pathfinder pathing;
	// --------------

	[SerializeField]
	public int[,] m_arriWorld = new int[2 * m_iBoundsX, 2 * m_iBoundsY];

	[SerializeField]
	public int[,] m_arriOrigins = new int[2 * m_iBoundsX, 2 * m_iBoundsY];

	void Awake()
	{
		m_LvlXMLSaver = LevelXMLSaver.Load (Path.Combine (Application.persistentDataPath, "level.xml"));
		m_arriWorld = m_LvlXMLSaver.Extract();
	}

	void Start()
	{
		/*
		pathing = new Pathfinder();
		Debug.Log (pathing.StartPathing(1, 26, 48, 23, m_arriWorld));

		Vector2 next = new Vector2(1, 26);
		while ((next.x != 48 || next.y != 23) && next.x != -1)
		{
			next = pathing.GetNextPosition(next);
			Instantiate(DebugCube, next + new Vector2(0.5f, 0.5f) - new Vector2(m_iBoundsX, m_iBoundsY), Quaternion.identity);
		}
		*/
	}

	public static Vector2 WorldToIndex(Vector2 vec2WorldPos)
	{
		return Vector2.zero;
	}

	public static Vector2 IndexToWorldOffset(int iIndexX, int iIndexY)
	{
		return new Vector2(iIndexX * m_fWidth, iIndexY * m_fHeight);
	}

	public static Vector2 IndexToWorldNoOffset(int iIndexX, int iIndexY)
	{
		return new Vector2(
			(iIndexX - m_fWidth) * m_fWidth + m_fWidth / 2.0f,
			(iIndexY - m_fHeight) * m_fHeight + m_fHeight / 2.0f);
	}
}
