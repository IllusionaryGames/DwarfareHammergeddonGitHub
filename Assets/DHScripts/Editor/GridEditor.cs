using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

[CustomEditor (typeof(Grid))]
public class GridEditor : Editor
{
	Grid m_Grid;

	private FillingWindow window = null;

	private Vector2 m_vec2MousePos;

	private Camera m_camCam;

	private LevelXMLSaver m_lvlXmlSaver;
	private ELoadTexture m_ELoadTexture;
	private PrefabReferencer m_prefabReferencer;

	private DHWorld m_DHWorld;

	public void OnEnable()
	{
		m_Grid = (Grid)target;
		SceneView.onSceneGUIDelegate = GridUpdate;
		m_lvlXmlSaver = new LevelXMLSaver();
		m_prefabReferencer = new PrefabReferencer();

		m_prefabReferencer = PrefabReferencer.Load(Path.Combine (Application.persistentDataPath, "KeyInfo.xml"));
		m_prefabReferencer.Extract();

		m_ELoadTexture = new ELoadTexture();

		m_DHWorld = m_Grid.gameObject.GetComponent<DHWorld>();

		m_Grid.m_arrIndexProfiles = new IndexProfile[m_DHWorld.m_arriWorld.GetLength(0), m_DHWorld.m_arriWorld.GetLength(1)];
		
		RebuildMap();
	}
	void GridUpdate(SceneView sceneview)
	{
		Event e = Event.current;
		
		m_vec2MousePos = e.mousePosition;
		
		m_camCam = Camera.current;
		
		Ray r = m_camCam.ScreenPointToRay(new Vector3(m_vec2MousePos.x, -m_vec2MousePos.y + m_camCam.pixelHeight));
		// calculate index in the grid from current mouse position
		int iIndexX = Mathf.FloorToInt((r.origin.x + m_Grid.boundsX * m_Grid.width) / m_Grid.width);
		int iIndexY = Mathf.FloorToInt((r.origin.y + m_Grid.boundsY * m_Grid.height) / m_Grid.height);
		
		if (e.type == EventType.KeyDown)
		{
			if (e.keyCode == KeyCode.F1)
			{
				Debug.Log ("saving");
				Debug.Log ("now converting");
				m_lvlXmlSaver.Compress(m_DHWorld.m_arriOrigins);
				Debug.Log ("finished converting, now saving");
				m_lvlXmlSaver.Save(Path.Combine(Application.dataPath, "DHLevelDesign/XmlMaps/"+m_Grid.m_strLevelName));
				Debug.Log ("Saved at " + Path.Combine(Application.dataPath, "DHLevelDesign/XmlMaps/"+m_Grid.m_strLevelName));
				Debug.Log ("Successfully saved!");
			}
			else if (e.keyCode == KeyCode.F2)
			{
				Debug.Log ("loading");
				m_lvlXmlSaver = LevelXMLSaver.Load(Path.Combine(Application.dataPath, "DHLevelDesign/XmlMaps/"+m_Grid.m_strLevelName));
				Debug.Log ("finished loading, now extracting");
				m_DHWorld.m_arriWorld.Initialize();
				m_DHWorld.m_arriOrigins.Initialize();
				int[,] arriTemp = m_lvlXmlSaver.Extract();
				for (int x = 0; x < arriTemp.GetLength(0); ++x)
				{
					for (int y = 0; y < arriTemp.GetLength(1); ++y)
					{
						if (x < m_DHWorld.m_arriWorld.GetLength(0) && y < m_DHWorld.m_arriWorld.GetLength(1))
						{
							if (arriTemp[x, y] > 0)
							{
								PrefabProfile profile = m_prefabReferencer.GetProfileByValue(arriTemp[x, y]);
								m_DHWorld.m_arriOrigins[x, y] = arriTemp[x, y];
								m_DHWorld.m_arriWorld[x, y] = arriTemp[x, y];
								for (int v = 0; v < profile.m_iSizeX; ++v)
								{
									for (int w = 0; w < profile.m_iSizeY; ++w)
									{
										if (!(v == 0 && w == 0))
										{
											m_DHWorld.m_arriWorld[x + v, y + w] = arriTemp[x, y];
										}
									}
								}
							}
						}
					}
				}
				Debug.Log ("Successfully loaded and extracted");
				RebuildMap();
				Debug.Log ("all rebuild");
			}
			else if (e.keyCode == KeyCode.F3)
			{
				Debug.Log ("Loading Texture");
				m_DHWorld.m_arriWorld = m_ELoadTexture.LoadLevelFromTexture(m_Grid.m_strTextureLevel, m_prefabReferencer);
				m_DHWorld.m_arriOrigins = m_DHWorld.m_arriWorld;
				RebuildMap();
				Debug.Log ("Loading complete");
			}
			else if (e.keyCode == KeyCode.LeftControl)
			{
				DeleteCube(iIndexX, iIndexY);
			}
			else if (e.keyCode == KeyCode.RightAlt)
			{
				FloodFill (iIndexX, iIndexY, m_Grid.SelectedProfile);
			}
		}
		if (e.isKey)
		{
			if (e.character == ' ')
			{
				if (m_Grid.TileID > 0 && m_Grid.SelectedPrefab)
				{
					PlaceCube((short)m_Grid.TileID, iIndexX, iIndexY, m_Grid.SelectedProfile);
				}
			}
			else
			{
				PrefabProfile profile;
				short iTileIDtemp = m_prefabReferencer.GetProfileByChar(e.character, out profile);
				if (profile.m_goPrefab != null)
				{
					m_Grid.SelectedProfile = profile;
					m_Grid.TileID = iTileIDtemp;
				}
			}
		}
		if (e.type == EventType.MouseDown)
		{
			Debug.Log (iIndexX + ", " + iIndexY);
			if (window != null)
			{
				if (e.button == 0)
				{
					window.m_iPositionX = iIndexX;
					window.m_iPositionY = iIndexY;
				}
				else if (e.button == 1)
				{
					window.m_iExtendX = iIndexX;
					window.m_iExtendY = iIndexY;
				}
			}
		}
	}

