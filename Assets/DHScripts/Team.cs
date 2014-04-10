using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Team : MonoBehaviour {

	// references
	private UpgradeConfigurator refUpgradeConfigurator;
	private HotSeat refHotSeat;
	private DwarfType refDwarfType;
	
	public int m_iTeamQuantity; // standard 2
	public int m_iActionpoints; // standard 100
	public float m_fHealthPointsBaseAtStart; 

	public RessourceInventory m_Inventory;

	public Dictionary<int, bool> m_dicTeamIsDead;

	public struct struTeam 
	{
		// TeamProperties
		public string strTeamname;
		public int iActionpointsTeam;
		public int iTeamID;
		public Dictionary<string, int> dicUpgradesTeam; 
		public RessourceInventory m_Inventory;
		public List<DwarfChar> lisDwarfs;
		public List<DwarfBomb> m_lisBombs;

		// UPGRADES
		// Damage
		public float fDamageMeleeUpgrade;
		public float fDamageDistantWeaponUpgrade;

		// Actionpoints
		public int iActionPointsMoveUpgrade;
		public int iActionPointsDemolitionBlockUpgrade;
		public int iActionPointsDigUpgrade;
		public int iActionPointsMeleeWeaponUpgrade;
		public int iActionPointsBuildUpgrade;

		// HitPoints
		public float fHitPointsUpgrade;
		public float[] arr_fHealthPointsDifferentDwarfTypes;

		// Base
		public float fHitPointsBase;
		public float fHealthPointsBaseMaxForGUI;

		public int iUpgradeStage; // 0 - 3

		// Treasure
		public bool bTeamHasTreasure;
		public Vector2 vec2TreasurePositionAtStart; 

		// Hardness
		public int iHardnessUpgrade;

		// Revenue
		public int iRevenueUpgrade;

		// Sight
		public int iSightRadiusUpgrade;

		// VictoryConditions
		public bool bVictoryBaseDestruction;
		public bool bVictoryTreasureObjectHunt;
		public bool bVictoryEnemyDestruction;

		public Vector2 vec2BasePositionInGrid;
	}
	
	public struTeam[] m_arrTeams;
	
	void Awake()
	{
		refUpgradeConfigurator = GetComponent<UpgradeConfigurator>();
		refHotSeat = GetComponent<HotSeat>();
		refDwarfType = GetComponent<DwarfType>();
		m_arrTeams = new struTeam[m_iTeamQuantity];
		m_dicTeamIsDead = new Dictionary<int, bool>();
	}

	void Start()
	{	

		// Initialize
		for(int j = 0; j < m_iTeamQuantity; j ++)
		{
			// initialize Teams
			m_arrTeams[j].iActionpointsTeam = m_iActionpoints;
			m_arrTeams[j].fHitPointsBase = m_fHealthPointsBaseAtStart;
			m_arrTeams[j].fHealthPointsBaseMaxForGUI = m_fHealthPointsBaseAtStart;
			m_arrTeams[j].bTeamHasTreasure = false;
			m_arrTeams[j].iTeamID = j + 1;
			m_arrTeams[j].dicUpgradesTeam = new Dictionary<string, int>();
			m_arrTeams[j].m_Inventory = new RessourceInventory(gameObject.GetComponent<Map>());
			m_arrTeams[j].lisDwarfs = new List<DwarfChar>();
			m_arrTeams[j].m_lisBombs = new List<DwarfBomb>();
			m_arrTeams[j].vec2TreasurePositionAtStart = new Vector2(0,0);
			m_arrTeams[j].arr_fHealthPointsDifferentDwarfTypes = new float[refDwarfType.m_arrDwarfTypes.Length];
			for (int k = 0; k < m_arrTeams[j].arr_fHealthPointsDifferentDwarfTypes.Length; k++)
			{
				m_arrTeams[j].arr_fHealthPointsDifferentDwarfTypes[k] = refDwarfType.m_arrDwarfTypes[k].iHealthPoints;
			}

			// Add all Upgrades
			for (int i = 0; i < refUpgradeConfigurator.m_arrUpgradeDefs.Length; i ++)
			{				
				m_arrTeams[j].dicUpgradesTeam.Add(refUpgradeConfigurator.m_arrUpgradeDefs[i].Name, 0);
			}
			
			m_dicTeamIsDead.Add(j, false);
		}
		// All Victories to false
		for (int i = 0; i < m_arrTeams.Length; i ++)
		{
			m_arrTeams[i].bVictoryBaseDestruction = false;
			m_arrTeams[i].bVictoryTreasureObjectHunt = false;
			m_arrTeams[i].bVictoryEnemyDestruction = false;
		}
	
		m_Inventory = new RessourceInventory(gameObject.GetComponent<Map>());
	}

	void Update()
	{

		// TEST
//		if(Input.GetKeyDown("q"))
//		{
//			DamageBase(-500f, 2);
//			Debug.Log("Team: " + m_arrTeams[1].fHitPointsBase );
//		}


		if(Input.GetKeyDown("t"))
		{
			Debug.Log("Team: Treasureposition " + m_arrTeams[1].vec2TreasurePositionAtStart);
		}

		if(Input.GetKeyDown("h")) // h for health
		{
			Debug.Log("GetBaseHealthPointsFromActiveTeam() " +  GetBaseHealthPointsFromActiveTeam());
			//Debug.Log("Team: GetMaxHealthPointsDwarfForGUI(1, 1) " + GetMaxHealthPointsDwarfForGUI(2, 2));
		}
		// TEST END
	}
	

	public float GetMaxHealthPointsDwarfForGUI (int _iID, int _iTeam)
	{
		if(_iID < m_arrTeams[_iTeam - 1].lisDwarfs.Count)
		{
			return m_arrTeams[_iTeam - 1].arr_fHealthPointsDifferentDwarfTypes[m_arrTeams[_iTeam - 1].lisDwarfs[_iID].byDwarfType];
		}
		else
		{
			//Debug.Log("Team: Dwarf is dead ");
			return 100f;
		}
	}

	/* public float GetMaxHealtPointsDwarfForGUI(int _iID, int _iTeam)
	{
		// if(m_arrTeams[_iTeam - 1].lisDwarfs[_iID].Type == refDwarfType.m_arrDwarfTypes[0])
		{
		
		}
	}*/

	/*public float GetMaxDwarfHealth(int _iID, int _iTeam)
	{
		DwarfType CurrentDwarfType = m_arrTeams[_iTeam - 1].lisDwarfs[_iID].Type;
		for(int i = 0; i < refDwarfType.m_arrDwarfTypes.Length; i ++)
		{
			// CurrentDwarfType.
		    if(CurrentDwarfType.m_arrDwarfTypes[i].strCategory == "digger")
			{
				return m_arrTeams[_iTeam - 1].fHealthPointsMinerMaxForGUI;
			}
			if(CurrentDwarfType.m_arrDwarfTypes[i].strCategory == "warrior")
			{
				return m_arrTeams[_iTeam - 1].fHealthPointsWarriorMaxForGUI;
			}
			if(CurrentDwarfType.m_arrDwarfTypes[i].strCategory == "demolition_expert")
			{
				return m_arrTeams[_iTeam - 1].fHealthPointsDemolitionExpertMaxForGUI;
			}
		}
		return -1;
	}*/

	/*public void GetDwarfTypeByDwarfChar()
	{
		DwarfChar CurrentDwarf = GetDwarfCharByIDAndTeamID(1,1);
	}*/

	public float GetBaseHealthPointsFromActiveTeam()
	{
		if(refHotSeat.m_iActiveTeam != 0)
		{
			return m_arrTeams[refHotSeat.m_iActiveTeam - 1].fHealthPointsBaseMaxForGUI;
		}
		return -1;		
	}


	public DwarfChar GetActiveDwarfID()
	{
		for(int i = 0; i < m_arrTeams.Length; i++)
		{
			foreach(DwarfChar iter in m_arrTeams[i].lisDwarfs)
			{
				if(iter.Active)
				{
					return  iter;
				}
			}
		}
		return null;
	}

	public DwarfBomb GetBombAt(int iIndexX, int iIndexY, int iTeamID)
	{
		if (iTeamID - 1 >= m_arrTeams.Length)
			return null;

		foreach (DwarfBomb bomb in m_arrTeams[iTeamID - 1].m_lisBombs)
		{
			if (bomb.iIndexX == iIndexX && bomb.iIndexY == iIndexY)
				return bomb;
		}

		return null;
	}

	public void SaveTreasurePositionAtStart(int _iXGrid, int _iYGrid)
	{
		for (int i = 0; i < m_arrTeams.Length; i ++)
		{
			m_arrTeams[i].vec2TreasurePositionAtStart = new Vector2(_iXGrid, _iYGrid);
		}
	}

	public void HasTreasure(bool _bHasTreasure, int _iTeam)
	{
		m_arrTeams[_iTeam - 1].bTeamHasTreasure = _bHasTreasure;
	}

	public DwarfChar GetDwarfCharByPosition(int _X, int _Y)
	{
		for (int i = 0 ; i < m_arrTeams.Length; i ++)
		{
			foreach(DwarfChar iter in m_arrTeams[i].lisDwarfs)
			{
				if (iter.iIndexPosX == _X && iter.iIndexPosY == _Y)
				{
					return iter;
				}
			}
		}
		return null;
	}

	public void DeleteDwarf(int _iID, int _iTeam)
	{
		if(_iTeam < 1 || _iTeam > m_arrTeams.Length)
			return;
		for (int j = 0; j <m_arrTeams[_iTeam - 1].lisDwarfs.Count; j++)
		{
			//Debug.Log("Map: deleteDwarf ");
			if(m_arrTeams[_iTeam - 1].lisDwarfs[j].iID == _iID)
			{
				GameObject goCurrentDwarf = m_arrTeams[_iTeam - 1].lisDwarfs[j].gameObject;
				m_arrTeams[_iTeam - 1].lisDwarfs.RemoveAt(j);
				Destroy(goCurrentDwarf);

				if (m_arrTeams[_iTeam - 1].lisDwarfs.Count == 0)
				{
					Debug.Log ("Team " + _iTeam + " got wiped");
					m_dicTeamIsDead[_iTeam] = true;

					// specialized for ONLY two teams:
					m_arrTeams[Mathf.Abs (_iTeam - 1)].bVictoryEnemyDestruction = true;
				}
			}
		}
	}

	public DwarfChar GetDwarfCharByIDAndTeamID(int _iID, int _iTeam)
	{
		foreach(DwarfChar iter in m_arrTeams[_iTeam - 1].lisDwarfs)
		{
			if(iter.iID == _iID)
			{
				return iter;
			}
		}
		return null;
	}

	public void DamageBase(float _fDamageHitPoints, int _iTeam)
	{
			m_arrTeams[_iTeam - 1].fHitPointsBase += _fDamageHitPoints;

		for (int i = 0; i < m_arrTeams.Length; i ++)
		{
			if(m_arrTeams[i].fHitPointsBase <= 0)
			{
				VictoryBaseDestruction(i + 1);
			}
		}
	}

	public void SetActionpointsToStartActionpointsForAllTeams()
	{
		for (int i = 0; i < m_arrTeams.Length; i++)
		{
			m_arrTeams[i].iActionpointsTeam = m_iActionpoints;
		}
	}

	void VictoryBaseDestruction(int _iTeam) 
	{
		m_arrTeams[_iTeam - 1].bVictoryBaseDestruction = true; // checked in ongui HUDsetup
	}

	void AdjustActionpointsInTeam (int _iActionpoints, int _iTeam)
	{
		m_arrTeams[_iTeam - 1].iActionpointsTeam += _iActionpoints;
	}

	void SetUpgradeActive (string _strUpgradeName, int _iTeam) // Team starts at 1 not 0
	{
		m_arrTeams[_iTeam - 1].dicUpgradesTeam[_strUpgradeName] = 1;	
	}

	void SetUpgradeInactive (string _strUpgradeName, int _iTeam) // Team starts at 1 not 0
	{
		m_arrTeams[_iTeam - 1].dicUpgradesTeam[_strUpgradeName] = 0;	
	}
}
