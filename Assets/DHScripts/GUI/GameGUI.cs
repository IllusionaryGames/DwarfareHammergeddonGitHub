using UnityEngine;
using System.Collections;

public class GameGUI : MonoBehaviour {

	[System.Serializable]
	private class BlockTextureReference
	{
		public int m_iBlockID;
		public Texture2D m_Tex;
	}
	[System.Serializable]
	private class BombTextureReference
	{
		public string m_strBombName;
		public Texture2D m_Tex;
	}

	public GUIStyle CircleHighlight;

	HotSeat m_hotseatref;
	Mouse2D m_mouse2dref;
	Map m_mapref;
	Team m_teamref;

	public Texture2D[] CursorTex;

	public Texture2D[] Background;
	public Texture2D[] Buttons;
	
	//0: Upgrade
	//1: Building Menu

	//public Texture2D[] BlocksButtonsRow1;
	//public Texture2D[] BlocksButtonsRow2;
	[SerializeField]
	private BlockTextureReference[] BlocksButtonsRow1;
	[SerializeField]
	private BlockTextureReference[] BlocksButtonsRow2;
	[SerializeField]
	private BlockTextureReference[] CraftingButtons;
	[SerializeField]
	private BombTextureReference[] ExplosivesButtons;
	//public Texture2D[] CraftingButtons;
	//public Texture2D[] ExplosivesButtons;

	float m_fBackgroundSize = 700;
	float m_fButtonSize = 115;
	bool m_bUpgradeActive = false;
	public bool m_bBuildngActive = false;
	
	public Texture2D[] ButtonUpgrade;

	Vector2 scrollPosition = Vector2.zero;

	bool m_bBlocks = true;
	bool m_bCrafting = false;
	bool m_bExplosives = false;
	private LayerMask LayerGUI;
	private LayerMask LayerBlocks;
	private LayerMask LayerDwarf;
	private LayerMask LayerAir;

	private DwarfChar m_ActiveDwarf;
	private HotSeat m_refHotSeat;

	void Start()
	{
		Screen.showCursor = false;
		
		LayerGUI = LayerMask.NameToLayer("GUITrigger");
		LayerBlocks = LayerMask.NameToLayer("Block");
		LayerDwarf = LayerMask.NameToLayer("Dwarf");
		LayerAir = LayerMask.NameToLayer("Air");

		m_refHotSeat = GameObject.Find ("DwarfareWorldObject").GetComponent<HotSeat>();
		m_mouse2dref = GameObject.Find("DwarfareWorldObject").GetComponent<Mouse2D>();
		m_mapref = GameObject.Find("DwarfareWorldObject").GetComponent<Map>();
		m_teamref = GameObject.Find("DwarfareWorldObject").GetComponent<Team>();
	}


	void Update ()
	{
		//Check if base is Clicked to activate Upgrade Menu
		if (Input.GetKeyUp (KeyCode.Mouse1))
		{
			RaycastHit vHit = new RaycastHit ();
			Ray vRay = Camera.main.ScreenPointToRay (Input.mousePosition);
			
			if (Physics.Raycast (vRay, out vHit, 1000)) { 
				if (vHit.transform.gameObject.layer == LayerGUI)
				{
					m_bUpgradeActive = !m_bUpgradeActive;
				}
			}
		}

		if (Input.GetKeyUp (KeyCode.U))
		{
			m_bUpgradeActive = !m_bUpgradeActive;
		}
		
		m_ActiveDwarf = m_refHotSeat.GetActiveDwarf();
		
		if(m_ActiveDwarf != null)
		{
			if (Input.GetKeyUp (KeyCode.Space))
			{
				m_bBuildngActive = !m_bBuildngActive;
				
			}
		}
	}
	