	/*
	 * // deprecated: array needs to be updated to origins array
	public void SafeLevelToEditorPrefs()
	{
		int[] arriTemp = new int[m_DHWorld.m_arriWorld.GetLength(0) * m_DHWorld.m_arriWorld.GetLength(1)];
		
		for (int i = 0; i < m_DHWorld.m_arriWorld.GetLength(0); ++i)
		{
			for (int j = 0; j < m_DHWorld.m_arriWorld.GetLength(1); ++j)
			{
				arriTemp[j * m_DHWorld.m_arriWorld.GetLength(0) + i] = m_DHWorld.m_arriWorld[i, j];
			}
		}
		
		EditorPrefsX.SetIntArray("TempLevel", arriTemp);
	}
	*/
	
	// Place single cube with the tile-ID, the index in the array and the specific gameobject
	public void PlaceCube(short sID, int iIndexX, int iIndexY, PrefabProfile profile)
	{
		// check if position is within the map array
		if (iIndexX >= 0 && (iIndexX + profile.m_iSizeX) <= 2 * m_Grid.boundsX &&
		    iIndexY >= 0 && (iIndexY + profile.m_iSizeY) <= 2 * m_Grid.boundsY)
		{
			// check if the passed prefabprofile is valid
			if (profile.m_goPrefab != null)
			{
				// check if target area is free
				bool bSpaceIsFree = true;
				bool bBreakLoop = false;
				for (int x = 0; x < profile.m_iSizeX; ++x)
				{
					for (int y = 0; y < profile.m_iSizeY; ++y)
					{
						if (m_DHWorld.m_arriWorld[iIndexX + x, iIndexY + y] > 0)
						{
							bSpaceIsFree = false;
							bBreakLoop = true;
							break;
						}
					}
					if (bBreakLoop)
						break;
				}

				if (bSpaceIsFree)
				{
					// instantiate gameobejct at target position
					GameObject goObj = (GameObject)PrefabUtility.InstantiatePrefab(profile.m_goPrefab);
					// create profile for that field
					IndexProfile temp;
					temp.bValid = true;
					temp.goObject = goObj;
					temp.iXorigin = iIndexX;
					temp.iYorigin = iIndexY;
					temp.goObject.transform.position = new Vector3(
												(iIndexX - m_Grid.boundsX) * m_Grid.width + m_Grid.width / 2.0f,
					                            (iIndexY - m_Grid.boundsY) * m_Grid.height + m_Grid.height / 2.0f, 0.0f);

					m_Grid.m_arrIndexProfiles[iIndexX, iIndexY] = temp;
					m_DHWorld.m_arriOrigins[iIndexX, iIndexY] = sID;
					m_DHWorld.m_arriWorld[iIndexX, iIndexY] = sID;
					for (int x = 0; x < profile.m_iSizeX; ++x)
					{
						for (int y = 0; y < profile.m_iSizeY; ++y)
						{
							if (!(x == 0 && y == 0))
							{
								temp.bValid = true;
								temp.goObject = null;
								temp.iXorigin = iIndexX;
								temp.iYorigin = iIndexY;
								m_Grid.m_arrIndexProfiles[iIndexX + x, iIndexY + y] = temp;
								m_DHWorld.m_arriWorld[iIndexX + x, iIndexY + y] = sID;
							}
						}
					}
				}
			}
		}
	}
	// Place single cube with the tile-ID, the index in the array and the specific gameobject without checking if there is already a cube
	public void PlaceCubeDirty(int iIndexX, int iIndexY, PrefabProfile profile)
	{
		if (iIndexX >= 0 && (iIndexX + profile.m_iSizeX) <= 2 * m_Grid.boundsX &&
		    iIndexY >= 0 && (iIndexY + profile.m_iSizeY) <= 2 * m_Grid.boundsY)
		{
			if (profile.m_goPrefab != null)
			{
				GameObject goObj = (GameObject)PrefabUtility.InstantiatePrefab(profile.m_goPrefab);
				goObj.transform.position = new Vector3((iIndexX - m_Grid.boundsX) * m_Grid.width + m_Grid.width / 2.0f,
				                                       (iIndexY - m_Grid.boundsY) * m_Grid.height + m_Grid.height / 2.0f, 0.0f);
				IndexProfile temp;
				temp.bValid = true;
				temp.goObject = goObj;
				temp.iXorigin = iIndexX;
				temp.iYorigin = iIndexY;
				temp.goObject.transform.position = new Vector3(
					(iIndexX - m_Grid.boundsX) * m_Grid.width + m_Grid.width / 2.0f,
					(iIndexY - m_Grid.boundsY) * m_Grid.height + m_Grid.height / 2.0f, 0.0f);
				
				m_Grid.m_arrIndexProfiles[iIndexX, iIndexY] = temp;

				for (int x = 0; x < profile.m_iSizeX; ++x)
				{
					for (int y = 0; y < profile.m_iSizeY; ++y)
					{
						if (!(x == 0 && y == 0))
						{
							temp.bValid = true;
							temp.goObject = null;
							temp.iXorigin = iIndexX;
							temp.iYorigin = iIndexY;
							m_Grid.m_arrIndexProfiles[iIndexX + x, iIndexY + y] = temp;
						}
					}
				}
			}
		}
	}
	// Fill Area with tile
	public void FillArea(int iStartX, int iStartY, int iEndX, int iEndY, short sTileID, PrefabProfile profile)
	{
		int iStartValX = Mathf.Min (iStartX, iEndX);
		int iEndValX = Mathf.Max (iStartX, iEndX);
		
		int iStartValY = Mathf.Min (iStartY, iEndY);
		int iEndValY = Mathf.Max (iStartY, iEndY);

		if (profile.m_iSizeX > 0 && profile.m_iSizeY > 0)
		{
			for (int x = iStartValX; x <= iEndValX; x += profile.m_iSizeX)
			{
				for (int y = iStartValY; y <= iEndValY; y += profile.m_iSizeY)
				{
					PlaceCube(sTileID, x, y, profile);
				}
			}
		}
	}
	// Clear the grid in a certain rectangle
	public void ClearArea(int iStartX, int iStartY, int iEndX, int iEndY)
	{
		int iStartValX = Mathf.Min (iStartX, iEndX);
		int iEndValX = Mathf.Max (iStartX, iEndX);
		
		int iStartValY = Mathf.Min (iStartY, iEndY);
		int iEndValY = Mathf.Max (iStartY, iEndY);
		
		for (int x = iStartValX; x <= iEndValX; ++x)
		{
			for (int y = iStartValY; y <= iEndValY; ++y)
			{
				DeleteCube (x, y);
			}
		}
	}
	// Delete cube from the grid on a certain index
	public void DeleteCube(int iIndexX, int iIndexY)
	{
		/* calculate origin here */
		if (iIndexX >= 0 && iIndexX < 2 * m_Grid.boundsX && iIndexY >= 0 && iIndexY < 2 * m_Grid.boundsY)
		{
			if (m_DHWorld.m_arriWorld[iIndexX, iIndexY] > 0)
			{
				IndexProfile tempProf = m_Grid.m_arrIndexProfiles[iIndexX, iIndexY];
				int iOriginX = tempProf.iXorigin;
				int iOriginY = tempProf.iYorigin;
				GameObject goObj = CubeAtIndex(iIndexX, iIndexY);
				if (goObj != null)
				{
					// check if gameobject is allowed to be deleted
					int index = m_prefabReferencer.ContainsGameobject(goObj);

					if (index >= 0)
					{
						PrefabProfile profile;
						profile = m_prefabReferencer.GetProfileByValue(index);
						m_DHWorld.m_arriOrigins[iOriginX, iOriginY] = 0;
						for (int x = 0; x < profile.m_iSizeX; ++x)
						{
							for (int y = 0; y < profile.m_iSizeY; ++y)
							{
								m_DHWorld.m_arriWorld[iOriginX + x, iOriginY + y] = 0;
								m_Grid.m_arrIndexProfiles[iOriginX + x, iOriginY + y].bValid = false;
							}
						}

						DestroyImmediate(goObj);
					}
				}
			}
		}
	}

