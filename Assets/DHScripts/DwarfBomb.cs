using UnityEngine;
using System.Collections;

public class DwarfBomb : MonoBehaviour
{
	private BombDefinition m_BombType;

	public int iIndexX;
	public int iIndexY;

	private int m_iUpgradeLevel;

	private Map m_Map;
	private Explosion m_Explosion;
	private Team.struTeam m_refTeam;

	private HotSeat m_HotSeatRef;

	public void Initialize(BombDefinition bombDef, int upgradeLevel, Team.struTeam refTeam, Explosion refExplosion)
	{
		m_BombType = bombDef;

		Vector2 fIndexPos = Map.WorldToIndex(transform.position);

		iIndexX = (int)fIndexPos.x;
		iIndexY = (int)fIndexPos.y;

		m_iUpgradeLevel = upgradeLevel;

		m_Explosion = refExplosion;
		m_refTeam = refTeam;

		m_HotSeatRef = GameObject.Find ("DwarfareWorldObject").GetComponent<HotSeat>();
	}

	void Update()
	{
		if (m_HotSeatRef != null)
		{
			if (m_HotSeatRef.iPlayPhaseTimeCounter == 0)
				Activate();
		}
	}

	public void Activate()
	{
		m_Explosion.detonate(new Vector2(iIndexX, iIndexY), m_BombType.ExplosionName, m_iUpgradeLevel);
		m_refTeam.m_lisBombs.Remove(this);
		Destroy(gameObject);
	}
}