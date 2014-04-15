using UnityEngine;
using System.Collections;

public class HUDSetup : MonoBehaviour
{
	HotSeat m_hotseatref;
	DwarfChar m_dwarfcharref;
	DwarfType m_refDwarfType;
	Team m_teamref;
	GUISetup m_GUISetupref;

	//Textures for the HUD. Index follows
	public Texture2D[] HUDTexture;
	//TIMER:
	//0: Timer Background
	//1: Left timer bar
	//2: Right timer bar
	//3: Timer Border
	//4: Avatar circle
	//5: Bar GUI
	//6: Health Bar
	//7: AP Bar
	//8: Left RED timer bar
	//9: Right RED timer bar
	//10: Chat
	//11...: Block Icons

	public Texture2D[] Buttons;
	public Texture2D[] Avatars;

	public Texture2D[] Background;
	public Texture2D[] Label;
	public Texture2D[] ButtonsOptions;
	public Texture2D[] ButtonsOptions2;

	public Texture2D[] ClassIcons;

	public Texture APTexture;

	float m_fScreenWidthCenter;
	float m_fScreenHeightCenter;

	float ScreenWidth = (Screen.width / 2);
	float ScreenHeight = (Screen.height / 2);
	float BackgroundWidth;
	float BackgroundHeight;
	float ImageWidth;
	float ImageHeight;
	float LabelWidth = 0.62f;
	float LabelHeight = 0.15f;
	int GUIID = 0;

	public float h_SliderValue;
	bool m_bShowSlider = false;
	
	float m_fTimerSize = 500;
	float m_fAvatarSize = 150;

	public GUIStyle HUDStyle;
	public GUIStyle PauseStyle;
	public GUIStyle ReadyStyle;
	public GUIStyle ResourceStyle;

	public int m_iTimerstage = 2;
	bool m_bInit = false;
	float m_fTimestamp;

	public float m_fScroll = 1;
	public float m_fHealthScrollActive = 1;
	public float m_fHealthScroll1 = 1;
	public float m_fHealthScroll2 = 1;
	public float m_fHealthScroll3 = 1;
	public float m_fHealthScroll4 = 1;
	public float[] m_arrfHealthScroll; 

	public float m_fAPScroll = 1;
	public float m_fBaseHealthScroll = 1;
	float Timer = 90;
	float m_fReadyTimer = 10;
	public bool m_bIsTimeRunning = false;
	float m_ftimeLeft;
	string m_sTimerDisplay = "";
	float m_fCountDownOffset = 0f;

	//Resources
	float m_iDirt;
	float m_iStone;
	float m_iIron;
	float m_iCoal;
	float m_iSaltpeter;

	//MaxHealth
	float m_fDwarfHealth;
	float m_fDwarfmaxHealth;
	float m_fDwarf_1_maxHealth;
	float m_fDwarf_2_maxHealth;
	float m_fDwarf_3_maxHealth;
	float m_fDwarf_4_maxHealth;

	float[] m_arrfDwarf_maxHealth;

	int AvatarID;

	float m_fDwarfAP;
	int m_iDwarfmaxAP;
	float m_fBaseHealth;
	float m_fBasemaxHealth;

	public bool m_bChatActive;
	public bool m_bClassChangerActive = false;

	int m_iGameQuit = 0;
	
	public float m_fYOffset;
	public float m_fYOffset2;

	int m_iAvatarID = 0;

	void Awake()
	{
		m_teamref = GameObject.Find("DwarfareWorldObject").GetComponent<Team> ();
		m_refDwarfType = GameObject.Find ("DwarfareWorldObject").GetComponent<DwarfType>();
	}

	void Start()
	{
		//Import Volume
		h_SliderValue = PlayerPrefs.GetFloat ("h_SliderValue");
		
		
		HUDStyle.fontSize = 35;

		m_dwarfcharref = GameObject.Find("DwarfareWorldObject").GetComponent<DwarfChar> ();
		
		m_arrfHealthScroll = new float[4];
		m_arrfDwarf_maxHealth = new float[4];

		m_hotseatref = GameObject.Find("DwarfareWorldObject").GetComponent<HotSeat> ();
		if (m_hotseatref != null) {
			Timer = m_hotseatref.m_iPlayPhaseTime;
			m_fReadyTimer = m_hotseatref.m_iGetReadyTime;
				}
		m_iDwarfmaxAP = m_teamref.m_iActionpoints;
						
	}

