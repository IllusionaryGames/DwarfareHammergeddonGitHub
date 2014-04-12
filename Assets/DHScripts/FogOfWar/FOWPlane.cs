using UnityEngine;
using System.Collections;

public class FOWPlane : MonoBehaviour {

	Transform m_FOWPlane;

	Vector3 m_FOWOffset;

	void Start ()
	{
		m_FOWPlane = GameObject.Find("FOWPlane").transform;
		if (m_FOWPlane != null)
			m_FOWPlane.position = new Vector3(transform.position.x, transform.position.y, m_FOWPlane.position.z);
	}

	void Update ()
	{
		//m_FOWPlane.position = new Vector3(transform.position.x, transform.position.y, m_FOWPlane.position.z);
	}
}
