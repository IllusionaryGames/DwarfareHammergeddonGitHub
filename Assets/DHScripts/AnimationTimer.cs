using UnityEngine;
using System.Collections;

[System.Serializable]
public class AnimDef
{
	public string AnimationName;
	public float AnimationTime;
}

public class AnimationTimer : MonoBehaviour
{
	public AnimDef[] m_AnimTimes;

	public float GetAnimTimeByString(string strName)
	{
		for (int i = 0; i < m_AnimTimes.Length; ++i)
		{
			if (m_AnimTimes[i].AnimationName.Equals(strName))
				return m_AnimTimes[i].AnimationTime;
		}

		return -1f;
	}
}
