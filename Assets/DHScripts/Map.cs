using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class Map : MonoBehaviour {

	public static float m_fWidth = 1.6f;
	public static float m_fHeight = 1.6f;
	
	public static int m_iBoundsX = 32;
	public static int m_iBoundsY = 32;

	// Mauli: Sort variables by namespace and
	//		  and data type
	//		  ( namespace over data type )



	// References from other Scripts
	private Explosion refExplosion;
	private DwarfChar refDwarfChar;
	private Mouse2D mouse2D;
	private HotSeat refHotSeat;
	private LevelXMLSaver levelXMLSaver;

	private DwarfType refDwarfType;

	private Team refTeam;

	// for Leveltexture reading
	private Color[,] arrcolLevel;

	private bool[,] arrbLevel;

	private GameObject m_preBlock1;

	// Active Player
	public int m_iActivePlayerIDStart;
	public int m_iActivePlayerTeamIDStart;
	public int m_iCurrentLevel  = 0; // startLevel
	public int iCurrentLevel
	{
		get { return m_iCurrentLevel; }
	}

	public int[,] arriXML_Level;

	// Instantiates a prefab in a grid
	public GameObject m_preRasterSphere;
	public GameObject m_preExplosionShpere;
	
	public float m_gridX;
	public float m_gridY;

	public bool[,] m_arrbExplosionSize;
	public bool m_bCreateRaster;

	public bool m_bLoadFromTexture;
	public bool m_bLoadFromXML;

	public string m_strLoadingXMLLevelName;
	public string m_strSavingXMLLevelName;

	public GameObject projectile;

	public GameObject m_goTreasureObject;

	public TextAsset[] XMLLevel;

	private LoadLevelTexture m_loadLevelTexture;
	
	public LoadLevelTexture refLoadLevelTexture
	{
		get { return m_loadLevelTexture; }
		set { m_loadLevelTexture = value; }
	}

	public m_struBlock[,] m_arrMap;

	public struct m_struBlock
	{
		public int iBlockID;
		public int iXIndex;
		public int iYIndex;
		public GameObject preRasterSphere;
		public GameObject prefab1;
		public Vector3 vec3Position;
	}


	//private int iTest = 0;
	void Awake()
	{
		// get references
		refHotSeat = GetComponent<HotSeat>();
		refExplosion = GetComponent<Explosion> ();
		mouse2D = GetComponent<Mouse2D> ();
		refLoadLevelTexture = GetComponent<LoadLevelTexture> ();
		refDwarfType = GetComponent<DwarfType>();
		refTeam = GetComponent<Team>();

	StringReader xml = new StringReader(XMLLevel[PlayerPrefs.GetInt("MapSelection")].text);

		refLoadLevelTexture.loadLevelsFromTexture (); // loads all Levels from Textures and saves them to an array
		//levelXMLSaver = LevelXMLSaver.Load(Path.Combine(Application.dataPath,"DHLevelDesign/XmlMaps/" + m_strLoadingXMLLevelName));
		levelXMLSaver = LevelXMLSaver.ReleaseLoad(xml);

		//arriXML_Level = new int[levelXMLSaver.Extract().GetLength (0), levelXMLSaver.Extract().GetLength (1)];
		arriXML_Level = levelXMLSaver.Extract ();
	}

	// Use this for initialization
	void Start () 
	{
		Screen.showCursor = true;
		if(m_bLoadFromTexture)
		{
			// createNewLevelfromTexture (m_iCurrentLevel);
			m_bLoadFromXML = false;
			
		}
		if(m_bLoadFromXML)
		{
			// createMapFromXML ();
			m_bLoadFromTexture = false;
		}
	}

	// Update is called once per frame
	void Update () 
	{
		if (Input.GetKeyDown ("e")) // e for explode
		{
			Vector2 vec2ExplosionPoint =  new Vector2(Mathf.Round(mouse2D.HitGridIntX),Mathf.Round(mouse2D.HitGridIntY));
			if(refLoadLevelTexture.Levels[m_iCurrentLevel].m_tex2DLevel.width < vec2ExplosionPoint.x  ||
			   refLoadLevelTexture.Levels[m_iCurrentLevel].m_tex2DLevel.height < vec2ExplosionPoint.y ||
			   vec2ExplosionPoint.y < 0 || vec2ExplosionPoint.x < 0)
			{
				Debug.Log("Map: Explosion out of Level Size");
			}
			refExplosion.detonate(vec2ExplosionPoint, refExplosion.strFirstExplosionName, 3);
		}
		if (Input.GetKeyDown ("c")) // c for CLEAR
		{
			clearCurrentLevel();
		}
		if (Input.GetKeyDown ("p")) // p for POSITION
		{
			if(refTeam.GetDwarfCharByPosition((int)mouse2D.HitGridIntX,(int)mouse2D.HitGridIntY))
			{
				Debug.Log("Map: CurrentDwarf.iIndexPosX" + refTeam.GetDwarfCharByPosition((int)mouse2D.HitGridIntX,(int)mouse2D.HitGridIntY).iIndexPosX);
			}

		}
		if (Input.GetKeyDown("l")) // l for LOAD
		{
			if(m_bLoadFromTexture)
			{
				createNewLevelfromTextureWithoutDwarfes (m_iCurrentLevel);
				m_bLoadFromXML = false;

			}
			if(m_bLoadFromXML)
			{
				createMapFromXMLWithoutDwarfs ();
				m_bLoadFromTexture = false;
			}
		}
		if (Input.GetKeyDown ("m")) // m for meorize 
		{

			levelXMLSaver.Compress (arriXML_Level);
			levelXMLSaver.Save ("Assets/DHLevelDesign/XmlMaps/" + m_strSavingXMLLevelName);
			Debug.Log("Map: XML-level saved: " +  m_strSavingXMLLevelName);
		}
		if (Input.GetKeyDown("q"))
		{
			// TEST
			// ChangeBlockIDAtPosition(0, (int)mouse2D.HitGridIntX, (int)mouse2D.HitGridIntY);
			//DeleteDwarf(iTest, 1);
			//iTest++;
		}
			
		// TEST
		// Debug.Log ("Map : vec2ExplosionPoint x = " + vec2ExplosionPoint.x + " y = " + vec2ExplosionPoint.y);
	}

	public void createMapFromXMLWithoutDwarfs()
	{
		Debug.Log("Map: createMapfromXMLwithoutDWarfs()");
		clearCurrentLevel ();
		
		// Debug.Log ("Map: levelWidth = " + refLoadLevelTexture.Levels[m_iCurrentLevel].m_tex2DLevel.width);
		// Debug.Log ("Map: levelHeight = " + refLoadLevelTexture.Levels[m_iCurrentLevel].m_tex2DLevel.height);
		m_arrMap = new m_struBlock[levelXMLSaver.Extract().GetLength (0) ,levelXMLSaver.Extract().GetLength (1)];

		for (int y = 0; y < m_arrMap.GetLength (1); y++)
		{
			for( int x = 0; x < m_arrMap.GetLength (0); x++)
			{
				m_arrMap[x, y] = new m_struBlock ();
				m_arrMap[x, y].vec3Position = new Vector3(x * m_gridX, y * m_gridY , 0);
				m_arrMap[x, y].iXIndex = x;
				m_arrMap[x, y].iYIndex = y;
				
				if(arriXML_Level[x, y] == 0 && m_bCreateRaster)
				{
					// create Raster
					GameObject goRasterSphere = 
						Instantiate(m_preRasterSphere, new Vector3(m_arrMap[x, y].vec3Position.x, m_arrMap[x, y].vec3Position.y, 0),
						            Quaternion.identity) as GameObject;
					m_arrMap[x, y].preRasterSphere = goRasterSphere;
				}
				
				// load Block
				GameObject go2BlockAtPosition = refLoadLevelTexture.GetBlockPrefabByTypeID(arriXML_Level[x,y]);
				// if block has Dwarf ID
				if(refDwarfType.getDwarfTypeByBlockTypeID(arriXML_Level[x, y]) != null)
				{
				
				}
				else // no Dwarf
				{
					if(go2BlockAtPosition != null)
					{
						GameObject goBlockAtPosition = Instantiate(go2BlockAtPosition,
						                                           new Vector3(m_arrMap[x, y].vec3Position.x, m_arrMap[x, y].vec3Position.y, 0),
						                                           go2BlockAtPosition.transform.rotation) as GameObject;
						m_arrMap[x, y].prefab1  = goBlockAtPosition;
					}
				}
				
				if(refLoadLevelTexture.GetBlockTypeByTypeID(arriXML_Level[x, y]) != null)
				{
					m_arrMap[x, y].iBlockID  = refLoadLevelTexture.GetBlockTypeByTypeID(arriXML_Level[x, y]).iTypeID;
					// Treasure and Additionalblocks
					if(m_arrMap[x,y].iBlockID == 72) // is it Treasure? 
					{
						refTeam.SaveTreasurePositionAtStart(x, y);
						SetBlockIDAtPositionwithWidthHeightWithoutDestroy(90, x, y, 2, 2);
					}
					if(m_arrMap[x, y].iBlockID == 73) // is it ClassChanger
					{
						SetBlockIDAtPositionwithWidthHeightWithoutDestroy(91, x, y, 2, 2);
					}
					if(m_arrMap[x, y].iBlockID == 70) // is it Base Team 1
					{
						SetBlockIDAtPositionwithWidthHeightWithoutDestroy(92, x, y, 4, 4);
					}
					if(m_arrMap[x, y].iBlockID == 71) // is it Base Team 2
					{
						SetBlockIDAtPositionwithWidthHeightWithoutDestroy(93, x, y, 4, 4);
					}
				}
				else
				{
					m_arrMap[x, y].iBlockID = 0; // no block									
				}				
			}
		}
	}
		
	public void createNewLevelfromTextureWithoutDwarfes(int _iLevel)
	{
		Debug.Log("Map: createMapfromXMLwithoutDWarfs()");
		clearCurrentLevel ();
		
		// Debug.Log ("Map: levelWidth = " + refLoadLevelTexture.Levels[m_iCurrentLevel].m_tex2DLevel.width);
		// Debug.Log ("Map: levelHeight = " + refLoadLevelTexture.Levels[m_iCurrentLevel].m_tex2DLevel.height);
		m_arrMap = new m_struBlock[levelXMLSaver.Extract().GetLength (0) ,levelXMLSaver.Extract().GetLength (1)];

		for (int y = 0; y < m_arrMap.GetLength (1); y++)
		{
			for( int x = 0; x < m_arrMap.GetLength (0); x++)
			{
				m_arrMap[x, y] = new m_struBlock ();
				m_arrMap[x, y].vec3Position = new Vector3(x * m_gridX, y * m_gridY , 0);
				m_arrMap[x, y].iXIndex = x;
				m_arrMap[x, y].iYIndex = y;


				// CHECK != -1
				if(refLoadLevelTexture.GetBlockIDAtPositionFromTexture(_iLevel, x , y) != -1)
				{
					m_arrMap [x, y].iBlockID = refLoadLevelTexture.GetBlockIDAtPositionFromTexture(_iLevel, x, y);
					arriXML_Level[x, y] = m_arrMap [x, y].iBlockID;
				}

				if(arriXML_Level[x, y] == 0 && m_bCreateRaster)
				{
					// create Raster
					GameObject goRasterSphere = 
						Instantiate(m_preRasterSphere, new Vector3(m_arrMap[x, y].vec3Position.x, m_arrMap[x, y].vec3Position.y, 0),
						            Quaternion.identity) as GameObject;
					m_arrMap[x, y].preRasterSphere = goRasterSphere;
				}
				
				// load Block
				GameObject go2BlockAtPosition = refLoadLevelTexture.GetBlockPrefabByTypeID(arriXML_Level[x,y]);
				// if block has Dwarf ID
				if(refDwarfType.getDwarfTypeByBlockTypeID(arriXML_Level[x, y]) != null)
				{
					
				}
				else // no Dwarf
				{
					if(go2BlockAtPosition != null)
					{
						GameObject goBlockAtPosition = Instantiate(go2BlockAtPosition,
						                                           new Vector3(m_arrMap[x, y].vec3Position.x, m_arrMap[x, y].vec3Position.y, 0),
						                                           go2BlockAtPosition.transform.rotation) as GameObject;
						m_arrMap[x, y].prefab1  = goBlockAtPosition;
					}
				}
				
				if(refLoadLevelTexture.GetBlockTypeByTypeID(arriXML_Level[x, y]) != null)
				{
					m_arrMap[x, y].iBlockID  = refLoadLevelTexture.GetBlockTypeByTypeID(arriXML_Level[x, y]).iTypeID;
					arriXML_Level[x, y] = m_arrMap[x, y].iBlockID;
					// Treasure and Additionalblocks
					if(m_arrMap[x,y].iBlockID == 72) // is it Treasure? 
					{
						refTeam.SaveTreasurePositionAtStart(x, y);
						SetBlockIDAtPositionwithWidthHeightWithoutDestroy(90, x, y, 2, 2);
					}
					if(m_arrMap[x, y].iBlockID == 73) // is it ClassChanger
					{
						SetBlockIDAtPositionwithWidthHeightWithoutDestroy(91, x, y, 2, 2);
					}
					if(m_arrMap[x, y].iBlockID == 70) // is it Base Team 1
					{
						SetBlockIDAtPositionwithWidthHeightWithoutDestroy(92, x, y, 4, 4);
					}
					if(m_arrMap[x, y].iBlockID == 71) // is it Base Team 2
					{
						SetBlockIDAtPositionwithWidthHeightWithoutDestroy(93, x, y, 4, 4);
					}
				}
				else
				{
					m_arrMap[x, y].iBlockID = 0; // no block	
					arriXML_Level[x, y] = 0;
				}				
			}
		}
	}



	void clearCurrentLevel()
	{
		for (int y = 0; y < refLoadLevelTexture.Levels[m_iCurrentLevel].m_tex2DLevel.height; y++) 
		{
			for (int x = 0; x < refLoadLevelTexture.Levels[m_iCurrentLevel].m_tex2DLevel.width; x++) 
			{
				if(m_arrMap != null)
				{
					Destroy (m_arrMap [x, y].prefab1);
				}
			}
		}
	}

	public bool IsRecoverable(int _iXGrid, int _iYGrid)
	{
		if(GetBlockTypeByID(m_arrMap[_iXGrid, _iYGrid].iBlockID).m_bIsRecoverable)
		{
			return true;
		}
		else
		{
			return false;
		}		       
	}

	public bool IsBaseTeam1AtPosition( int _iXGrid, int _iYGrid)
	{
		if(m_arrMap[_iXGrid, _iYGrid].iBlockID == 70 || 
		   m_arrMap[_iXGrid, _iYGrid].iBlockID == 92)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	public bool IsBaseTeam2AtPosition( int _iXGrid, int _iYGrid)
	{
		if(m_arrMap[_iXGrid, _iYGrid].iBlockID == 71 || 
		   m_arrMap[_iXGrid, _iYGrid].iBlockID == 93)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	public bool IsTreasurePileAtPosition(int _iXGrid, int _iYGrid)
	{
		if(m_arrMap[_iXGrid, _iYGrid].iBlockID == 72 ||
		 m_arrMap[_iXGrid, _iYGrid].iBlockID == 90)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	public bool IsClassChangerAtPosition(int _iXGrid, int _iYGrid)
	{
		if(m_arrMap[_iXGrid, _iYGrid].iBlockID == 73 ||
		   m_arrMap[_iXGrid, _iYGrid].iBlockID == 91)
		{
			return true;
		}
		else
		{
			return false;
		}
	}
	

	public void SetBlockIDAtPositionwithWidthHeightWithoutDestroy(int _iNewBlockID, int _iXGridOrigin,
	                                                              int _iYGridOrigin, int _iWidth, int _iHeight)
	{
		// Debug.Log("Map: set new IDs next to base");
		int iBlockIDOrigin = m_arrMap[_iXGridOrigin, _iYGridOrigin].iBlockID;
		for(int x = 0; x <  _iWidth; x++)
		{
			for (int y = 0; y < _iHeight; y++)
			{
//				Debug.Log("Map: set position");
				m_arrMap[_iXGridOrigin + x,  _iYGridOrigin + y].iBlockID = _iNewBlockID;
				arriXML_Level[_iXGridOrigin + x,  _iYGridOrigin + y] = _iNewBlockID;
			}
		}

		// set OriginBlockID back
		m_arrMap[_iXGridOrigin, _iYGridOrigin].iBlockID = iBlockIDOrigin;
		arriXML_Level[_iXGridOrigin, _iYGridOrigin] = iBlockIDOrigin;
	}
	
	public void ChangeBlockIDRecursevly(int iNewBlockID, int iOriginX, int iOriginY, int[] arriBlockFilter)
	{
		if (iOriginX < 0 || iOriginX >= m_arrMap.GetLength(0) ||
			iOriginY < 0 || iOriginY >= m_arrMap.GetLength(1))
		{
			return;
		}
	
		for (int i = 0; i < arriBlockFilter.Length; ++i)
		{
			if (m_arrMap[iOriginX, iOriginY].iBlockID == arriBlockFilter[i])
			{
				ChangeBlockIDAtPosition(iNewBlockID, iOriginX, iOriginY);
				
				ChangeBlockIDRecursevly(iNewBlockID, iOriginX - 1, iOriginY, arriBlockFilter);
				ChangeBlockIDRecursevly(iNewBlockID, iOriginX + 1, iOriginY, arriBlockFilter);
				ChangeBlockIDRecursevly(iNewBlockID, iOriginX, iOriginY - 1, arriBlockFilter);
				ChangeBlockIDRecursevly(iNewBlockID, iOriginX, iOriginY + 1, arriBlockFilter);
				
				break;
			}
		}
	}
	
	public void ChangeBlockIDInArea(int iNewBlockID, int iOriginX, int iYOriginY, int iWidth, int iHeight)
	{
		for (int x = 0; x < iWidth; ++x)
		{
			for (int y = 0; y < iHeight; ++y)
			{
				ChangeBlockIDAtPosition (iNewBlockID, iOriginX + x, iYOriginY + y);
			}
		}
	}

	public bool ChangeBlockIDAtPosition( int _iBlockID, int _iXGrid, int _iYGrid)
	{
		int CurrentBlockID = m_arrMap[_iXGrid, _iYGrid].iBlockID;

		m_arrMap[_iXGrid, _iYGrid].iBlockID = _iBlockID;
		// only destroy block if it is not a player
		if(CurrentBlockID != 74 && CurrentBlockID != 75 && CurrentBlockID != 76 &&
		   CurrentBlockID != 80 && CurrentBlockID != 81 && CurrentBlockID != 82)
		{
			Destroy(m_arrMap[_iXGrid, _iYGrid].prefab1);
		}
		GameObject Prefab = new GameObject();
		Prefab = GetBlockTypeByID(_iBlockID).m_preBlock;
		
		if (Prefab == null)
			return false;
		
		GameObject Clone = Instantiate( Prefab, m_arrMap[_iXGrid, _iYGrid].vec3Position, Prefab.transform.rotation) as GameObject;
		m_arrMap[_iXGrid, _iYGrid].prefab1 = Clone;
		return true;
	}

	// ignores actual treasurepile
	public bool IsTreasureAtPosition(int iIndexX, int iIndexY)
	{
		if (m_goTreasureObject == null)
		{
			Debug.LogWarning ("Variable m_goTreasureObject unassigned.");
			return false;
		}
		Vector2 vec2Pos = WorldToIndex(m_goTreasureObject.transform.position);

		if (iIndexX == (int)vec2Pos.x && iIndexY == (int)vec2Pos.y)
		{
			return true;
		}

		return false;
	}

	/*public bool IsDwarfAtPosition(int _iXGrid, int _iYGrid)
	{
		if( m_arrMap[_iXGrid, _iYGrid].iBlockID == 74 || m_arrMap[_iXGrid, _iYGrid].iBlockID == 75 || m_arrMap[_iXGrid, _iYGrid].iBlockID == 76 
		 || m_arrMap[_iXGrid, _iYGrid].iBlockID == 80 || m_arrMap[_iXGrid, _iYGrid].iBlockID == 81 || m_arrMap[_iXGrid, _iYGrid].iBlockID == 82 )
		{
			Debug.Log("Dwarf at Position " + _iXGrid + " " + _iYGrid);
			return true;
		}
		else
			return false;
	}

	public bool IsDwarfOfTeam1(int _iXGrid, int _iYGrid)
	{
		if(m_arrMap[_iXGrid, _iYGrid].iBlockID == 74 || m_arrMap[_iXGrid, _iYGrid].iBlockID == 75 || m_arrMap[_iXGrid, _iYGrid].iBlockID == 76)
		{
			Debug.Log("Dwarf is from Team 1");
			return true;
		}
		else
			return false;
	}

	public bool IsDwarfOfTeam2(int _iXGrid, int _iYGrid)
	{
		if(m_arrMap[_iXGrid, _iYGrid].iBlockID == 80 || m_arrMap[_iXGrid, _iYGrid].iBlockID == 81 || m_arrMap[_iXGrid, _iYGrid].iBlockID == 82)
		{
			Debug.Log("Dwarf is from Team 2");
			return true;
		}
		else
			return false;
	}

	public bool IsActiveDwarfAtPosition(int _iXGrid, int _iYGrid)
	{
		DwarfChar ActiveDwarf = refHotSeat.GetActiveDwarf();
		if(ActiveDwarf)
		{
			if(ActiveDwarf.iIndexPosX == _iXGrid && ActiveDwarf.iIndexPosY == _iYGrid)
			{
				Debug.Log("Active Dwarf is at Position " + _iXGrid + " " + _iYGrid);
				return true;
			}
			else
				return false;
		}
		else 
		{
			Debug.Log("No Active Dwarf");
			return false;
		}
	}

	public bool IsDwarfFromActiveTeam(int _iXGrid, int _iYGrid)
	{
		if(IsDwarfAtPosition(_iXGrid, _iYGrid))
		{
			int iActiveTeam = refHotSeat.GetActiveTeam();
			if(iActiveTeam == 1)
			{
				if(IsDwarfOfTeam1(_iXGrid, _iYGrid))
				{
					Debug.Log("Dwarf is From Active Team " + iActiveTeam);
					return true;
				}
				else
					return false;
			}
			if(iActiveTeam == 2)
			{
				if(IsDwarfOfTeam2(_iXGrid, _iYGrid))
				{
					Debug.Log("Dwarf is From Active Team " + iActiveTeam);
					return true;
				}
				else 
					return false;
			}
			else
			{
				Debug.Log("Active Team isnt 1 or 2");
				return false;
			}
		}
		else
		{
			Debug.Log("There is no Dwarf At Position");
				return false;
		}
	}*/



	public LoadLevelTexture.BlockType GetBlockTypeByID (int _iID)
	{
		for (int i = 0; i < refLoadLevelTexture.BlockTypes.Length; i ++)
		{
			if(_iID == refLoadLevelTexture.BlockTypes[i].iTypeID)
			{
				return refLoadLevelTexture.BlockTypes[i];
			}
		}
		return null;
	}


	public bool IsSolid(int iIndexX, int iIndexY)
	{
		if (iIndexX >= 0 &&  iIndexX < 2 * m_iBoundsX && iIndexY >= 0 && iIndexY < 2 * m_iBoundsY)
		{
			return GetBlockTypeByID(m_arrMap[iIndexX, iIndexY].iBlockID).m_bIsSolid;
		}
		return false;
	}
	
	public bool IsPassable(int iIndexX, int iIndexY)
	{
		if (iIndexX >= 0 && iIndexX < 2 * m_iBoundsX && iIndexY >= 0 && iIndexY < 2 * m_iBoundsY)
		{
			return GetBlockTypeByID(m_arrMap[iIndexX, iIndexY].iBlockID).m_bPassable;
		}
		return false;
	}
	
	public bool IsClimbable(int iIndexX, int iIndexY)
	{
		if (iIndexX >= 0 && iIndexX < 2 * m_iBoundsX && iIndexY >= 1 && iIndexY < 2 * m_iBoundsY)
		{
			return GetBlockTypeByID(m_arrMap[iIndexX, iIndexY].iBlockID).m_bIsClimbable;
		}
		return false;
	}
	
	public bool IsInAir(int iIndexX, int iIndexY)
	{
		if (iIndexX >= 0 && iIndexX < 2 * m_iBoundsX && iIndexY >= 1 && iIndexY < 2 * m_iBoundsY)
		{
			if (!IsSolid(iIndexX, iIndexY - 1))
				return true;
		}
		return false;
	}

	public bool IsEmpty(int iIndexX, int iIndexY)
	{
		if (iIndexX >= 0 && iIndexX < 2 * m_iBoundsX && iIndexY >= 1 && iIndexY < 2 * m_iBoundsY)
		{
			return (m_arrMap[iIndexX, iIndexY].iBlockID == 0 && refTeam.GetDwarfCharByPosition(iIndexX, iIndexY) == null);
		}
		return false;
	}

	public Vector2 GetPlayerSpawn()
	{
		for (int x = 0; x < m_arrMap.GetLength(0); ++x)
		{
			for (int y = 0; y < m_arrMap.GetLength(1); ++y)
			{
				if (arriXML_Level[x, y] == 73 ||
				    arriXML_Level[x, y] == 74 ||
				    arriXML_Level[x, y] == 75)
				{
					return new Vector2(x, y);
				}
			}
		}

		return new Vector2(-1, -1);
	}

	public static Vector2 WorldToIndex(Vector2 vec2WorldPos)
	{
		return new Vector2(Mathf.RoundToInt(vec2WorldPos.x / m_fWidth), Mathf.RoundToInt (vec2WorldPos.y / m_fHeight));
	}
	
	public static Vector2 IndexToWorldOffset(int iIndexX, int iIndexY)
	{
		return new Vector2(iIndexX * m_fWidth, iIndexY * m_fHeight);
	}

	public static Vector3 IndexToWorldOffset(float iIndexX, float iIndexY, float iIndexZ)
	{
		return new Vector3(iIndexX * m_fWidth, iIndexY * m_fHeight, iIndexZ);
	}

	public static Vector2 IndexToWorldNoOffset(int iIndexX, int iIndexY)
	{
		return new Vector2(
			(iIndexX - m_fWidth) * m_fWidth + m_fWidth / 2.0f,
			(iIndexY - m_fHeight) * m_fHeight + m_fHeight / 2.0f);
	}
}