	void Update()
	{
		AudioListener.volume = h_SliderValue;

		DwarfChar ActiveDwarf = m_hotseatref.GetActiveDwarf ();

		if (ActiveDwarf == null) 
		{
			m_fDwarfHealth = 0;
			m_fDwarfAP = 0;
			m_fBaseHealth = 0;
		}
		else 
		{
			m_fDwarfHealth = ActiveDwarf.fHealth;
			m_fDwarfAP = m_teamref.m_arrTeams [ActiveDwarf.iTeamID - 1].iActionpointsTeam;
			m_fBaseHealth = m_teamref.m_arrTeams [ActiveDwarf.iTeamID - 1].fHitPointsBase;
			
			for(int i = 0; i < m_arrfDwarf_maxHealth.Length; i++)
			{
				m_arrfDwarf_maxHealth[i] = m_teamref.GetMaxHealthPointsDwarfForGUI(i, m_hotseatref.GetActiveDwarf().iTeamID);
/*
				m_fDwarf_1_maxHealth = m_teamref.GetMaxHealthPointsDwarfForGUI(0,m_hotseatref.GetActiveDwarf().iTeamID);
				m_fDwarf_2_maxHealth = m_teamref.GetMaxHealthPointsDwarfForGUI(1,m_hotseatref.GetActiveDwarf().iTeamID);
				m_fDwarf_3_maxHealth = m_teamref.GetMaxHealthPointsDwarfForGUI(2,m_hotseatref.GetActiveDwarf().iTeamID);
				m_fDwarf_4_maxHealth = m_teamref.GetMaxHealthPointsDwarfForGUI(3,m_hotseatref.GetActiveDwarf().iTeamID);
*/
			}

		}
		
		if(Input.GetKeyUp(KeyCode.Escape))
		{
			Debug.Log("Hit escape");
			m_bIsTimeRunning = !m_bIsTimeRunning;
		}
	}