	void OnGUI ()
	{
		//sets GUI infront of everything
		GUI.depth = 0;

		float m_fScreenWidthCenter = Screen.width / 2;
		float m_fScreenHeightCenter = Screen.height / 2;

		//Upgrade Menu
		if (m_bUpgradeActive == true)
		{	
			GUI.Label (new Rect (m_fScreenWidthCenter - (m_fBackgroundSize * 1.25f / 2f),
								 m_fScreenHeightCenter - (m_fBackgroundSize / 3f),
								 m_fBackgroundSize * 1.25f, m_fBackgroundSize * 1.25f),
								 Background[0]);
			
			for (int i = 0; i<ButtonUpgrade.GetLength(0); ++i)
			{
				if (GUI.Button (new Rect (m_fScreenWidthCenter - (m_fBackgroundSize * 1.25f / 2f) + 310,
										  m_fScreenHeightCenter - (m_fBackgroundSize / 4f) + 30 + (i * (m_fButtonSize)),
										  m_fButtonSize, m_fButtonSize),
										  ButtonUpgrade[i],GUIStyle.none))
				{
				}
			}
		}
		//Building Menu
		if (m_bBuildngActive == true) 
		{
			GUI.Label (new Rect (m_fScreenWidthCenter - (m_fBackgroundSize * 0.9f / 2f),
								 m_fScreenHeightCenter - (m_fBackgroundSize * 0.9f / 2f),
								 m_fBackgroundSize * 0.9f, m_fBackgroundSize * 0.9f), Background[1]);
								 
			GUI.Label (new Rect (m_fScreenWidthCenter - (m_fButtonSize / 2f),
								 m_fScreenHeightCenter + (m_fBackgroundSize / 4f),
								 m_fButtonSize, m_fButtonSize), Background[2]);

			for (int i = 0; i<Buttons.GetLength(0) -1; ++i)
			{
				int k = i;
				if(m_ActiveDwarf.byDwarfType == 2)
				{
					if(i == 2)
						k = 3;
				}
				if (GUI.Button (new Rect (m_fScreenWidthCenter - m_fButtonSize * 1.75f + (i * (m_fButtonSize*1.15f)),
										  m_fScreenHeightCenter - m_fBackgroundSize / 3f, m_fButtonSize * 1.15f, m_fButtonSize * 1.15f/1.5f),
										  Buttons[k], GUIStyle.none))
				{
					switch (i)
					{
						//Blocks tab
					case 0:
							m_bBlocks = true;
							if(m_bCrafting)
								m_bCrafting = !m_bCrafting;
							if(m_bExplosives)
								m_bExplosives = !m_bExplosives;
						break;
						//Crafting tab
					case 1:
							m_bCrafting = true;
							if(m_bBlocks)
								m_bBlocks = !m_bBlocks;
							if(m_bExplosives)
								m_bExplosives = !m_bExplosives;
						break;
						//Explosives tab
					case 2:
						if(m_ActiveDwarf.byDwarfType == 2)
						{
							m_bExplosives = true;
							if(m_bBlocks)
								m_bBlocks = !m_bBlocks;
							if(m_bCrafting)
								m_bCrafting = !m_bCrafting;
						}
							else
							m_bExplosives = false;
						break;
						
					}

				}
			}
			if(m_bBlocks)
			{
				m_bExplosives = false;
				scrollPosition = GUI.BeginScrollView (
									 new Rect (m_fScreenWidthCenter - m_fButtonSize,
										 m_fButtonSize / 1.5f +  m_fScreenHeightCenter - m_fBackgroundSize / 3.4f,m_fBackgroundSize * 0.4f,
										 m_fBackgroundSize * 0.4f), scrollPosition,
									 new Rect (0, 0,m_fBackgroundSize * 0.3f, BlocksButtonsRow1.GetLength (0) * m_fButtonSize));
								 
				for (int i = 0; i<BlocksButtonsRow1.GetLength(0); ++i)
				{
					if (GUI.Button (
						new Rect (0, 0 + (i * (m_fButtonSize)), m_fButtonSize, m_fButtonSize),
						BlocksButtonsRow1[i].m_Tex, CircleHighlight))
					{
						m_ActiveDwarf.iSelectedBlock = BlocksButtonsRow1[i].m_iBlockID;
						m_ActiveDwarf.strSelectedBombtype = null;
					}
				}
				for (int i = 0; i<BlocksButtonsRow2.GetLength(0); ++i) {
					if (GUI.Button (
						new Rect (0+m_fButtonSize, 0 + (i * (m_fButtonSize)), m_fButtonSize, m_fButtonSize),
						BlocksButtonsRow2[i].m_Tex, CircleHighlight))
					{
						m_ActiveDwarf.iSelectedBlock = BlocksButtonsRow2[i].m_iBlockID;
						m_ActiveDwarf.strSelectedBombtype = null;
					}
				}
				GUI.EndScrollView ();
			}
			if(m_bCrafting)
			{
				for (int i = 0; i<CraftingButtons.GetLength(0); ++i)
				{
					if (GUI.Button (new Rect (m_fScreenWidthCenter - m_fButtonSize,
											  m_fButtonSize / 1.5f + m_fScreenHeightCenter - m_fBackgroundSize / 3.4f + (i * (m_fButtonSize)),
											  m_fButtonSize, m_fButtonSize),
											  CraftingButtons[i].m_Tex, CircleHighlight))
					{
						m_ActiveDwarf.iSelectedBlock = CraftingButtons[i].m_iBlockID;
						m_ActiveDwarf.strSelectedBombtype = null;
					}
				}
			}
			if(m_bExplosives)
			{
				if(m_ActiveDwarf.byDwarfType == 2)
				{
					for (int i = 0; i<ExplosivesButtons.GetLength(0); ++i)
					{
						if (GUI.Button (new Rect (m_fScreenWidthCenter - m_fButtonSize,
												  m_fButtonSize / 1.5f + m_fScreenHeightCenter - m_fBackgroundSize / 3.4f + (i * (m_fButtonSize)),
												  m_fButtonSize, m_fButtonSize),
												  ExplosivesButtons[i].m_Tex, CircleHighlight))
						{
							m_ActiveDwarf.strSelectedBombtype = ExplosivesButtons[i].m_strBombName;
						}
					}
				}
				else
				{
					m_bBlocks = true;
				}
			}
		}
		
		Vector3 mousePos = Input.mousePosition;
		Rect pos = new Rect (mousePos.x, Screen.height - 5 - mousePos.y, CursorTex [0].width / 12, CursorTex [0].height / 12);
		DwarfChar TempChar = m_teamref.GetDwarfCharByPosition((int)m_mouse2dref.HitGridIntX, (int)m_mouse2dref.HitGridIntY);
		
		if(TempChar != null && m_ActiveDwarf != null && TempChar.iTeamID != m_ActiveDwarf.iTeamID)
			GUI.Label (pos, CursorTex [3]);
		else if(m_mapref.IsRecoverable((int)m_mouse2dref.HitGridIntX, (int)m_mouse2dref.HitGridIntY))
			GUI.Label (pos, CursorTex [2]);
		else if(m_mapref.IsPassable((int)m_mouse2dref.HitGridIntX, (int)m_mouse2dref.HitGridIntY))
			GUI.Label (pos, CursorTex [4]);
		else 
		{
			if (Input.GetKey (KeyCode.Mouse0))
			{
				GUI.Label (pos, CursorTex [1]);
			}
			else
			{
				GUI.Label (pos, CursorTex [0]);
			}
		}
	}
}