	public GameObject CubeAtIndex(int iIndexX, int iIndexY)
	{
		IndexProfile prof;
		prof = m_Grid.m_arrIndexProfiles[iIndexX, iIndexY];
		//if (prof.bValid)
			return m_Grid.m_arrIndexProfiles[prof.iXorigin, prof.iYorigin].goObject;
		//else
			//return null;
	}
	
	// delete every gameobject which is allowed to be deleted
	public void ClearAll()
	{
		GameObject[] goObjects = FindObjectsOfType(typeof(GameObject)) as GameObject[];
		
		foreach (GameObject obj in goObjects)
		{
			if (m_prefabReferencer.ContainsGameobject(obj) > -1)
			{
				DestroyImmediate(obj);
			}
		}
	}
	
	public void FloodFill(int iIndexX, int iIndexY, PrefabProfile profile)
	{
		if (profile.m_iSizeX == 1 && profile.m_iSizeY == 1)
		{
			if (iIndexX >= 0 && iIndexX < 2 * m_Grid.boundsX && iIndexY >= 0 && iIndexY < 2 * m_Grid.boundsY)
			{
				if (m_DHWorld.m_arriWorld[iIndexX, iIndexY] == 0)
				{
					PlaceCube (m_Grid.TileID, iIndexX, iIndexY, profile);
					// recursive call on the neighbours
					FloodFill (iIndexX + 1, iIndexY, profile);
					FloodFill (iIndexX - 1, iIndexY, profile);
					FloodFill (iIndexX, iIndexY + 1, profile);
					FloodFill (iIndexX, iIndexY - 1, profile);
				}
			}
		}
	}
	