	void OnGUI () {

		
		DwarfChar ActiveDwarf = m_hotseatref.GetActiveDwarf ();



		if (ActiveDwarf != null)
		{
			m_iDirt = m_teamref.m_arrTeams [ActiveDwarf.iTeamID - 1].m_Inventory.GetRessourceCount (21);
			m_iStone = m_teamref.m_arrTeams [ActiveDwarf.iTeamID - 1].m_Inventory.GetRessourceCount (30);
			m_iIron = m_teamref.m_arrTeams [ActiveDwarf.iTeamID - 1].m_Inventory.GetRessourceCount (31);
			m_iCoal = m_teamref.m_arrTeams [ActiveDwarf.iTeamID - 1].m_Inventory.GetRessourceCount (22);
			m_iSaltpeter = m_teamref.m_arrTeams [ActiveDwarf.iTeamID - 1].m_Inventory.GetRessourceCount (32);
		}
		else
		{
			m_iDirt = 0;
			m_iStone = 0;
			m_iIron = 0;
			m_iCoal = 0;
			m_iSaltpeter = 0;
		}


		//TIMER FUNCTION STARTS HERE
		HUDStyle.fontSize = 35;

		m_fScreenWidthCenter = Screen.width/2;
		m_fScreenHeightCenter = Screen.height/2;

		GUI.Label(new Rect (m_fScreenWidthCenter-(m_fTimerSize/2f),10f,m_fTimerSize,m_fTimerSize),HUDTexture[0]);

		if (m_iTimerstage == 0) {

			HUDStyle.normal.textColor = Color.blue;

			m_ftimeLeft = m_hotseatref.m_iPlayPhaseCounterForGUI;

			if(m_bIsTimeRunning == true)
			{
						if(!m_bInit)
						{
				//m_fTimestamp = Time.time - (Timer - m_ftimeLeft);
				m_bInit = true;
			}
			
			
				if (GUI.Button (new Rect (m_fScreenWidthCenter - 28f, 25f, m_fTimerSize / 9, m_fTimerSize / 9), m_sTimerDisplay, GUIStyle.none)) {
							m_hotseatref.iPlayPhaseTimeCounter = 0;
				//Debug.Log ("found me");
						} else {
							//	m_ftimeLeft = Timer - Time.time + m_fTimestamp;

								m_fScroll = m_ftimeLeft / Timer - 0.1f;
						}
			}
			if(m_bIsTimeRunning == false)
			{
				m_bInit = false;
			}
			GUI.DrawTextureWithTexCoords (new Rect (m_fScreenWidthCenter - 25f, 10f, -m_fTimerSize / 2 * m_fScroll, 85.44f), HUDTexture [1], new Rect (0f, 0f, 1.21f * m_fScroll, 1f));
			GUI.DrawTextureWithTexCoords (new Rect (m_fScreenWidthCenter + 25f, 10f, m_fTimerSize / 2 * m_fScroll, 85.44f), HUDTexture [2], new Rect (0f, 0f, 1.21f * m_fScroll, 1f));
						if (m_ftimeLeft < 0)
								m_ftimeLeft = 0;
						if (m_fScroll < 0)
								m_fScroll = 0;
						if (m_ftimeLeft < 10) {
								m_sTimerDisplay = " ";
								m_fCountDownOffset = 2f;
						}
						if (m_ftimeLeft > 10 && m_ftimeLeft < 20) {
								m_fCountDownOffset = 5;
						}
						if(m_ftimeLeft > 20)
						{
							m_fCountDownOffset =10f;
						}
						if (m_ftimeLeft == 0) {
				//m_fScroll = 1;
				//m_ftimeLeft = m_fReadyTimer;
				m_iTimerstage = 1;
				m_bInit = false;
				m_bIsTimeRunning = true;

						}
			
				}

		if(m_iTimerstage == 1){

			//Get Ready Label
			GUI.Label(new Rect(m_fScreenWidthCenter-(m_fAvatarSize/1.25f),m_fScreenHeightCenter-100,m_fAvatarSize,m_fAvatarSize),"Get Ready!", ReadyStyle);

			HUDStyle.normal.textColor = Color.red;

			m_ftimeLeft = m_hotseatref.m_iGetReadyCounterForGUI;

			if (GUI.Button (new Rect (m_fScreenWidthCenter - 28f, 25f, m_fTimerSize / 9, m_fTimerSize / 9), m_sTimerDisplay, GUIStyle.none)) {
				m_bIsTimeRunning = false;
				m_iTimerstage = 2;
				//Debug.Log ("found me");
			}

			if(m_bIsTimeRunning == true)
			{
				if(!m_bInit)
				{
					//m_fTimestamp = Time.time - (m_fReadyTimer - m_ftimeLeft);
					m_bInit = true;
				}
				//m_ftimeLeft = m_fReadyTimer - Time.time + m_fTimestamp;
				
				m_fScroll = m_ftimeLeft / m_fReadyTimer - 0.1f;
			}
			if(m_bIsTimeRunning == false)
			{
				m_bInit = false;
			}
			GUI.DrawTextureWithTexCoords (new Rect (m_fScreenWidthCenter - 25f, 10f, -m_fTimerSize / 2 * m_fScroll, 85.44f), HUDTexture [8], new Rect (0f, 0f, 1.21f * m_fScroll, 1f));
			GUI.DrawTextureWithTexCoords (new Rect (m_fScreenWidthCenter + 25f, 10f, m_fTimerSize / 2 * m_fScroll, 85.44f), HUDTexture [9], new Rect (0f, 0f, 1.21f * m_fScroll, 1f));
			if (m_fScroll < 0)
				m_fScroll = 0;
			if (m_ftimeLeft < 10) {
				m_sTimerDisplay = " ";
				m_fCountDownOffset = 2f;
			}
			if (m_ftimeLeft > 10 && m_ftimeLeft < 20) {
				m_fCountDownOffset = -3.5f;
			}
			if(m_ftimeLeft > 20)
			{
				m_fCountDownOffset = 0f;
			}
			if(m_ftimeLeft <= 0)
			{
				m_bIsTimeRunning = false;
				m_iTimerstage = 2;
			}
		}
		if (m_iTimerstage == 2) 
		{
			m_bInit = false;
			//m_ftimeLeft = Timer;
			//m_fScroll = 1;
			m_bIsTimeRunning = true;
			m_iTimerstage = 0;
				}

//			GUI.DrawTextureWithTexCoords (new Rect (m_fScreenWidthCenter - 25f, 10f, -m_fTimerSize / 2 * m_fScroll, 85.44f), HUDTexture [8], new Rect (0f, 0f, 1.21f * m_fScroll, 1f));
//			GUI.DrawTextureWithTexCoords (new Rect (m_fScreenWidthCenter + 25f, 10f, m_fTimerSize / 2 * m_fScroll, 85.44f), HUDTexture [9], new Rect (0f, 0f, 1.21f * m_fScroll, 1f));

		GUI.Label(new Rect (m_fScreenWidthCenter-(m_fTimerSize/2),10,m_fTimerSize,m_fTimerSize),HUDTexture[3]);
		GUI.Label(new Rect (m_fScreenWidthCenter-16f-m_fCountDownOffset,35f,m_fTimerSize,m_fTimerSize), m_sTimerDisplay + (int)(m_ftimeLeft),HUDStyle);
			
		
		if(ActiveDwarf != null)
		{
			if(ActiveDwarf.iTeamID == 1)
				AvatarID = ActiveDwarf.byDwarfType+1;
			if(ActiveDwarf.iTeamID == 2)
				AvatarID = ActiveDwarf.byDwarfType+3+1;
		}
		else
		{
			AvatarID = 7;
		}

		//Avatar Rings on either side of the screen
		GUI.Label(new Rect (Screen.width-(m_fAvatarSize),10f,m_fAvatarSize,m_fAvatarSize),Avatars[AvatarID]);
		GUI.Label(new Rect (10f,10f,m_fAvatarSize,m_fAvatarSize),Avatars[0]);
		GUI.Label(new Rect (Screen.width-(m_fAvatarSize),10f,m_fAvatarSize,m_fAvatarSize),HUDTexture[4]);


		//Scroll Math Stuff
		if(ActiveDwarf != null)
		{
		m_fBasemaxHealth = m_teamref.m_arrTeams[ActiveDwarf.iTeamID - 1].fHealthPointsBaseMaxForGUI;
		m_fBaseHealthScroll = m_fBaseHealth/ m_fBasemaxHealth;
		}
		if (ActiveDwarf != null && m_teamref.GetActiveDwarfID() != null)
		{
			m_fHealthScrollActive = m_fDwarfHealth / m_teamref.GetMaxHealthPointsDwarfForGUI(m_teamref.GetActiveDwarfID().iID,m_hotseatref.GetActiveDwarf().iTeamID);
			m_fAPScroll = m_fDwarfAP / 100;
			if(m_teamref)
			{
				for(int i = 0; i < m_teamref.m_arrTeams [ActiveDwarf.iTeamID - 1].lisDwarfs.Count; i++)
				{
					m_arrfHealthScroll[i] = m_teamref.m_arrTeams [ActiveDwarf.iTeamID - 1].lisDwarfs [i].iHealth / m_arrfDwarf_maxHealth[i];
					//Debug.Log(m_arrfHealthScroll[i]);
				}
			}
		}
		else 
		{
			if(m_teamref)
			{
				if(ActiveDwarf)
				{
					for(int i = 0; i < m_teamref.m_arrTeams [ActiveDwarf.iTeamID - 1].lisDwarfs.Count; i++)
					{
						m_arrfHealthScroll[i] = 1;
					}
				}
			}
			/*
			m_fHealthScroll1 = 1;
			m_fHealthScroll2 = 1;
			m_fHealthScroll3 = 1;
			m_fHealthScroll4 = 1;
			*/
		}

		if(m_teamref)
		{
			if(ActiveDwarf)
			{
				for(int i = 0; i < m_teamref.m_arrTeams [ActiveDwarf.iTeamID - 1].lisDwarfs.Count; i++)
				{
					if(m_arrfHealthScroll[i] <= 0)
						m_arrfHealthScroll[i] =0;
				}
			}
		}
		
		/*
		if(m_fHealthScroll1 <= 0)
			m_fHealthScroll1 =0;
		if(m_fHealthScroll2 <= 0)
			m_fHealthScroll2 =0;
		if(m_fHealthScroll3 <= 0)
			m_fHealthScroll3 =0;
		if(m_fHealthScroll4 <= 0)
			m_fHealthScroll4 =0;
			*/

		//MiniAvatars

		int OffsetCounter = 1;
		
		if(ActiveDwarf != null)
		{
			foreach(DwarfChar DChar in m_teamref.m_arrTeams [ActiveDwarf.iTeamID - 1].lisDwarfs)
			{
				OffsetCounter++;
				if(DChar.Active)
					GUI.Label(new Rect (Screen.width-m_fAvatarSize+10,m_fAvatarSize*1.75f*0.18f + OffsetCounter * 109.5f+10,m_fAvatarSize/3,m_fAvatarSize/3),Avatars[8]);
				
				switch (DChar.byDwarfType)
				{
				case 0:
					GUI.Label(new Rect (Screen.width-(m_fAvatarSize/2),m_fAvatarSize*1.75f*0.18f + OffsetCounter * 111f,m_fAvatarSize/2.1f,m_fAvatarSize/2.1f),Avatars[1 + (ActiveDwarf.iTeamID-1) * 3]);
					
				break;
				case 1:
					GUI.Label(new Rect (Screen.width-(m_fAvatarSize/2),m_fAvatarSize*1.75f*0.18f + OffsetCounter * 111f,m_fAvatarSize/2.1f,m_fAvatarSize/2.1f),Avatars[2+ (ActiveDwarf.iTeamID-1) * 3]);
					
				break;
				case 2:
					GUI.Label(new Rect (Screen.width-(m_fAvatarSize/2),m_fAvatarSize*1.75f*0.18f + OffsetCounter * 111f,m_fAvatarSize/2.1f,m_fAvatarSize/2.1f),Avatars[3+ (ActiveDwarf.iTeamID-1) * 3]);
					
				break;
				}
			}
		}
		else
		{
			GUI.Label(new Rect (Screen.width-(m_fAvatarSize/2),m_fAvatarSize*1.75f*0.19f + OffsetCounter * 109.5f,m_fAvatarSize/2,m_fAvatarSize/2),Avatars[7]);
			GUI.Label(new Rect (Screen.width-(m_fAvatarSize/2),m_fAvatarSize*1.75f*0.19f + OffsetCounter * 109.5f,m_fAvatarSize/2,m_fAvatarSize/2),Avatars[7]);
			GUI.Label(new Rect (Screen.width-(m_fAvatarSize/2),m_fAvatarSize*1.75f*0.19f + OffsetCounter * 109.5f,m_fAvatarSize/2,m_fAvatarSize/2),Avatars[7]);
		}

		
		GUI.Label(new Rect (Screen.width-(m_fAvatarSize/2),m_fAvatarSize*1.75f,m_fAvatarSize/2,m_fAvatarSize/2),HUDTexture[4]);
		GUI.Label(new Rect (Screen.width-(m_fAvatarSize/2+3),m_fAvatarSize*1.75f+(m_fAvatarSize/2),m_fAvatarSize/2,m_fAvatarSize/2),HUDTexture[5]);
		//		GUI.DrawTextureWithTexCoords (new Rect (Screen.width-(m_fAvatarSize/2)-3, m_fAvatarSize*1.75f+19.5f, m_fAvatarSize/2 * m_fHealthScroll1, m_fAvatarSize/2), HUDTexture [6], new Rect (0f, 0f, m_fHealthScroll1, 4.6f));
		
		
		GUI.Label(new Rect (Screen.width-(m_fAvatarSize/2),m_fAvatarSize*2.5f,m_fAvatarSize/2,m_fAvatarSize/2),HUDTexture[4]);
		GUI.Label(new Rect (Screen.width-(m_fAvatarSize/2+3),m_fAvatarSize*2.5f+(m_fAvatarSize/2),m_fAvatarSize/2,m_fAvatarSize/2),HUDTexture[5]);
		//		GUI.DrawTextureWithTexCoords (new Rect (Screen.width-(m_fAvatarSize/2)-3, m_fAvatarSize*2.5f+19.5f, m_fAvatarSize/2 * m_fHealthScroll2, m_fAvatarSize/2), HUDTexture [6], new Rect (0f, 0f, m_fHealthScroll2, 4.6f));
		
		GUI.Label(new Rect (Screen.width-(m_fAvatarSize/2),m_fAvatarSize*3.25f,m_fAvatarSize/2,m_fAvatarSize/2),HUDTexture[4]);
		GUI.Label(new Rect (Screen.width-(m_fAvatarSize/2+3),m_fAvatarSize*3.25f+(m_fAvatarSize/2),m_fAvatarSize/2,m_fAvatarSize/2),HUDTexture[5]);
		//		GUI.DrawTextureWithTexCoords (new Rect (Screen.width-(m_fAvatarSize/2)-3, m_fAvatarSize*3.25f+19.5f, m_fAvatarSize/2 * m_fHealthScroll3, m_fAvatarSize/2), HUDTexture [6], new Rect (0f, 0f, m_fHealthScroll3, 4.6f));
		
		
		GUI.Label(new Rect (Screen.width-(m_fAvatarSize/2),m_fAvatarSize*4f,m_fAvatarSize/2,m_fAvatarSize/2),HUDTexture[4]);
		GUI.Label(new Rect (Screen.width-(m_fAvatarSize/2+3),m_fAvatarSize*4f+(m_fAvatarSize/2),m_fAvatarSize/2,m_fAvatarSize/2),HUDTexture[5]);
		//		GUI.DrawTextureWithTexCoords (new Rect (Screen.width-(m_fAvatarSize/2)-3, m_fAvatarSize*4f+19.5f, m_fAvatarSize/2 * m_fHealthScroll4, m_fAvatarSize/2), HUDTexture [6], new Rect (0f, 0f, m_fHealthScroll4, 4.6f));
		
		

		if(m_teamref)
		{
			if(ActiveDwarf)
			{
				for(int i = 0; i < m_teamref.m_arrTeams [ActiveDwarf.iTeamID - 1].lisDwarfs.Count; i++)
				{
//					GUI.Label(new Rect (Screen.width-(m_fAvatarSize/2),5 + m_fAvatarSize*1.75f + i * 110,m_fAvatarSize/2,m_fAvatarSize/2),HUDTexture[4]);
//					GUI.Label(new Rect (Screen.width-(m_fAvatarSize/2+3),m_fAvatarSize*1.75f + i* 110 + m_fAvatarSize/2,m_fAvatarSize/2,m_fAvatarSize/2),HUDTexture[5]);
					GUI.DrawTextureWithTexCoords (new Rect (Screen.width-(m_fAvatarSize/2+3), 19+ m_fAvatarSize*1.75f + i * 112.5f, m_fAvatarSize/2 * m_arrfHealthScroll[i], m_fAvatarSize/2), HUDTexture [6], new Rect (0f, 0f, m_arrfHealthScroll[i], 4.6f));
				}
			}
		}

		GUI.Label(new Rect (10f,10f,m_fAvatarSize,m_fAvatarSize),HUDTexture[4]);

		//RIGHT SIDE OF THE SCREEN
		//Life and AP on the RIGHT of the screen

		GUI.Label(new Rect (Screen.width-(m_fAvatarSize)-5f,m_fAvatarSize+10f,m_fAvatarSize,m_fAvatarSize),HUDTexture[5]);
		GUI.DrawTextureWithTexCoords (new Rect (Screen.width-(m_fAvatarSize)-5f, m_fAvatarSize*0.3f, m_fAvatarSize * m_fHealthScrollActive, m_fAvatarSize), HUDTexture [6], new Rect (0f, 0f, m_fHealthScrollActive, 4.6f));
		GUI.Label(new Rect (Screen.width-(m_fAvatarSize)+65,m_fAvatarSize+18,m_fAvatarSize,m_fAvatarSize),"" + m_fDwarfHealth, PauseStyle);
		GUI.Label(new Rect (Screen.width-(m_fAvatarSize)-5f,m_fAvatarSize+45f,m_fAvatarSize,m_fAvatarSize),HUDTexture[5]);
		GUI.DrawTextureWithTexCoords (new Rect (Screen.width-(m_fAvatarSize)-5f, m_fAvatarSize*0.3f+35, m_fAvatarSize * m_fAPScroll, m_fAvatarSize), HUDTexture [7], new Rect (0f, 0f, m_fAPScroll, 4.6f));
		GUI.Label(new Rect (Screen.width-(m_fAvatarSize)+65,m_fAvatarSize+53f,m_fAvatarSize,m_fAvatarSize),"" + m_fDwarfAP, PauseStyle);

		//LEFT SIDE OF THE SCREEN
		//Life on the LEFT of the screen
		GUI.Label(new Rect (10f,m_fAvatarSize+10f,m_fAvatarSize,m_fAvatarSize),HUDTexture[5]);
		GUI.DrawTextureWithTexCoords (new Rect (10f, m_fAvatarSize/2-30f, m_fAvatarSize * m_fBaseHealthScroll, m_fAvatarSize), HUDTexture [6], new Rect (0f, 0f, m_fBaseHealthScroll, 4.6f));
		GUI.Label(new Rect (70f,m_fAvatarSize+18f,m_fAvatarSize,m_fAvatarSize),""+m_fBaseHealth, PauseStyle);
		//GUI.Label(new Rect (10f,m_fAvatarSize+10f,m_fAvatarSize,m_fAvatarSize),HUDTexture[6]);

		if(m_bChatActive)
		GUI.Label(new Rect (10f,Screen.height-m_fAvatarSize*1.5f,m_fAvatarSize*2.5f,m_fAvatarSize*2.5f),HUDTexture[10]);

		//Block Icons
		GUI.Label(new Rect (10f,m_fAvatarSize+25,m_fAvatarSize,m_fAvatarSize),HUDTexture[11]);
		GUI.Label(new Rect (10f,m_fAvatarSize+75,m_fAvatarSize,m_fAvatarSize),HUDTexture[11]);
		GUI.Label(new Rect (10f,m_fAvatarSize+125,m_fAvatarSize,m_fAvatarSize),HUDTexture[11]);
		GUI.Label(new Rect (10f,m_fAvatarSize+175,m_fAvatarSize,m_fAvatarSize),HUDTexture[11]);
		GUI.Label(new Rect (10f,m_fAvatarSize+225,m_fAvatarSize,m_fAvatarSize),HUDTexture[11]);

		//Block Resources
		GUI.Label(new Rect (25f,m_fAvatarSize+90f,m_fAvatarSize/1.5f,m_fAvatarSize/1.5f),"Erde:\t\t\t\t" + m_iDirt, ResourceStyle);
		GUI.Label(new Rect (25f,m_fAvatarSize+140f,m_fAvatarSize/1.5f,m_fAvatarSize/1.5f),"Stein:\t\t\t"+ m_iStone, ResourceStyle);
		GUI.Label(new Rect (25f,m_fAvatarSize+190f,m_fAvatarSize/1.5f,m_fAvatarSize/1.5f),"Eisen:\t\t\t"+ m_iIron, ResourceStyle);
		GUI.Label(new Rect (25f,m_fAvatarSize+240f,m_fAvatarSize/1.5f,m_fAvatarSize/1.5f),"Kohle:\t\t\t"+ m_iCoal, ResourceStyle);
		GUI.Label(new Rect (25f,m_fAvatarSize+290f,m_fAvatarSize/1.5f,m_fAvatarSize/1.5f),"Salpeter:\t"+ m_iSaltpeter, ResourceStyle);

		//Class Changer
		if(m_bClassChangerActive)
		{
			for (int i = 0; i<ClassIcons.GetLength(0); ++i)
			{
				if (GUI.Button (
					new Rect (Screen.width / 3 * 0.775f + (i * (m_fAvatarSize * 1.14f)),
					(int)Screen.height / 2, m_fAvatarSize, m_fAvatarSize),
					ClassIcons [i], GUIStyle.none))
				{
					ActiveDwarf.ChangeClass(m_refDwarfType.m_arrDwarfTypes[i].strCategory);
					m_bClassChangerActive = false;
				}
			}
		}



		// Victories 
		for (int i = 0; i < m_teamref.m_arrTeams.Length; i++)
		{
			// Base destroyed
			if(m_teamref.m_arrTeams[i].bVictoryBaseDestruction)
			{
				int iTeam = i + 1;
				if(iTeam == 2)
				{
					iTeam = 1;
				}
				else
				{
					iTeam = 2;
				}
				string strWinBaseDestruction = "Team " + iTeam + " wins! Base destroyed!";
			   //Get Ready Label
				GUI.Label(new Rect(m_fScreenWidthCenter-(m_fAvatarSize/1.25f),m_fScreenHeightCenter-100,m_fAvatarSize,m_fAvatarSize),strWinBaseDestruction, ReadyStyle);
			}

			// Enemy destroyed
			if(m_teamref.m_arrTeams[i].bVictoryEnemyDestruction)
			{
				int iTeam = i + 1;
				if(iTeam == 2)
				{
					iTeam = 1;
				}
				else
				{
					iTeam = 2;
				}
				string strWinBaseDestruction = "Team " + iTeam + " wins! All enemies destroyed!";
				//Get Ready Label
				GUI.Label(new Rect(m_fScreenWidthCenter-(m_fAvatarSize/1.25f),m_fScreenHeightCenter-100,m_fAvatarSize,m_fAvatarSize),strWinBaseDestruction, ReadyStyle);
			}

			// Enemy destroyed
			if(m_teamref.m_arrTeams[i].bVictoryTreasureObjectHunt)
			{
				int iTeam = i + 1;
				if(iTeam == 2)
				{
					iTeam = 1;
				}
				else
				{
					iTeam = 2;
				}
				string strWinBaseDestruction = "Team " + iTeam + " wins! Got Treasue!";
				//Get Ready Label
				GUI.Label(new Rect(m_fScreenWidthCenter-(m_fAvatarSize/1.25f),m_fScreenHeightCenter-100,m_fAvatarSize,m_fAvatarSize),strWinBaseDestruction, ReadyStyle);
			}


		}


		//PAUSE MENU STARTS HERE		
		if (m_bIsTimeRunning == false) {
			if(Input.GetKey(KeyCode.Escape))
				GUIID = 0;

			Screen.showCursor = true;
			
			if(GUIID == 0)
			{
			GUI.Label(new Rect (m_fScreenWidthCenter-(m_fTimerSize*1.25f/2f),m_fScreenHeightCenter-(m_fTimerSize*1.25f/2f),m_fTimerSize*1.25f,m_fTimerSize*1.25f),Background[0]);

			for (int i = 0; i<Buttons.GetLength(0); ++i) {
				
				// Make the first button. If it is pressed, Application.Loadlevel (1) will be executed
				if (GUI.Button (new Rect (m_fScreenWidthCenter-(m_fAvatarSize*1.25f/2f),m_fScreenHeightCenter-(m_fAvatarSize)+ (i * (m_fAvatarSize *0.75f)),m_fAvatarSize*1.25f,m_fAvatarSize/2.5f), Buttons[i], PauseStyle)) {
					switch (i) {
						//Tutorial
					case 0: // Weiter
						m_bIsTimeRunning = true;
						m_hotseatref.m_bPausePhase = false;
						m_hotseatref.InvokeRepeating(m_hotseatref.GetLastInvoke(), 1f, 1f);
						break;
						//Join
					case 1:
						GUIID = 1;	
						break;
						//Host
					case 2:
						//Debug.Log ("loading level");
						PlayerPrefs.SetInt("m_iGameQuit", 2);
						PlayerPrefs.SetFloat ("h_SliderValue", h_SliderValue);
						Application.LoadLevel (0);
						break;
					}
				}
			}
			}
			if (GUIID == 1) {
				
				GUI.Label(new Rect (m_fScreenWidthCenter-(m_fTimerSize*1.25f/2f),m_fScreenHeightCenter-(m_fTimerSize*1.25f/2f),m_fTimerSize*1.25f,m_fTimerSize*1.25f),Background[1]);
				GUI.Label (new Rect (m_fScreenWidthCenter-(m_fTimerSize*1.25f/2f) + m_fTimerSize*0.38f,m_fScreenHeightCenter-(m_fTimerSize*1.15f/2f), m_fTimerSize*0.5f, m_fTimerSize*0.5f), Label [0], PauseStyle);
				
				for (int i = 0; i<ButtonsOptions.GetLength(0); ++i) {
					//Debug.Log("what??");
					// Make the first button. If it is pressed, Application.Loadlevel (1) will be executed
					if (GUI.Button (new Rect ((int)(m_fScreenWidthCenter-(m_fTimerSize*0.66f/2f)), (int)(m_fScreenHeightCenter-(m_fTimerSize*0.67f/2f) + (i * (m_fAvatarSize * 0.4f))), m_fAvatarSize,m_fAvatarSize/4), ButtonsOptions [i], PauseStyle)) {
						switch (i) {
							case 2:
								m_bShowSlider = true;
							break;
						}
					}	
				}
				for (int i = 0; i<ButtonsOptions2.GetLength(0); ++i) {
					// Make the first button. If it is pressed, Application.Loadlevel (1) will be executed
					if (GUI.Button (new Rect ((int)(m_fScreenWidthCenter-(m_fTimerSize*0.66f/2f) +(i * (m_fAvatarSize *1.19f) )), (int)(ScreenHeight - (m_fAvatarSize * -1.09f)), m_fAvatarSize,m_fAvatarSize/4), ButtonsOptions2 [i], PauseStyle)) {
						
						switch (i) {
						case 0:
							break;
						case 1:
							m_bShowSlider = false;
							GUIID = 0;
							break;
						}
						
					}	
				}	
				if(m_bShowSlider)
				{
						GUI.Label(new Rect (m_fScreenWidthCenter-(m_fTimerSize*-0.1f/2f),m_fScreenHeightCenter-(m_fTimerSize*0.54f/2f)-30,150,30),"Gesamtlautstärke", PauseStyle);
						h_SliderValue = GUI.HorizontalSlider(new Rect(m_fScreenWidthCenter-(m_fTimerSize*-0.1f/2f),m_fScreenHeightCenter-(m_fTimerSize*0.54f/2f),100,30),h_SliderValue,0.0f,1.0f);
				}
			}
		}
	}
	void APTextFlash()
	{
		APTexture = HUDTexture[13];
	}

}