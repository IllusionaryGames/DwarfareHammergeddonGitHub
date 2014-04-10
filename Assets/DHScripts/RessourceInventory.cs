using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RessourceInventory
{
	private int[,] m_arriRessources;

	public RessourceInventory(Map map)
	{
		// [ressource][0] = typeid
		// [ressource][1] = ressourcecount

		m_arriRessources = new int[map.refLoadLevelTexture.BlockTypes.GetLength(0), 2];

		for (int i = 0; i < map.refLoadLevelTexture.BlockTypes.GetLength(0); ++i)
		{
			m_arriRessources[i, 0] = map.refLoadLevelTexture.BlockTypes[i].iTypeID;
			m_arriRessources[i, 1] = 0;
		}
	}

	public void AddRessPoint(int iRessType, int iAmount)
	{
		for (int i = 0; i < m_arriRessources.GetLength(0); ++i)
		{
			if (m_arriRessources[i, 0] == iRessType)
			{
				m_arriRessources[i, 1] += iAmount;
			}
		}
	}

	public int GetRessourceCount(int iRessType)
	{
		for (int i = 0; i < m_arriRessources.GetLength(0); ++i)
		{
			if (m_arriRessources[i, 0] == iRessType)
			{
				return m_arriRessources[i, 1];
			}
		}

		return -1;
	}
}
