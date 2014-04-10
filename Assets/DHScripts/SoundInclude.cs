using UnityEngine;
using System.Collections;

public class SoundInclude : MonoBehaviour
{
	public int m_iSoundPeriod;

	public AudioClip[] m_Negations;
	public AudioClip[] m_Confirmation;

	public AudioClip GetRandomNegation()
	{
		return m_Negations[Random.Range(0, m_Negations.Length)];
	}

	public AudioClip GetRandomConfirmation()
	{
		return m_Confirmation[Random.Range (0, m_Confirmation.Length)];
	}
}