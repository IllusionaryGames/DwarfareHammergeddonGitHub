using UnityEngine;
using System.Collections;


public class Lightning : MonoBehaviour {


	public GameObject CamGameObject;
	float CamPosition;
	public float m_fMaxDarkness;

	// Update is called once per frame
	void Update () {
	
		CamPosition = CamGameObject.transform.position.y;

		gameObject.light.intensity = ((CamPosition * 0.03f / 2)*(CamPosition * 0.03f / 2)) - 0.5f;

		if (gameObject.light.intensity >= 0.5f) 
		{
			gameObject.light.intensity = 0.5f;
		}
		if (gameObject.light.intensity <= m_fMaxDarkness) 
		{
			gameObject.light.intensity = m_fMaxDarkness;
		}

	}
}
