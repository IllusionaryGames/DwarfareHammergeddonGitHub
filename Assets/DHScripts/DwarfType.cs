using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DwarfType : MonoBehaviour 
{

	public Material matMinerTeam2;
	public Material matWarriorTeam2;
	public Material matDemolitionTeam2;


	[System.Serializable]
	public class m_struDwarf
	{
		public string strCategory; 		 // digger, warrior, demolition_expert
		public GameObject goDwarfType;
		public GameObject goDistantWeapon;

		// Damage
		public int iHealthPoints;  		 // HP
		public int iMeleeDamage;		
		public int iDistanceDamage;
		public int iFallDamagePerBlock;
		public int iAdditionalBuildingDamage; // only demolition_expert

		public float fDigTime;	
	}

	public m_struDwarf[] m_arrDwarfTypes;

	public m_struDwarf GetDwarfTypeByGameobject(GameObject _goDwarfType)
	{
		for (int i = 0; i < m_arrDwarfTypes.GetLength(0); i++)
		{
			if(m_arrDwarfTypes[i].goDwarfType == _goDwarfType)
			{
				return m_arrDwarfTypes[i];
			}
		}
		return null;
	}

	public m_struDwarf GetDwarfTypeByName(string _strCategory)
	{
		for(int i = 0; i < m_arrDwarfTypes.GetLength(0); i ++)
		{
			if(m_arrDwarfTypes[i].strCategory == _strCategory)
			{
				return m_arrDwarfTypes[i];
			}
		}
		return null;
	}

	public m_struDwarf getDwarfTypeByBlockTypeID(int _blockTypeID)
	{
		m_struDwarf CurrentDwarf = new m_struDwarf();
		switch (_blockTypeID)
		{
		case 74:
			
			for (int r = 0; r < m_arrDwarfTypes.GetLength(0); r++)
			{
				if(m_arrDwarfTypes[r].strCategory == "digger")
				{
					CurrentDwarf = m_arrDwarfTypes[r];
					return CurrentDwarf ;
				}			
			}
			break;
			
			
		case 75:
			
			for (int r = 0; r < m_arrDwarfTypes.GetLength(0); r++)
			{
				if(m_arrDwarfTypes[r].strCategory == "warrior")
				{
					CurrentDwarf = m_arrDwarfTypes[r];
					return CurrentDwarf ;
				}
			}
			
			break;
		case 76:
			
			for (int r = 0; r < m_arrDwarfTypes.GetLength(0); r++)
			{
				if(m_arrDwarfTypes[r].strCategory == "demolition_expert")
				{
					CurrentDwarf = m_arrDwarfTypes[r];
					return CurrentDwarf ;
				}											
			}											
			break;									
		case 80:
			
			for (int r = 0; r < m_arrDwarfTypes.GetLength(0); r++)
			{
				if(m_arrDwarfTypes[r].strCategory == "digger")
				{
					CurrentDwarf = m_arrDwarfTypes[r];
					return CurrentDwarf ;
				}
			}
			break;
		case 81:
			
			for (int r = 0; r < m_arrDwarfTypes.GetLength(0); r++)
			{
				
				if(m_arrDwarfTypes[r].strCategory == "warrior")
				{
					CurrentDwarf = m_arrDwarfTypes[r];
					return CurrentDwarf ;
				}				
			}
			break;
		case 82:
			
			for (int r = 0; r < m_arrDwarfTypes.GetLength(0); r++)
			{
				
				if(m_arrDwarfTypes[r].strCategory == "demolition_expert")
				{
					CurrentDwarf = m_arrDwarfTypes[r];
					return CurrentDwarf ;
				}									
			}
			break;
		default:
			return null;
		}
		return CurrentDwarf ;
	}
}
