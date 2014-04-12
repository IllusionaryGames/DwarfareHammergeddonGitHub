using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
	int m_iIndexX;
	int m_iIndexY;

	int m_iIndexCollisionX;
	int m_iIndexCollisionY;

	float m_fSpeed;

	Vector3 m_vec2Dir;

	GameObject m_goTarget;

	DwarfChar m_Parent;

	public void Initialize(Vector2 vec2Dir, float fSpeed, DwarfChar parent, GameObject target)
	{
		m_fSpeed = fSpeed;

		m_vec2Dir = vec2Dir;

		m_Parent = parent;

		m_goTarget = target;

		Vector2 targetIndex = Map.WorldToIndex((Vector2)target.transform.position);
		m_iIndexCollisionX = (int)targetIndex.x;
		m_iIndexCollisionY = (int)targetIndex.y;
	}

	void Update()
	{
		transform.Translate(m_vec2Dir * m_fSpeed);

		Vector2 iIndexPos = Map.WorldToIndex((Vector2)transform.position);

		m_iIndexX = (int)iIndexPos.x;
		m_iIndexY = (int)iIndexPos.y;

		if (m_iIndexX == m_iIndexCollisionX && m_iIndexY == m_iIndexCollisionY)
		{
			DwarfChar opponent = m_goTarget.GetComponent<DwarfChar>();

			if (opponent != null)
			{
				opponent.fHealth -= m_Parent.DistanceDamage;
			}

			Destroy(this.gameObject);
		}
	}
}
