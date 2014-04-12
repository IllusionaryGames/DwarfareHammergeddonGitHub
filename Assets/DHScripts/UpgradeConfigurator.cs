using UnityEngine;
using System.Collections;

[System.Serializable]
public class UpgradeActionDefinition
{
	public int Movement;
	public int Mining;
	public int Explosives;
	public int MeleeAttack;
	public int DistanceAttack;
}

[System.Serializable]
public class UpgradeDefinition
{
	public string Name;
	public int NoOfUpgrades; // how often can this upgrade applied

	public int HardnessDegree;
	public int HardnessDegBuilding;
	public float HealthPoints;
	public float HpRegeneration;
	public float MeleeDamage;
	public float DistanceDamage;
	public float WalkingSpeed;
	public int Yield;
	public int SightRadius;
	public string[] Requirements;
	public UpgradeActionDefinition ActionDef;
}

public class UpgradeConfigurator : MonoBehaviour
{
	public UpgradeDefinition[] m_arrUpgradeDefs;

	private Team m_refTeam;

	void Start()
	{
		m_refTeam = GameObject.Find ("DwarfareWorldObject").GetComponent<Team>();
	}

	public void ApplyUpgrade(int iTeamID, string strUpgradeName)
	{
		UpgradeDefinition selectedDef = new UpgradeDefinition();

		for (int i = 0; i < m_arrUpgradeDefs.Length; ++i)
		{
			if (m_arrUpgradeDefs[i].Name.Equals(strUpgradeName))
			{
			    selectedDef = m_arrUpgradeDefs[i];
			}
		}
		
		if (selectedDef == null)
		{
			Debug.LogWarning ("Upgrade with name " + strUpgradeName + " not found.");
			return;
		}
		
		Team.struTeam tempStruTeam = m_refTeam.m_arrTeams[iTeamID - 1];
		
		bool bRequirementsMet = true;

		if (tempStruTeam.dicUpgradesTeam[strUpgradeName] >= selectedDef.NoOfUpgrades)
		{
			Debug.Log ("Upgrade already researched.");
			return;
		}
//		for (int i = 0; i < selectedDef.Requirements.Length; ++i)
//		{
//			if (tempStruTeam.dicUpgradesTeam[selectedDef.Requirements[i]] < selectedDef.NoOfUpgrades)
//			{
//				bRequirementsMet = false;
//				break;
//			}
//		}
		if (bRequirementsMet)
		{
			tempStruTeam.dicUpgradesTeam[strUpgradeName]++;

			tempStruTeam.iHardnessUpgrade += selectedDef.HardnessDegree;
//			tempStruTeam. = HardnessDegBuilding
			tempStruTeam.fHitPointsUpgrade += selectedDef.HealthPoints;
			//tempStruTeam.h = HpRegeneration;
//			tempStruTeam.iDamageMeleeUpgrade += selectedDef.MeleeDamage;
//			tempStruTeam.iDamageDistantWeaponUpgrade += selectedDef.DistanceDamage;
//			tempStruTeam.fWalkingSpeedMiner...Warrior...Demolition
			tempStruTeam.iRevenueUpgrade += selectedDef.Yield;
			tempStruTeam.iSightRadiusUpgrade += selectedDef.SightRadius;

			tempStruTeam.iActionPointsMoveUpgrade += selectedDef.ActionDef.Movement;
			tempStruTeam.iActionPointsDigUpgrade += selectedDef.ActionDef.Mining;
			tempStruTeam.iActionPointsDemolitionBlockUpgrade += selectedDef.ActionDef.Explosives;
			tempStruTeam.iActionPointsMeleeWeaponUpgrade += selectedDef.ActionDef.MeleeAttack;
			//tempStruTeam.ac = DistanceWeapon;
			//tempStruTeam.iActionPointsBuildUpgrade
			//tempStruTeam.iActionpointsTeam

			m_refTeam.m_arrTeams[iTeamID - 1] = tempStruTeam;

			foreach (DwarfChar dchar in tempStruTeam.lisDwarfs)
			{
				dchar.iPickAxeHardness += selectedDef.HardnessDegree;
				dchar.fHealth += selectedDef.HealthPoints;
				dchar.fHealthRegeneration += selectedDef.HpRegeneration;
				dchar.fMeleeDmg += selectedDef.MeleeDamage;
				dchar.DistanceDamage += selectedDef.DistanceDamage;
				if (selectedDef.WalkingSpeed > 0)
					dchar.WalkingSpeed *= selectedDef.WalkingSpeed;
				dchar.iYield += selectedDef.Yield;
				dchar.iSightRadius += selectedDef.SightRadius;
			}
		}

	}
}
