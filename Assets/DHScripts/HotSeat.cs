using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HotSeat : MonoBehaviour {

	// references from other scripts
	private Team refTeam;
	private Mouse2D refMouse2D;
	private Map refMap;
	private DwarfType refDwarfType;

	// Hot Seat active?
	public bool m_bHotSeatActive;

	// Times adjusted in editor
	public int m_iPlayPhaseTime;
	public int m_iGetReadyTime;
	public int m_iShowtimeForGUI;

	// Outline
	public Color32 m_col32OutlineNotActive;
	public Color32 m_col32Team1Active;
	public Color32 m_col32Team1NextActive;
	public Color32 m_col32Team2Active;
	public Color32 m_col32Team2NextActive;

	// Coin
	public GameObject m_goTeamCoin;
	public float m_fAccelerationFaktorForTeamCoin; // standard 4
	public float m_fRotationAmountTeamCoinPerFrame; // standard 180
	public float m_fCoinDistanceToCamera; // standard 1
	public float m_fCoinYOffsetToCamera;
	public float m_fCoinShowTimeTurning; // standard 4
	public float m_fCoinShowTimeChosenTeam; // standard 2
	private bool m_bRotateCoin;

	// TimeCounter
	private int m_iPlayPhaseTimeCounter;
	public int iPlayPhaseTimeCounter
	{
		get { return m_iPlayPhaseTimeCounter; }
		set { m_iPlayPhaseTimeCounter = value; }
	}
	
	private int m_iCountdownInSeconds;
	public int iCountdownInSeconds
	{
		get { return m_iCountdownInSeconds; }
		set { m_iCountdownInSeconds = value; }
	}

	private GameObject m_goMainCamera;
	private GameObject goTeamCoinClone;
	private Vector3 vec3InFrontOfMainCamera;
	private List<Vector2> m_lisarrPositionsTeam1;
	private List<Vector2> m_lisarrPositionsTeam2;
	private int m_iCounterForEverySecondRound;
	private int m_iCurrentTeam;
	private int m_iIDCounterTeam1;
	private int m_iIDCounterTeam2;
	private int m_iCounterEveryPlayPhase;
	public int m_iGetReadyCounterForGUI;
	public int m_iPlayPhaseCounterForGUI;
	public int m_iActiveTeam; 
	private bool m_bStartCountdownPlayPhase;
	private bool m_bGetReadyPhase;
	private bool m_bPlayPhase;
	public bool m_bPausePhase;
	private string m_strLastInvoke;

	DwarfChar ActiveDwarf;
	DwarfChar NextActiveDwarf;

	void Awake()
	{
		// get References
		refTeam = GetComponent<Team>();
		refMouse2D = GetComponent<Mouse2D>();
		refMap = GetComponent<Map>();
		refDwarfType = GetComponent<DwarfType>();
	}
	
	// Use this for initialization
	void Start () 
	{
		m_iCountdownInSeconds = m_iGetReadyTime;
		m_iGetReadyCounterForGUI = m_iCountdownInSeconds;
		m_iPlayPhaseCounterForGUI = m_iPlayPhaseTimeCounter;
		m_strLastInvoke = "DecreaseGetReadyCountdown";

		m_goMainCamera = GameObject.FindWithTag("MainCamera");
		m_iCountdownInSeconds = m_iGetReadyTime;
		m_iActiveTeam = 0;

		// createMapWithoutDwarfs
		refMap.createMapFromXMLWithoutDwarfs();
		// refMap.createNewLevelfromTextureWithoutDwarfes(refMap.iCurrentLevel);

		// getStartPositions
		GetStartPositionsDwarfFromXMLTeam1();
		GetStartPositionsDwarfFromXMLTeam2();

		// choose Dwarfs and start Hot seat
		ChooseDwarfs();


	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			m_bPausePhase = !m_bPausePhase;
			if(!m_bPausePhase)
			{
				InvokeRepeating(m_strLastInvoke, 1f, 1f);	
			}
		}

		// if TeamCoin is available
		if(goTeamCoinClone)
		{
			// Update Coinposiiton
			vec3InFrontOfMainCamera = new Vector3(m_goMainCamera.transform.position.x,
			                                      m_goMainCamera.transform.position.y + m_fCoinYOffsetToCamera,
			                                      m_goMainCamera.transform.position.z + m_fCoinDistanceToCamera);
			goTeamCoinClone.transform.position = vec3InFrontOfMainCamera;

			// rotateCoin
			if(m_bRotateCoin) 
			{
				// accalerate coinrotation
				m_fRotationAmountTeamCoinPerFrame += m_fAccelerationFaktorForTeamCoin;
				goTeamCoinClone.transform.Rotate(0, m_fRotationAmountTeamCoinPerFrame * Time.deltaTime, 0);
			}
		}
	}

	void StartPause()
	{
		if(!m_bPausePhase)
		{
			Debug.Log("HotSeat: start pause ");
			InvokeRepeating(m_strLastInvoke, 1f, 1f);	
		}
	}

	void StopPause()
	{

	}
	
	
	void ChooseDwarfs()
	{
		int iIDDwarfTeam1 = 0;
		int iIDDwarfTeam2 = 0;
		int iTeam1 = 1;
		int iTeam2 = 2;

		int[] arriDwarftypesTeam1 = PlayerPrefsX.GetIntArray("Team1IDs");
		int[] arriDwarftypesTeam2 = PlayerPrefsX.GetIntArray("Team2IDs");
		if(arriDwarftypesTeam1 != null && arriDwarftypesTeam2 != null)
		{
			foreach(int iter in arriDwarftypesTeam1)
			{
				SpwanDwarf(iter, iTeam1, iIDDwarfTeam1);
				iIDDwarfTeam1++;
			}

			foreach(int iter in arriDwarftypesTeam2)
			{
				SpwanDwarf(iter, iTeam2, iIDDwarfTeam2);
				iIDDwarfTeam2++;
			}
		}
		else
		{
			Debug.Log("HotSeat: Spawn random dwarfs ");
			// Team 1, Dwarf0
			SpwanDwarf(Random.Range(0,3), iTeam1, iIDDwarfTeam1);
			iIDDwarfTeam1++;
			
			// Team 2, Dwarf0
			SpwanDwarf(Random.Range(0,3), iTeam2, iIDDwarfTeam2);
			iIDDwarfTeam2++;
			
			// Team 2, Dwarf1
			SpwanDwarf(Random.Range(0,3), iTeam2, iIDDwarfTeam2);
			iIDDwarfTeam2++;
			
			// Team 1, Dwarf1
			SpwanDwarf(Random.Range(0,3), iTeam1, iIDDwarfTeam1);
			iIDDwarfTeam1++;
			
			// Team 1, Dwarf2
			SpwanDwarf(Random.Range(0,3), iTeam1, iIDDwarfTeam1);
			iIDDwarfTeam1++;
			
			// Team 2, Dwarf2
			SpwanDwarf(Random.Range(0,3), iTeam2, iIDDwarfTeam2);
			iIDDwarfTeam2++;
			
			// Team 2, Dwarf3
			SpwanDwarf(Random.Range(0,3), iTeam2, iIDDwarfTeam2);
			iIDDwarfTeam2++;
			
			// Team 1, Dwarf3
			SpwanDwarf(Random.Range(0,3), iTeam1, iIDDwarfTeam1);
			iIDDwarfTeam1++;
		}

		StartCoroutine("SpwanCoin");
	}

	IEnumerator SpwanCoin()
	{
		if(m_goTeamCoin)
		{
			vec3InFrontOfMainCamera = new Vector3(m_goMainCamera.transform.position.x,
			                                      m_goMainCamera.transform.position.y + m_fCoinYOffsetToCamera,
			                                      m_goMainCamera.transform.position.z + m_fCoinDistanceToCamera);
			goTeamCoinClone = Instantiate(m_goTeamCoin, vec3InFrontOfMainCamera, m_goTeamCoin.transform.rotation) as GameObject;
			m_bRotateCoin = true;
		}
		else
		{
			Debug.Log("HotSeat: No goTeamCoin on HotSeatScript");
		}

		yield return new WaitForSeconds(m_fCoinShowTimeTurning);

		m_bRotateCoin = false;
		StartCoroutine("StopCoinAndShowTeam");
	}

	IEnumerator StopCoinAndShowTeam()
	{
		// which team starts?
		int iMin = 1; // inclusive
		int iMax = 3; // exclusive
		m_iCounterEveryPlayPhase = Random.Range(iMin, iMax);

		if(m_iCounterEveryPlayPhase == 1)
		{
			goTeamCoinClone.transform.eulerAngles = new Vector3(0, 0, 0);
		}
		if(m_iCounterEveryPlayPhase == 2)
		{
			goTeamCoinClone.transform.eulerAngles = new Vector3(0, 180, 0);
		}

		yield return new WaitForSeconds(m_fCoinShowTimeChosenTeam);
		Destroy(goTeamCoinClone);
		// PlayPhase
		StartHotSeat();
	}

	void SpwanDwarf(int _iDwarfType, int _iTeam, int _iIDDwarfTeam)
	{


		if(_iTeam == 1)
		{
			// Get CurrentSpawnPosition
			Vector3 vec3CurrentPositionTeam1 = new Vector3(m_lisarrPositionsTeam1[_iIDDwarfTeam].x, m_lisarrPositionsTeam1[_iIDDwarfTeam].y, 0);
			// Instantiate Dwarf
			GameObject goCurrentDwarf = Instantiate(refDwarfType.m_arrDwarfTypes[_iDwarfType].goDwarfType, vec3CurrentPositionTeam1, 
			                                        refDwarfType.m_arrDwarfTypes[_iDwarfType].goDwarfType.transform.rotation) as GameObject;



			// Add DwarfCharScript to current dwarf for move and Gameplay
			goCurrentDwarf.AddComponent<DwarfChar>();
			DwarfChar refDwarfChar = goCurrentDwarf.GetComponent<DwarfChar>();
			
			refDwarfChar.Initialize(refDwarfType.m_arrDwarfTypes[_iDwarfType].strCategory, vec3CurrentPositionTeam1, _iTeam, _iIDDwarfTeam);
			refDwarfChar.SetOutlineShader(0f, m_col32OutlineNotActive);
			refTeam.m_arrTeams[_iTeam - 1].lisDwarfs.Add(refDwarfChar);
		}
		if(_iTeam == 2)
		{
			// Get CurrentSpawnPosition
			Vector3 vec3CurrentPositionTeam2 = new Vector3(m_lisarrPositionsTeam2[_iIDDwarfTeam].x, m_lisarrPositionsTeam2[_iIDDwarfTeam].y, 0);
			// Instantiate Dwarf
			GameObject goCurrentDwarf = Instantiate(refDwarfType.m_arrDwarfTypes[_iDwarfType].goDwarfType, vec3CurrentPositionTeam2, 
			                                        refDwarfType.m_arrDwarfTypes[_iDwarfType].goDwarfType.transform.rotation) as GameObject;

			SkinnedMeshRenderer CurrentRenderer = goCurrentDwarf.GetComponentInChildren<SkinnedMeshRenderer>();
			if(_iDwarfType == 0)
			{
				CurrentRenderer.material = refDwarfType.matMinerTeam2;
			}
			if(_iDwarfType == 1)
			{
				CurrentRenderer.material = refDwarfType.matWarriorTeam2;
			}
			if(_iDwarfType == 2)
			{
				CurrentRenderer.material = refDwarfType.matDemolitionTeam2;
			}

			// Add DwarfCharScript to current dwarf for move and Gameplay
			goCurrentDwarf.AddComponent<DwarfChar>();
			DwarfChar refDwarfChar = goCurrentDwarf.GetComponent<DwarfChar>();
			
			refDwarfChar.Initialize(refDwarfType.m_arrDwarfTypes[_iDwarfType].strCategory, vec3CurrentPositionTeam2, _iTeam, _iIDDwarfTeam);
			refDwarfChar.SetOutlineShader(0f, m_col32OutlineNotActive);
			refTeam.m_arrTeams[_iTeam - 1].lisDwarfs.Add(refDwarfChar);
		}
	}
	

	void StartHotSeat()
	{
		if(m_bHotSeatActive)
		{
			GetReadyPhase();
		}
	}
		
	void GetReadyPhase()
	{
		m_bGetReadyPhase = true;
		// set all Dwarfs to not active
		for (int i = 0; i < refTeam.m_arrTeams.Length; i ++)
		{
			foreach(DwarfChar iter in refTeam.m_arrTeams[i].lisDwarfs)
			{
				iter.Active = false;
				iter.SetOutlineShader(0f, m_col32OutlineNotActive);
			}
		}

		m_iCountdownInSeconds = m_iGetReadyTime;
		if(!m_bPausePhase)
		{
			InvokeRepeating("DecreaseGetReadyCountdown" , 1f , 1f );
		}
	}

	void PlayPhase()
	{
		// Debug.Log("HotSeat: PlayPhase ");
		m_bPlayPhase = true;

		// setActionpoints back
		refTeam.SetActionpointsToStartActionpointsForAllTeams();
		// CHECK is Map: loadfromXML aktive
		m_iActiveTeam = m_iCurrentTeam;
		ActiveDwarf = NextActiveDwarf;
		ActiveDwarf.Active = true;
		if(ActiveDwarf.iTeamID == 1)
		{
			ActiveDwarf.SetOutlineShader(0.005f, m_col32OutlineNotActive);
			ActiveDwarf.SetOutlineShader(0.005f, m_col32Team1Active);
		}
		if(ActiveDwarf.iTeamID == 2)
		{
			ActiveDwarf.SetOutlineShader(0.005f, m_col32OutlineNotActive);
			ActiveDwarf.SetOutlineShader(0.005f, m_col32Team2Active);
		}
							
		m_iPlayPhaseTimeCounter = m_iPlayPhaseTime;
		if(!m_bPausePhase)
		{
			InvokeRepeating("DecreasePlayPhaseCounter",1f , 1f);
		}
	}

	void DecreasePlayPhaseCounter() // Decreases every Second
	{
		m_iPlayPhaseCounterForGUI = m_iPlayPhaseTimeCounter;
		if(m_bPausePhase)
		{
			m_strLastInvoke = "DecreasePlayPhaseCounter";
			CancelInvoke();
		}

		if(m_iPlayPhaseTimeCounter <= 0)
		{
			m_bPlayPhase = false;
			CancelInvoke();
			GetReadyPhase();
		}
		m_iPlayPhaseTimeCounter--;
	}

	void DecreaseGetReadyCountdown() // Decreases every Second
	{
		m_iGetReadyCounterForGUI = m_iCountdownInSeconds;
		if(m_bPausePhase)
		{
			m_strLastInvoke = "DecreaseGetReadyCountdown";
			CancelInvoke();
		}
		// zoom to next active Dwarf one second before it starts
		if(m_iCountdownInSeconds == m_iGetReadyTime - 1)
		{

			// set Active Player
			m_iCounterEveryPlayPhase++;
			if (m_iCounterEveryPlayPhase % 2 == 1) // Team1
			{
				m_iCurrentTeam = 1;

				m_iIDCounterTeam1++;
				m_iIDCounterTeam2++;
			}
			else 								   // Team2
			{
				m_iCurrentTeam = 2;
			}


			int iAmountDwarfsInTeam1 = refTeam.m_arrTeams[1 - 1].lisDwarfs.Count;
			int iAmountDwarfsInTeam2 = refTeam.m_arrTeams[2 - 1].lisDwarfs.Count;

			if(m_iIDCounterTeam1 >= iAmountDwarfsInTeam1)
			{
				m_iIDCounterTeam1 = 0;
			}
			if(m_iIDCounterTeam2 >= iAmountDwarfsInTeam2)
			{
				m_iIDCounterTeam2 = 0;
			}

			if(m_iCurrentTeam == 1)
			{

				if(refTeam.m_arrTeams[m_iCurrentTeam - 1].lisDwarfs[m_iIDCounterTeam1] != null)
				{
					NextActiveDwarf = refTeam.m_arrTeams[m_iCurrentTeam - 1].lisDwarfs[m_iIDCounterTeam1];
				}
				else 
				{
					Debug.Log("HotSeat: all dwarfs destroyed in team " +  m_iCurrentTeam);
				}
			}
			if(m_iCurrentTeam == 2)
			{
				if(refTeam.m_arrTeams[m_iCurrentTeam - 1].lisDwarfs[m_iIDCounterTeam2] != null)
				{
					NextActiveDwarf = refTeam.m_arrTeams[m_iCurrentTeam - 1].lisDwarfs[m_iIDCounterTeam2];
				}
				else 
				{
					Debug.Log("HotSeat: all dwarfs destroyed in team " +  m_iCurrentTeam);
				}
			}
			//NextActiveDwarf = refTeam.GetDwarfCharByIDAndTeamID(m_iIDCounterTeam1, m_iCurrentTeam);
			refMouse2D.ZoomToPosition(NextActiveDwarf.gameObject.transform.position);	
			if(NextActiveDwarf.iTeamID == 1)
			{
				NextActiveDwarf.SetOutlineShader(0.005f, m_col32OutlineNotActive);
				NextActiveDwarf.SetOutlineShader(0.005f, m_col32Team1NextActive);
			}
			else if(NextActiveDwarf.iTeamID == 2)
			{
				NextActiveDwarf.SetOutlineShader(0.005f, m_col32OutlineNotActive);
				NextActiveDwarf.SetOutlineShader(0.005f, m_col32Team2NextActive);
			}
		}

		if(m_iCountdownInSeconds <= 0)
		{
			m_bGetReadyPhase = false;
			CancelInvoke();
			refMouse2D.StopZoom();
			PlayPhase();
		}
		m_iCountdownInSeconds--;
	}

	void GetStartPositionsDwarfFromXMLTeam1()
	{
		m_lisarrPositionsTeam1 = new List<Vector2>();
		for (int x = 0; x < refMap.arriXML_Level.GetLength(0); x++)
		{
			for (int y = 0; y < refMap.arriXML_Level.GetLength(1); y++)
			{
				if(refMap.arriXML_Level[x, y] == 74 || refMap.arriXML_Level[x, y] == 75 || refMap.arriXML_Level[x, y] == 76)
				{
					m_lisarrPositionsTeam1.Add(new Vector2(x,y));
				}
			}
		}
	}

	void GetStartPositionsDwarfFromXMLTeam2()
	{
		m_lisarrPositionsTeam2 = new List<Vector2>();
		for (int x = 0; x < refMap.arriXML_Level.GetLength(0); x++)
		{
			for (int y = 0; y < refMap.arriXML_Level.GetLength(1); y++)
			{
				if(refMap.arriXML_Level[x, y] == 80 || refMap.arriXML_Level[x, y] == 81 || refMap.arriXML_Level[x, y] == 82)
				{
					m_lisarrPositionsTeam2.Add(new Vector2(x,y));
				}
			}
		}
	}

	public string GetLastInvoke()
	{
		return m_strLastInvoke;
	}

	public DwarfChar GetActiveDwarf()
	{
		if(ActiveDwarf != null)
		{
			return ActiveDwarf;
		}
		else
		{
			return null;
		}
	}
}
