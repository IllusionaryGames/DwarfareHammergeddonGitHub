using UnityEngine;
using System.Collections;

[System.Serializable]
public class BombDefinition
{
	public string Name;

	public int SalpeterCost;
	public int CoalCost;
	public int IronCost;

	public float Damage;

	public int HardnessDegree;

	public string ExplosionName;

	public GameObject BombPrefab;
}

public class BombConfigurator : MonoBehaviour
{
	public BombDefinition[] m_arrBombTypes;

	public BombDefinition GetBombtypeByName(string strName)
	{
		if (!string.IsNullOrEmpty(strName))
		{
			for (int i = 0; i < m_arrBombTypes.Length; ++i)
			{
				if (m_arrBombTypes[i].Name.Equals(strName))
				{
					return m_arrBombTypes[i];
				}
			}
		}
		else
		{
			Debug.LogWarning ("Passed bombdefinition name is null or empty");
		}

		return null;
	}
}