	// rebuild the geometry of the grid according to the grid array
	public void RebuildMap()
	{
		// first clear all geometry
		ClearAll();
		
		// loop through the grid array
		for (int x = 0; x < m_DHWorld.m_arriWorld.GetLength(0); ++x)
		{
			for (int y = 0; y < m_DHWorld.m_arriWorld.GetLength(1); ++y)
			{	
				if (m_DHWorld.m_arriOrigins[x, y] > 0)
				{
					PlaceCubeDirty(x, y,m_prefabReferencer.GetProfileByValue((short)m_DHWorld.m_arriOrigins[x, y]));
				}
			}
		}
	}


	public override void OnInspectorGUI()
	{
		GUILayout.BeginHorizontal();
		GUILayout.Label (" Grid Width ");
		m_Grid.width = Mathf.Max (EditorGUILayout.FloatField(m_Grid.width, GUILayout.Width (50)), 1);
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.Label (" Grid Height ");
		m_Grid.height = Mathf.Max (EditorGUILayout.FloatField(m_Grid.height, GUILayout.Width (50)), 1);
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.Label (" Levelname ");
		m_Grid.m_strLevelName = EditorGUILayout.TextField(m_Grid.m_strLevelName, GUILayout.Width(255));
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.Label (" Texturename ");
		m_Grid.m_strTextureLevel = EditorGUILayout.TextField(m_Grid.m_strTextureLevel, GUILayout.Width(255));
		GUILayout.EndHorizontal();

		if (GUILayout.Button("Fill Area", GUILayout.Width(255)))
		{
			window = (FillingWindow)EditorWindow.GetWindow(typeof(FillingWindow));
			window.editor = this;
			window.m_byActionID = 1;
			window.profile = m_Grid.SelectedProfile;
			window.m_sTileID = m_Grid.TileID;
		}

		if (GUILayout.Button("Clear Area", GUILayout.Width(255)))
		{
			window = (FillingWindow)EditorWindow.GetWindow(typeof(FillingWindow));
			window.editor = this;
			window.m_byActionID = 2;
		}

		if (GUILayout.Button("Fill All", GUILayout.Width(255)))
		{
			//Debug.Log (goPref);

			for (int x = 0; x < m_DHWorld.m_arriWorld.GetLength(0); ++x)
			{
				for (int y = 0; y < m_DHWorld.m_arriWorld.GetLength(1); ++y)
				{
					PlaceCube((short)m_Grid.TileID, x, y, m_Grid.SelectedProfile);
				}
			}
		}

		if (GUILayout.Button("Clear All", GUILayout.Width(255)))
		{
			for (int i = 0; i < m_DHWorld.m_arriWorld.GetLength(0); ++i)
			{
				for (int j = 0; j < m_DHWorld.m_arriWorld.GetLength(1); ++j)
				{
					m_DHWorld.m_arriWorld[i, j] = 0;
					m_DHWorld.m_arriOrigins[i, j] = 0;
					m_Grid.m_arrIndexProfiles[i, j].bValid = false;
				}
			}
			ClearAll();
		}


		if (GUILayout.Button("Rebuild Map", GUILayout.Width(255)))
		{
			RebuildMap();
		}

		SceneView.RepaintAll();
	}
}
