using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DwarfChar : MonoBehaviour
{
	enum ActionState
	{
		NONE,
		WALKING,
		FALLING,
		CLIMBING,
		MINING,
		ATTACK_MELEE,
		ATTACK_DISTANCE,
		DYING
	};
	
	public delegate void VoidDelegate();
	event VoidDelegate NoAPsLeft;
	
	#region CommonVariables
	private byte m_byDwarfType; // class changebale
	public byte byDwarfType
	{
		get {return m_byDwarfType;}
	}

	private int m_iIndexPosX;
	public int iIndexPosX
	{
		get { return m_iIndexPosX; }
	}

	private int m_iIndexPosY;
	public int iIndexPosY
	{
		get { return m_iIndexPosY; }
	}
	private int m_iPrevPosX;
	private int m_iPrevPosY;

	private int m_iTeamID;
	public int iTeamID
	{
		get { return m_iTeamID; }
	}
	private int m_iID;
	public int iID
	{
		get { return m_iID; }
	}

	private int m_iMiningPositionX;
	private int m_iMiningPositionY;

	private int m_iSoundCount;

	private bool m_bActive = false;
	public bool Active
	{
		get { return m_bActive; }
		set { m_bActive = value; }
	}

	private bool m_bDropping = false;
	private bool m_bPrevDropping = false;

	private bool m_bHasTreasure = false;

	private int m_iSelectedBlock = -1;
	public int iSelectedBlock
	{
		get { return m_iSelectedBlock; }
		set { m_iSelectedBlock = value; }
	}

	private float m_fWalkingSpeed = 0.1f; // class changebale
	public float WalkingSpeed
	{
		get { return m_fWalkingSpeed; }
		set { m_fWalkingSpeed = value; }
	}
	private float m_fDropSpeed = 5f;
	private float m_fDropDamage = 10f;
	public float m_fDropDmgPerBlock = 10f; // class changebale

	private float m_fPosCorrectionTreshold = 0.05f;
	
	private float m_fOutlineWidth;

	private Vector2 vec2NextPos = new Vector2(-1, -1);
	private Vector2 vec2Destiny = new Vector2(-1, -1);
	private Vector2 vec2NextWorldPos = new Vector2(-1, -1);
	
	private string m_strSilhouetteShader = "Outlined/Silhouetted Diffuse";
	private string m_strSelectedBombtype = null;
	public string strSelectedBombtype
	{
		get { return m_strSelectedBombtype; }
		set { m_strSelectedBombtype = value; }
	}
	
	private Color m_colOutlineColor;
	
	private GameObject m_goProjectile; // class changebale
	private GameObject m_goTreasureIcon;
	private GameObject m_refGoTreasurePile;

	private Animation m_AnimationComponent;

	private SoundInclude m_Sounds;

	private Renderer[] m_mrRenderer;

	private Dictionary<string, float> m_dicAnimationLenghts;

	private ActionState m_eAState = ActionState.NONE;
	private ActionState m_eConnectingAState = ActionState.NONE;

	private RessourceInventory m_Inventory;
	private Pathfinder m_Pathing;
	private DwarfType m_DwarfType; // class changebale
	#endregion
	
	#region Upgrade-able Variables
	// class changer take values of teamclass
	private int m_iBombUpgradeLevel = 0;
	public int iBombUpgradeLevel
	{
		get { return m_iBombUpgradeLevel; }
		set { m_iBombUpgradeLevel = value; }
	}
	private float m_fHealth = 0;
	public float fHealth
	{
		get { return m_fHealth; }
		set { m_fHealth = value; }
	}
	public int iHealth
	{
		get { return Mathf.RoundToInt(m_fHealth); }
	}
	private float m_fHealthRegeneration;
	public float fHealthRegeneration
	{
		get { return m_fHealthRegeneration; }
		set { m_fHealthRegeneration = value; }
	}
	private int m_iYield = 1;
	public int iYield
	{
		get { return m_iYield; }
		set { m_iYield = value; }
	}
	private int m_iSightRadius = 2;
	public int iSightRadius
	{
		get { return m_iSightRadius; }
		set { m_iSightRadius = value; }
	}
	private float m_fMeleeDmg;
	public float fMeleeDmg
	{
		get { return m_fMeleeDmg; }
		set { m_fMeleeDmg = value; }
	}
	
	private float m_fDistanceDmg;
	public float DistanceDamage
	{
		get { return m_fDistanceDmg; }
		set { m_fDistanceDmg = value; }
	}
	private float m_iDistanceRange = 4;
	public float iDistanceRange
	{
		get { return m_iDistanceRange; }
		set { m_iDistanceRange = value; }
	}
	private int m_iPickeAxeHardness = 20;
	public int iPickAxeHardness
	{
		get { return m_iPickeAxeHardness; }
		set { m_iPickeAxeHardness = value; }
	}
	private float m_fDigTime;
	public float fDigTime
	{
		get { return m_fDigTime; }
		set { m_fDigTime = value; }
	}
	#endregion

	#region References
	private Transform m_refSelfTransform;
	
	private Map m_refMap;
	private Explosion m_refExplosion;
	private BombConfigurator m_refBombConfig;
	private Mouse2D m_refMouse;
	private Team m_refTeam;
	private ActionConfigurator m_ActionConfig;
	private GameGUI m_GameGui;
	private HUDSetup m_refHudSetup;
	private Camera m_camMain;
	#endregion

	#region Initialization
	void Awake()
	{
		m_refSelfTransform = transform;
	
		GameObject goWorldObject = GameObject.Find ("DwarfareWorldObject");

		m_DwarfType = goWorldObject.GetComponent<DwarfType>();
		m_refMap = goWorldObject.GetComponent<Map>();
		m_refTeam = goWorldObject.GetComponent<Team>();
		m_refExplosion = goWorldObject.GetComponent<Explosion>();
		m_refBombConfig = goWorldObject.GetComponent<BombConfigurator>();
		m_ActionConfig = goWorldObject.GetComponent<ActionConfigurator>();
		m_Sounds = goWorldObject.GetComponent<SoundInclude>();
		
		GameObject goGameGui = GameObject.Find ("GUIObject");
		m_GameGui = goGameGui.GetComponentInChildren<GameGUI>();
		m_refHudSetup = goGameGui.GetComponentInChildren<HUDSetup>();
		
		if (m_refHudSetup == null)
			Debug.LogWarning ("no hudsetup component found");
		
		m_camMain = Camera.main;

		m_dicAnimationLenghts = new Dictionary<string, float>();

		if (m_ActionConfig == null)
			Debug.LogWarning ("No ActionConfigurator found on the DwarfareWorldObject");
	}
	
	void Start()
	{
		m_goTreasureIcon = GameObject.Find ("TreasureIcon");

		if (m_goTreasureIcon == null)
			Debug.LogWarning("No TreasureIcon was found in scene. Ask Mauli what to do.");

		m_refGoTreasurePile = GameObject.Find ("Treasure_Pile(Clone)");

		if (m_refGoTreasurePile == null)
			Debug.LogWarning("No TreasurePile was found in scene. Ask Mauli what to do.");

		m_Pathing = new Pathfinder(m_refMap, m_refMap.m_preRasterSphere);
		//m_Pathing.m_bDebugMode = true;
		m_Inventory = m_refTeam.m_arrTeams[m_iTeamID - 1].m_Inventory;
		
		m_refMouse = m_refMap.gameObject.GetComponent<Mouse2D>();

		Ray ray = new Ray(new Vector3(m_refSelfTransform.position.x, m_refSelfTransform.position.y, m_refSelfTransform.position.z - 1f), new Vector3(0f, 0f, 1f));
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit))
		{
			FogOfWar fow = hit.collider.gameObject.GetComponent<FogOfWar>();
			if (fow != null)
				fow.MakeTransparent(hit.triangleIndex, m_iSightRadius);
		}

		m_AnimationComponent = gameObject.animation;
		if (m_AnimationComponent == null)
		{
			m_AnimationComponent = gameObject.GetComponentInChildren<Animation>();
		}
		AnimationTimer animTimer = GameObject.Find ("DwarfareWorldObject").GetComponent<AnimationTimer>();

		foreach (AnimationState animState in m_AnimationComponent)
		{
			float fTime = animTimer.GetAnimTimeByString(animState.name);
			if (fTime > 0)
			{
				animState.speed = animState.length / fTime;
			}

			m_dicAnimationLenghts.Add(animState.name, animState.length / animState.speed);
		}

		m_iMiningPositionX = -1;
		m_iMiningPositionY = -1;;

		m_iSoundCount = m_Sounds.m_iSoundPeriod;

		m_fWalkingSpeed = Map.m_fWidth / m_dicAnimationLenghts["walking"] * 8f;

		m_AnimationComponent.Play("idle");
	}
	#endregion

	#region Iinit Method
	public void Initialize(string strDwarfType, Vector2 vec2Pos, int iTeamID, int iID)
	{
		string[] strNames = new string[] {
			m_DwarfType.m_arrDwarfTypes[0].strCategory,
			m_DwarfType.m_arrDwarfTypes[1].strCategory,
			m_DwarfType.m_arrDwarfTypes[2].strCategory
		};
		
		for (int i = 0; i < m_DwarfType.m_arrDwarfTypes.GetLength(0); ++i)
		{
			if (strNames[i].Equals(strDwarfType))
			{
				m_byDwarfType = (byte)i;
				m_fHealth = (float)m_DwarfType.m_arrDwarfTypes[i].iHealthPoints;
				m_fMeleeDmg = m_DwarfType.m_arrDwarfTypes[i].iMeleeDamage;
				m_fDistanceDmg = m_DwarfType.m_arrDwarfTypes[i].iDistanceDamage;
				m_fDigTime = m_DwarfType.m_arrDwarfTypes[i].fDigTime;
				m_goProjectile = m_DwarfType.m_arrDwarfTypes[i].goDistantWeapon;
				m_fDropDmgPerBlock = m_DwarfType.m_arrDwarfTypes[i].iFallDamagePerBlock;
			}
		}
		
		m_iID = iID;
		m_iTeamID = iTeamID;
		
		m_iIndexPosX = (int)vec2Pos.x;
		m_iIndexPosY = (int)vec2Pos.y;
		
		m_refSelfTransform.position = Map.IndexToWorldOffset(m_iIndexPosX, m_iIndexPosY);
		
		// cache materials
		m_mrRenderer = (Renderer[])gameObject.GetComponentsInChildren<Renderer>();
	}
	#endregion

	#region Update
	void Update()
	{	
		if (m_AnimationComponent != null)
		{
			if (m_eAState == ActionState.NONE && !m_AnimationComponent.isPlaying)
			{
				m_AnimationComponent.Play("idle");
			}
		}
		else
		{
			UpdateAnimationComponent();
		}

		#region Drop Detection
		m_bPrevDropping = m_bDropping;
		if (m_eAState != ActionState.CLIMBING)
		{
			if (m_refMap.IsInAir(m_iIndexPosX, m_iIndexPosY) && !m_refMap.IsClimbable(iIndexPosX, m_iIndexPosY))
			{
				if (m_eAState != ActionState.FALLING && vec2NextWorldPos.x < 0 && vec2NextWorldPos.y < 0)
				{
					m_AnimationComponent.Play("falling");

					m_eAState = ActionState.FALLING;

					int y = m_iIndexPosY;
					for (y = m_iIndexPosY; y > 0; --y)
					{
						if (!m_refMap.IsInAir(m_iIndexPosX, y))
						    break;
					}
					m_fDropDamage = Mathf.Max ((m_iIndexPosY - y - 1) * m_fDropDmgPerBlock, 0f);
					vec2NextWorldPos = Map.IndexToWorldOffset(m_iIndexPosX, y);
					m_bDropping = true;
				}
			}
			else
			{
				if (vec2NextWorldPos.x < 0 && vec2NextWorldPos.y < 0)
					m_bDropping = false;
			}
		}
		if (!m_bDropping && m_bPrevDropping)
		{
			m_fHealth -= m_fDropDamage;
			m_fDropDamage = 0f;
			m_AnimationComponent.Play ("falling_landing");
			m_eAState = ActionState.NONE;
		}
		#endregion

		int iIndexX = Mathf.RoundToInt (m_refMouse.HitGridIntX);
		int iIndexY = Mathf.RoundToInt (m_refMouse.HitGridIntY);

		if (!m_GameGui.m_bBuildngActive)
		{
			bool bValidMove = false;
			bool bActionThisFrame = false;

			if (m_bActive && m_eAState == ActionState.NONE)
			{
				#region RightMouseButton
				if (Input.GetMouseButtonDown(1))
				{
					bActionThisFrame = true;

					int iDiffX = iIndexX - m_iIndexPosX;
					int iDiffY = iIndexY - m_iIndexPosY;

					Vector3 vec3CurrentRot = m_refSelfTransform.rotation.eulerAngles;

					float fYRot = vec3CurrentRot.y;

					if (iDiffX > 0)
						fYRot = 270f;
					else if (iDiffX < 0)
						fYRot = 90f;
					else if (iDiffX == 0 && iDiffY == 0 && vec2NextWorldPos.x < 0 && vec2NextWorldPos.y < 0)
						fYRot = 0f;

					m_refSelfTransform.eulerAngles = new Vector3(vec3CurrentRot.x, fYRot, vec3CurrentRot.z);

					bValidMove = CheckTreasure(iIndexX, iIndexY);
					CheckTreasureToBase(m_iIndexPosX, m_iIndexPosY);
					
					if (!bValidMove)
						bValidMove = CheckClassChanger(iIndexX, iIndexY);

					if (!bValidMove)
					{
						if (m_byDwarfType == 2)
						{
							DwarfBomb tempBomb = m_refTeam.GetBombAt(iIndexX, iIndexY, m_iTeamID);

							if (tempBomb != null)
							{
								tempBomb.Activate();
								bValidMove = true;
							}
						}
					}

					if (!bValidMove)
						bValidMove = StartMovement(iIndexX, iIndexY);
					if (!bValidMove)
						bValidMove = StartMining(iIndexX, iIndexY);
					if (!bValidMove)
						bValidMove = CheckAttack(iIndexX, iIndexY);
				}
				#endregion
				#region LeftMouseButton
				else if (Input.GetMouseButtonDown(0))
				{
					bActionThisFrame = true;

					if (!bValidMove)
						bValidMove = CheckSettingBlock(iIndexX, iIndexY);
					if (!bValidMove)
						bValidMove = CheckPlacingBomb(iIndexX, iIndexY);

				}
				#endregion
				if (bActionThisFrame)
				{
					if (!bValidMove)
					{
						if (m_iSoundCount == m_Sounds.m_iSoundPeriod)
						{
							m_camMain.audio.PlayOneShot(m_Sounds.GetRandomNegation());
							m_iSoundCount = 0;
						}
						m_iSoundCount++;
					}
					else
					{
						if (m_iSoundCount == m_Sounds.m_iSoundPeriod)
						{
							m_camMain.audio.PlayOneShot(m_Sounds.GetRandomConfirmation());
							m_iSoundCount = 0;
						}
						m_iSoundCount++;
					}
				}
			}

			#region Pathfinding
			if (vec2NextPos.x >= 0 && vec2NextPos.y >= 0)
			{
				if (m_iIndexPosX == (int)vec2Destiny.x && m_iIndexPosY == (int)vec2Destiny.y)
				{
					if (vec2NextWorldPos.x < 0 && vec2NextWorldPos.y < 0)
					{
						FinishPathfinding();
						//m_eAState = m_eConnectingAState;
						switch (m_eConnectingAState)
						{
						case ActionState.MINING:
							StartMining(m_iMiningPositionX, m_iMiningPositionY);
							break;
						}
						m_eConnectingAState = ActionState.NONE;
					}
				}
				else if ((m_iIndexPosX == (int)vec2NextPos.x && m_iIndexPosY == (int)vec2NextPos.y && m_eAState == ActionState.WALKING) ||
				          m_eAState == ActionState.NONE)
				{
					if (vec2NextWorldPos.x < 0 && vec2NextWorldPos.y < 0)
					{
						if (m_refTeam.m_arrTeams[m_iTeamID - 1].iActionpointsTeam >= m_ActionConfig.Walking)
						{
							vec2NextPos = m_Pathing.GetNextPosition(new Vector2(m_iIndexPosX, m_iIndexPosY));
							vec2NextWorldPos = Map.IndexToWorldOffset((int)vec2NextPos.x, (int)vec2NextPos.y);

							if (!m_AnimationComponent.IsPlaying("walking"))
								m_AnimationComponent.Play ("walking");

							float fYRot = m_refSelfTransform.eulerAngles.y;

							if ((int)vec2NextPos.x > m_iIndexPosX)
								fYRot = 270f;
							else if ((int)vec2NextPos.x < m_iIndexPosX)
								fYRot = 90f;

							m_refSelfTransform.eulerAngles = new Vector3(m_refSelfTransform.eulerAngles.x, fYRot, m_refSelfTransform.eulerAngles.z);

							m_refTeam.m_arrTeams[m_iTeamID - 1].iActionpointsTeam -= m_ActionConfig.Walking;

							if ((int)vec2NextPos.y > m_iIndexPosY)
								m_eAState = ActionState.CLIMBING;
							else
								m_eAState = ActionState.WALKING;
						}
						else
						{
							FinishPathfinding();
						}
					}
				}
			}
			if (m_iPrevPosX != m_iIndexPosX || m_iPrevPosY != m_iIndexPosY)
			{
				Ray ray = new Ray(new Vector3(m_refSelfTransform.position.x, m_refSelfTransform.position.y, m_refSelfTransform.position.z - 1f), new Vector3(0f, 0f, 1f));
				RaycastHit hit;
				if (Physics.Raycast(ray, out hit))
				{
					FogOfWar fow = hit.collider.gameObject.GetComponent<FogOfWar>();
					if (fow != null)
					{
						fow.MakeTransparent(hit.triangleIndex, m_iSightRadius);
					}
				}
			}
			#endregion
		}

		#region Movement
		if (vec2NextWorldPos.x >= 0 && vec2NextWorldPos.y >= 0)
		{
			Vector3 fMovement = new Vector3();
			
			switch (m_eAState)
			{
			case ActionState.WALKING:
				// move in direction of the next worldpos position
				float fDiffX = Mathf.Abs (vec2NextWorldPos.x - m_refSelfTransform.position.x);
				float fDiffY = Mathf.Abs (vec2NextWorldPos.y - m_refSelfTransform.position.y);

				if (fDiffX > fDiffY)
					fMovement = Vector3.back * m_fWalkingSpeed * Time.deltaTime;
				else if (fDiffX < fDiffY)
					fMovement = Vector3.down * m_fWalkingSpeed * Time.deltaTime;
				break;
			case ActionState.FALLING:
				fMovement = Vector3.down * m_fDropSpeed * Time.deltaTime;
				break;
			case ActionState.CLIMBING:
				fMovement = Vector3.up * m_fWalkingSpeed * Time.deltaTime;
				break;
			}
			// safety check if dwarf is running over the point
			// alternative: m_fPosCorrectionTreshold proportional to speed + frames per second
			if ((vec2NextWorldPos - new Vector2(m_refSelfTransform.position.x, m_refSelfTransform.position.y)).sqrMagnitude > m_fPosCorrectionTreshold)
			{
				// x and z refer to local space ( therefore the 0f is at x )
				m_refSelfTransform.Translate (0f, fMovement.y, fMovement.z);
			}
			else
			{
				m_refSelfTransform.position = vec2NextWorldPos;
				vec2NextWorldPos = new Vector2(-1, -1);
				m_eAState = ActionState.NONE;
			}
		}
		m_iPrevPosX = m_iIndexPosX;
		m_iPrevPosY = m_iIndexPosY;
		
		Vector2 vec2NewIndexPos = Map.WorldToIndex(transform.position);
		m_iIndexPosX = (int)vec2NewIndexPos.x;
		m_iIndexPosY = (int)vec2NewIndexPos.y;
		#endregion

		#region ActionStates
		// do that for all states
		if (m_eAState == ActionState.MINING)
		{
			if (!m_AnimationComponent.IsPlaying("digging"))
			{
				LoadLevelTexture.BlockType blockTypeAtPos = m_refMap.GetBlockTypeByID(m_refMap.m_arrMap[iIndexX, iIndexY].iBlockID);

				if (m_iMiningPositionX >= 0 && m_iMiningPositionY >= 0)
				{
					// HACK HACK HACK
					int iBlockTypeID = blockTypeAtPos.m_iTypeID;
					if (iBlockTypeID == 20)
						iBlockTypeID = 21;
					// HACK HACK HACK
					m_Inventory.AddRessPoint(iBlockTypeID, m_iYield);
					m_refMap.ChangeBlockIDAtPosition(0, m_iMiningPositionX, m_iMiningPositionY);
					m_refTeam.m_arrTeams[m_iTeamID - 1].iActionpointsTeam -= m_ActionConfig.Digging;

					m_eAState = ActionState.NONE;

					// Move to mining location
					StartMovement(m_iMiningPositionX, m_iMiningPositionY);

					// reset mining
					m_iMiningPositionX = -1;
					m_iMiningPositionY = -1;
				}
			}
		}
		else if (m_eAState == ActionState.ATTACK_MELEE)
		{
			if (!m_AnimationComponent.IsPlaying("attack_melee"))
			{
				m_eAState = ActionState.NONE;
				m_AnimationComponent.Play("idle");
			}
		}
		else if (m_eAState == ActionState.ATTACK_DISTANCE)
		{
			if (!m_AnimationComponent.IsPlaying("attack_distance"))
			{
				m_eAState = ActionState.NONE;
				m_AnimationComponent.Play ("idle");
			}
		}
		#endregion

		if (m_fHealth <= 0f)
		{
			if (m_eAState != ActionState.DYING)
			{
				m_eAState = ActionState.DYING;
				m_AnimationComponent.Play ("dying");
			}
			else
			{
				if (!m_AnimationComponent.isPlaying)
				{
					if (m_refTeam.m_arrTeams[m_iTeamID - 1].bTeamHasTreasure)
					{
						m_bHasTreasure = false;
						m_refTeam.m_arrTeams[m_iTeamID - 1].bTeamHasTreasure = false;
						m_goTreasureIcon.transform.parent = null;
						Debug.Log ("treasure was los");
					}

					m_refTeam.DeleteDwarf(m_iID, m_iTeamID);
				}
			}
		}
	}
	#endregion

	#region Setting Blocks
	private bool CheckSettingBlock(int iIndexX, int iIndexY)
	{
		if (!BlockIsNeighbour (iIndexX, iIndexY, m_iIndexPosX, m_iIndexPosY))
			return false;
		if (m_byDwarfType == 2 && !string.IsNullOrEmpty(m_strSelectedBombtype))
			return false;
		if (m_iSelectedBlock < 0)
			return false;
		
		LoadLevelTexture.BlockType placingBlockType = m_refMap.refLoadLevelTexture.GetBlockTypeByTypeID(m_iSelectedBlock);
		
		if (m_refMap.IsSolid(iIndexX, iIndexY))
			return false;
		if (!placingBlockType.m_bPlaceable)
			return false;
		// HACK HACK HACK HACK HACK HACK HACK HACK HACK
		// check if enough ress
		bool success = false;
						
		if (m_iSelectedBlock == 4)
		{
			if (m_refTeam.m_arrTeams[m_iTeamID - 1].iActionpointsTeam >= m_ActionConfig.BuildingLadder)
			{
				success = m_refMap.ChangeBlockIDAtPosition(m_iSelectedBlock, iIndexX, iIndexY);
				m_refTeam.m_arrTeams[m_iTeamID - 1].iActionpointsTeam -= m_ActionConfig.BuildingLadder;

				return success;
			}
		}
		else
		{
			if (m_refTeam.m_arrTeams[m_iTeamID - 1].iActionpointsTeam >= m_ActionConfig.SettingBlock)
			{
				if (m_refTeam.m_arrTeams[m_iTeamID - 1].m_Inventory.GetRessourceCount(m_iSelectedBlock) > 0)
				{
					success = m_refMap.ChangeBlockIDAtPosition(m_iSelectedBlock, iIndexX, iIndexY);
					m_refTeam.m_arrTeams[m_iTeamID - 1].iActionpointsTeam -= m_ActionConfig.SettingBlock;
					m_refTeam.m_arrTeams[m_iTeamID - 1].m_Inventory.AddRessPoint(m_iSelectedBlock, -1);
				}
			}
		}
						
		if (m_refMap.m_arrMap[iIndexX, iIndexY - 1].iBlockID == 20 && success)
		{
			m_refMap.ChangeBlockIDAtPosition(21, iIndexX, iIndexY - 1);
		}

		return success;
		// HACK HACK HACK HACK HACK HACK HACK HACK HACK
	}
	#endregion

	#region Planting Bombs
	private bool CheckPlacingBomb(int iIndexX, int iIndexY)
	{
		if (!BlockIsNeighbour(iIndexX, iIndexY, m_iIndexPosX, m_iIndexPosY))
			return false;
		if (m_byDwarfType != 2 || string.IsNullOrEmpty (m_strSelectedBombtype))
			return false;

		BombDefinition bombDef = m_refBombConfig.GetBombtypeByName(m_strSelectedBombtype);

		if (bombDef == null)
			return false;

		if (bombDef.BombPrefab == null)
			return false;
		if (m_Inventory.GetRessourceCount(22) >= bombDef.CoalCost &&
		    m_Inventory.GetRessourceCount(31) >= bombDef.IronCost &&
		    m_Inventory.GetRessourceCount(32) >= bombDef.SalpeterCost)
		{
			if (m_refTeam.m_arrTeams[m_iTeamID - 1].iActionpointsTeam >= m_ActionConfig.PlantingBomb)
			{
				GameObject goBomb = (GameObject)Instantiate(bombDef.BombPrefab, Map.IndexToWorldOffset(iIndexX, iIndexY), bombDef.BombPrefab.transform.rotation);
				DwarfBomb tempDwarfBomb = goBomb.AddComponent<DwarfBomb>();
				tempDwarfBomb.Initialize(bombDef, 1, m_refTeam.m_arrTeams[m_iTeamID - 1], m_refExplosion);
				m_refTeam.m_arrTeams[m_iTeamID - 1].m_lisBombs.Add(tempDwarfBomb);
				
				m_Inventory.AddRessPoint(22, -bombDef.CoalCost);
				m_Inventory.AddRessPoint(31, -bombDef.IronCost);
				m_Inventory.AddRessPoint(32, -bombDef.SalpeterCost);
				
				m_refTeam.m_arrTeams[m_iTeamID - 1].iActionpointsTeam -= m_ActionConfig.PlantingBomb;

				return true;
			}
		}

		return false;
	}
	#endregion

	#region GetMiningPosition
	private Vector2 GetMiningPosition(int iIndexX, int iIndexY)
	{
		Vector2 miningPos = new Vector2(-1, -1);
		int iMinWalkingDist = int.MaxValue;
		bool pathing = false;

		int posX = iIndexX + 1;
		if (m_refMap.IsPassable(posX, iIndexY) &&
		    (!m_refMap.IsInAir(posX, iIndexY) || m_refMap.IsClimbable(posX, iIndexY)))
		{
			pathing = m_Pathing.StartPathing(m_iIndexPosX, m_iIndexPosY, posX, iIndexY);

			if (pathing)
			{
				int pathLength = m_Pathing.GetPathLength();
				if (iMinWalkingDist > pathLength)
				{
					iMinWalkingDist = pathLength;
					miningPos = new Vector2(posX, iIndexY);
				}
			}
		}
		posX = iIndexX - 1;
		if (m_refMap.IsPassable(posX, iIndexY) &&
		    (!m_refMap.IsInAir(posX, iIndexY) || m_refMap.IsClimbable(posX, iIndexY)))
		{
			pathing = m_Pathing.StartPathing(m_iIndexPosX, m_iIndexPosY, posX, iIndexY);
			
			if (pathing)
			{
				int pathLength = m_Pathing.GetPathLength();
				if (iMinWalkingDist > pathLength)
				{
					iMinWalkingDist = pathLength;
					miningPos = new Vector2(posX, iIndexY);
				}
			}
		}
		int posY = iIndexY + 1;
		if (m_refMap.IsPassable(iIndexX, posY) &&
		    (!m_refMap.IsInAir(iIndexX, posY) || m_refMap.IsClimbable(iIndexX, posY)))
		{
			pathing = m_Pathing.StartPathing(m_iIndexPosX, m_iIndexPosY, iIndexX, posY);
			
			if (pathing)
			{
				int pathLength = m_Pathing.GetPathLength();
				if (iMinWalkingDist > pathLength)
				{
					iMinWalkingDist = pathLength;
					miningPos = new Vector2(iIndexX, posY);
				}
			}
		}
		posY = iIndexY - 1;
		if (m_refMap.IsPassable(iIndexX, posY) &&
		    (!m_refMap.IsInAir(iIndexX, posY) || m_refMap.IsClimbable(posY, iIndexY)))
		{
			pathing = m_Pathing.StartPathing(m_iIndexPosX, m_iIndexPosY, iIndexX, posY);
			
			if (pathing)
			{
				int pathLength = m_Pathing.GetPathLength();
				if (iMinWalkingDist > pathLength)
				{
					iMinWalkingDist = pathLength;
					miningPos = new Vector2(iIndexX, posY);
				}
			}
		}

		return miningPos;
	}
	#endregion

	#region Movement
	private bool StartMovement(int iIndexX, int iIndexY)
	{
		bool bValidMove = false;

		DwarfChar PlayerAtPosition = m_refTeam.GetDwarfCharByPosition(iIndexX, iIndexY);

		if (iIndexX == m_iIndexPosX && iIndexY == m_iIndexPosY)
			return false;
		if (!m_refMap.IsPassable(iIndexX, iIndexY))
		{
			return false;
		}
		if (PlayerAtPosition != null && PlayerAtPosition.iTeamID != m_iTeamID)
			return false;
		if (m_refMap.IsInAir(iIndexX, iIndexY) && !m_refMap.IsClimbable(iIndexX, iIndexY))
		{
			return false;
		}
		if (m_refTeam.m_arrTeams[m_iTeamID - 1].iActionpointsTeam < m_ActionConfig.Walking)
		{
			return false;
		}
		// curser icon change
		bool pathing = false;
		pathing = m_Pathing.StartPathing(m_iIndexPosX, m_iIndexPosY, iIndexX, iIndexY);

		if (!pathing)
		{
			return false;
		}

		vec2NextPos = m_Pathing.GetNextPosition(new Vector2(m_iIndexPosX, m_iIndexPosY));

		float fYRot = m_refSelfTransform.eulerAngles.y;
		
		if ((int)vec2NextPos.x > m_iIndexPosX)
			fYRot = 270f;
		else if ((int)vec2NextPos.x < m_iIndexPosX)
			fYRot = 90f;
		
		m_refSelfTransform.eulerAngles = new Vector3(m_refSelfTransform.eulerAngles.x, fYRot, m_refSelfTransform.eulerAngles.z);

		vec2NextWorldPos = Map.IndexToWorldOffset((int)vec2NextPos.x, (int)vec2NextPos.y);

		vec2Destiny = new Vector2(iIndexX, iIndexY);

		bValidMove = true;

		m_refTeam.m_arrTeams[m_iTeamID - 1].iActionpointsTeam -= m_ActionConfig.Walking;

		if ((int)vec2NextPos.y > m_iIndexPosY)
		{
			m_eAState = ActionState.CLIMBING;
			//m_AnimationComponent.Play("climbing");
		}
		else
		{
			m_eAState = ActionState.WALKING;
			m_AnimationComponent.Play("walking");
		}

		return bValidMove;
	}

	private void FinishPathfinding()
	{
		vec2Destiny = new Vector2(-1, -1);
		vec2NextPos = new Vector2(-1, -1);
		
		m_AnimationComponent.Play("idle");
		
		m_eAState = ActionState.NONE;
		
		m_Pathing.DeleteDebugSphere();
	}
	#endregion

	#region Attack
	private bool CheckAttack(int iIndexX, int iIndexY)
	{
		DwarfChar PlayerAtPosition = m_refTeam.GetDwarfCharByPosition(iIndexX, iIndexY);

		int iDiffX = iIndexX - m_iIndexPosX;
		int iDiffY = iIndexY - m_iIndexPosY;

		if (PlayerAtPosition == null)
			return false;
		if (PlayerAtPosition.iTeamID == m_iTeamID)
			return false;
		#region Melee
		if ((iIndexX == m_iIndexPosX + 1 || iIndexX == m_iIndexPosX - 1) && iIndexY == m_iIndexPosY)
		{
			if (m_refTeam.m_arrTeams[m_iTeamID - 1].iActionpointsTeam >= m_ActionConfig.Melee)
			{
				PlayerAtPosition.m_fHealth -= m_fMeleeDmg;
				m_refTeam.m_arrTeams[m_iTeamID - 1].iActionpointsTeam -= m_ActionConfig.Melee;
				m_AnimationComponent.Play ("attack_melee");
				
				m_eAState = ActionState.ATTACK_MELEE;

				return true;
			}
		}
		#endregion
		#region Distance
		else
		{
			if (m_refTeam.m_arrTeams[m_iTeamID - 1].iActionpointsTeam >= m_ActionConfig.Distance)
			{
				if (Mathf.Abs (iDiffX) <= m_iDistanceRange && Mathf.Abs (iDiffY) <= m_iDistanceRange)
				{
					int x = m_iIndexPosX, y = m_iIndexPosY;
					int iDirX = 0;
					if (Mathf.Abs (iDiffX) > 0)
					{
						iDirX = iDiffX / Mathf.Abs (iDiffX);
						for (; Mathf.Abs(x - iIndexX) > 0; x += iDirX)
						{
							if (m_refMap.IsSolid(x, m_iIndexPosY))
								break;
						}
					}
					int iDirY = 0;
					if (Mathf.Abs (iDiffY) > 0)
					{
						iDirY = iDiffY / Mathf.Abs (iDiffY);
						for (; Mathf.Abs(y - iIndexY) > 0; y += iDirY)
						{
							if (m_refMap.IsSolid(m_iIndexPosX, y))
								break;
						}
					}
					
					GameObject goTemp = null;
					
					if (x == iIndexX && y == iIndexY)
					{
						goTemp = PlayerAtPosition.gameObject;
					}
					else
					{
						goTemp = m_refMap.m_arrMap[iIndexX, iIndexY].prefab1;
					}
					
					if (goTemp != null)
					{
						// throwing direction
						
						GameObject goProj= (GameObject)Instantiate(m_goProjectile, m_refSelfTransform.position, Quaternion.identity);
						//goProj.animation.Play("AxeRotation");
						Projectile projTemp = goProj.AddComponent<Projectile>();
						projTemp.Initialize(new Vector2(iDirX, iDirY), 0.1f, this, goTemp);
						
						m_refTeam.m_arrTeams[m_iTeamID - 1].iActionpointsTeam -= m_ActionConfig.Distance;
						
						m_AnimationComponent.Play ("attack_distance");
						
						m_eAState = ActionState.ATTACK_DISTANCE;

						return true;
					}
				}
			}
		}
		#endregion

		return false;
	}
	#endregion

	#region Treasure
	private bool CheckTreasure(int iIndexX, int iIndexY)
	{
		if ((Mathf.Abs (iIndexX - m_iIndexPosX) > 1 || Mathf.Abs (iIndexY - m_iIndexPosY) > 0) &&
		    Mathf.Abs (iIndexX - m_iIndexPosX) > 0 || Mathf.Abs (iIndexY - m_iIndexPosY) > 1)
			return false;

		if (m_refMap.IsTreasurePileAtPosition(iIndexX, iIndexY))
		//if (m_refMap.m_arrMap[iIndexX, iIndexY].iBlockID == 20)
		{
			m_bHasTreasure = true;
			m_refTeam.m_arrTeams[m_iTeamID - 1].bTeamHasTreasure = true;
			//m_refMap.ChangeBlockIDInArea(0, iIndexX, iIndexY, 3, 3);
			m_refMap.ChangeBlockIDRecursevly(0, iIndexX, iIndexY, new int[] { 72, 90 } );
			m_goTreasureIcon.transform.parent = m_refSelfTransform;
			m_goTreasureIcon.transform.localPosition = new Vector3(0f, .6f, 0f);
			return true;
		}
		if (m_refMap.IsTreasureAtPosition(iIndexX, iIndexY))
		{
			// HACK HACK HACK HACK
			if (!m_refTeam.m_arrTeams[Mathf.Abs (m_iTeamID - 2)].bTeamHasTreasure)
			// HACK HACK HACK HACK
			{
				m_bHasTreasure = true;
				m_refTeam.m_arrTeams[m_iTeamID - 1].bTeamHasTreasure = true;
				//m_refMap.ChangeBlockIDInArea(0, iIndexX, iIndexY, 3, 3);
				m_goTreasureIcon.transform.parent = m_refSelfTransform;
				m_goTreasureIcon.transform.localPosition = new Vector3(0f, .6f, 0f);
				return true;
			}
		}
		// valid move
		return false;
	}

	// HACK HACK HACK HACK HACK
	private bool CheckTreasureToBase(int iIndexX, int iIndexY)
	{
		bool bAtBase = false;

		if (!m_bHasTreasure)
			return false;

		switch (m_iTeamID)
		{
		case 1:
			if (m_refMap.IsBaseTeam1AtPosition(iIndexX - 1, iIndexY) ||
			    m_refMap.IsBaseTeam1AtPosition(iIndexX + 1, iIndexY) ||
			    m_refMap.IsBaseTeam1AtPosition(iIndexX, iIndexY - 1) ||
			    m_refMap.IsBaseTeam1AtPosition(iIndexX, iIndexY + 1))
			{
				bAtBase = true;
			}
			break;
		case 2:
			if (m_refMap.IsBaseTeam2AtPosition(iIndexX - 1, iIndexY) ||
			    m_refMap.IsBaseTeam2AtPosition(iIndexX + 1, iIndexY) ||
			    m_refMap.IsBaseTeam2AtPosition(iIndexX, iIndexY - 1) ||
			    m_refMap.IsBaseTeam2AtPosition(iIndexX, iIndexY + 1))
			{
				bAtBase = true;
			}
			break;
		}

		if (bAtBase)
		{
			m_refTeam.m_arrTeams[m_iTeamID - 1].bVictoryTreasureObjectHunt = true;
			m_goTreasureIcon.transform.parent = null;
			m_goTreasureIcon.transform.position = new Vector3(-1, -1, -1);
			//Debug.Log (m_iTeamID + " brought the treasure home");
			return true;
		}

		return false;
	}
	// HACK HACK HACK HACK HACK
	#endregion

	#region ClassChanger
	private bool CheckClassChanger(int iIndexX, int iIndexY)
	{
		if ((Mathf.Abs (iIndexX - m_iIndexPosX) > 1 || Mathf.Abs (iIndexY - m_iIndexPosY) > 0) &&
		    Mathf.Abs (iIndexX - m_iIndexPosX) > 0 || Mathf.Abs (iIndexY - m_iIndexPosY) > 1)
			return false;
		if (!m_refMap.IsClassChangerAtPosition(iIndexX, iIndexY))
			return false;
		
		m_refHudSetup.m_bClassChangerActive = true;
		
		return true;
	}
	public bool ChangeClass(string strDwarfType)
	{
		MeshFilter dwarfMesh = GetComponent<MeshFilter> ();
		
		if (dwarfMesh == null)
		{
			dwarfMesh = GetComponentInChildren<MeshFilter>();
		}
		
		if (dwarfMesh == null)
		{
			Debug.LogError ("Class Change failed. No Meshfilter found.");
			return false;
		}
		
		foreach (Transform child in m_refSelfTransform)
		{
			Destroy(child.gameObject);
		}
		
		GameObject goNewChild = Instantiate(m_DwarfType.GetDwarfTypeByName(strDwarfType).goDwarfType,
		                                    m_refSelfTransform.position, m_refSelfTransform.rotation) as GameObject;
		goNewChild.transform.parent = m_refSelfTransform;
		
		//UpdateAnimationComponent();
		m_mrRenderer = (Renderer[])gameObject.GetComponentsInChildren<Renderer>();
		//Debug.Log (m_mrRenderer[m_mrRenderer.Length - 1].gameObject);
		// HACK HACK HACK HACK HACK HACK HACK HACK HACK
		if (m_iTeamID == 2)
		{
			switch (strDwarfType)
			{
				case "digger":
					m_mrRenderer[m_mrRenderer.Length - 1].material = m_DwarfType.matMinerTeam2;
					break;
				case "warrior":
					m_mrRenderer[m_mrRenderer.Length - 1].material = m_DwarfType.matWarriorTeam2;
					break;
				case "demolition_expert":
					m_mrRenderer[m_mrRenderer.Length - 1].material = m_DwarfType.matDemolitionTeam2;
					break;
			}
		}
		// HACK HACK HACK HACK HACK HACK HACK HACK HACK
		
		// change properties
		Team.struTeam selfTeam = m_refTeam.m_arrTeams[m_iTeamID - 1];
		int iDwarfID = 0;
		// HACK HACK HACK
		for (int i = 0; i <= 2; ++i)
		{
			if (strDwarfType.Equals(m_DwarfType.m_arrDwarfTypes[i].strCategory))
			{
				iDwarfID = i;
				break;
			}
		}
		// HACK HACK HACK
		m_byDwarfType = (byte)iDwarfID;
		float fMaxHealth = selfTeam.arr_fHealthPointsDifferentDwarfTypes[(int)m_byDwarfType];
		float fHealthPercentage = m_fHealth / fMaxHealth;
		
		float fNewMaxHealth = selfTeam.arr_fHealthPointsDifferentDwarfTypes[iDwarfID];
		m_fHealth = fHealthPercentage * fNewMaxHealth;
		
		m_fMeleeDmg = selfTeam.fDamageMeleeUpgrade;
		
		m_fDistanceDmg = selfTeam.fDamageDistantWeaponUpgrade;
		
		SetOutlineShader(m_fOutlineWidth, m_colOutlineColor);
		
		return true;
	}
	public void UpdateAnimationComponent()
	{
		m_AnimationComponent = null;
		
		m_AnimationComponent = gameObject.animation;
		if (m_AnimationComponent == null)
		{
			m_AnimationComponent = gameObject.GetComponentInChildren<Animation>();
		}
		if (m_AnimationComponent == null)
		{
			Debug.LogError ("Updating AnimationComponent failed. No Animation found.");
			return;
		}

		AnimationTimer animTimer = GameObject.Find ("DwarfareWorldObject").GetComponent<AnimationTimer>();

		m_dicAnimationLenghts.Clear();
		foreach (AnimationState animState in m_AnimationComponent)
		{
			float fTime = animTimer.GetAnimTimeByString(animState.name);
			if (fTime > 0)
			{
				animState.speed = animState.length / fTime;
			}
			m_dicAnimationLenghts.Add(animState.name, animState.length / animState.speed);
		}
		
		m_fWalkingSpeed = Map.m_fWidth / m_dicAnimationLenghts["walking"] * 8f;
	}
	#endregion

	#region Mining
	private bool StartMining(int iIndexX,int iIndexY)
	{
		if (!m_refMap.IsRecoverable(iIndexX, iIndexY))
			return false;
		if (m_refMap.IsPassable(iIndexX, iIndexY))
			return false;
		if (m_refTeam.m_arrTeams[m_iTeamID - 1].iActionpointsTeam < m_ActionConfig.Digging)
			return false;
		if (m_AnimationComponent.GetClip("digging") == null)
			return false;

		LoadLevelTexture.BlockType blockTypeAtPos = m_refMap.GetBlockTypeByID(m_refMap.m_arrMap[iIndexX, iIndexY].iBlockID);
		
		if (blockTypeAtPos == null)
		{
			Debug.LogWarning ("The ID: " + m_refMap.m_arrMap[iIndexX, iIndexY].iBlockID +
			                  " returns null as a block at the position " + iIndexX + ", " + iIndexY);
			return false;
		}

		if (!blockTypeAtPos.m_bDestroyable || m_iPickeAxeHardness < blockTypeAtPos.m_iHardness)
			return false;

		if ((Mathf.Abs (iIndexX - m_iIndexPosX) > 1 || Mathf.Abs (iIndexY - m_iIndexPosY) > 0) &&
		    Mathf.Abs (iIndexX - m_iIndexPosX) > 0 || Mathf.Abs (iIndexY - m_iIndexPosY) > 1)
		{
			Vector2 vec2MiningPos = GetMiningPosition(iIndexX, iIndexY);
			if (vec2MiningPos.x >= 0 && vec2MiningPos.y >= 0)
			{
				StartMovement((int)vec2MiningPos.x, (int)vec2MiningPos.y);
				m_eConnectingAState = ActionState.MINING;

				m_iMiningPositionX = iIndexX;
				m_iMiningPositionY = iIndexY;
			}
		}
		else
		{
			m_iMiningPositionX = iIndexX;
			m_iMiningPositionY = iIndexY;
			
			m_AnimationComponent.Play("digging");
			m_eAState = ActionState.MINING;
		}

		return true;
	}
	#endregion

	private bool BlockIsNeighbour(int iIndexX, int iIndexY, int iPosX, int iPosY)
	{
		if ((Mathf.Abs (iIndexX - iPosX) <= 1 && Mathf.Abs (iIndexY - iPosY) == 0) ||
		    Mathf.Abs (iIndexX - iPosX) == 0 && Mathf.Abs (iIndexY - iPosY) <= 1)
		{
			return true;
		}

		return false;
	}

	#region SetOulineShader
	public void SetOutlineShader(float fOutlineWidth, Color32 col32Outline)
	{
		m_fOutlineWidth = fOutlineWidth;
		m_colOutlineColor = col32Outline;
	
		Renderer parentRenderer = (Renderer)gameObject.GetComponent<Renderer>();

		if (parentRenderer != null)
		{
			foreach (Material mat in parentRenderer.materials)
			{
				if (mat.shader.name.Equals(m_strSilhouetteShader))
				{
					mat.SetFloat("_Outline", fOutlineWidth);
					mat.SetColor("_OutlineColor", col32Outline);
				}
			}
		}
		
		foreach (Renderer render in m_mrRenderer)
		{
			if (render == null)
				continue;
			
			foreach (Material mat in render.materials)
			{
				if (mat == null)
					continue;
					
				if (mat.shader.name.Equals(m_strSilhouetteShader))
				{
					mat.SetFloat("_Outline", fOutlineWidth);
					mat.SetColor("_OutlineColor", col32Outline);
				}
			}
		}
	}
	#endregion
}
