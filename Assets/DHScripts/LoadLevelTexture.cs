using UnityEngine;
using System.Collections;

public class LoadLevelTexture : MonoBehaviour 
{
	[System.Serializable]
	public class BlockType
	{
		public string m_strBlockName;

		public Color m_colBlockColor;
		public GameObject m_preBlock;
		public bool m_bIsPlayer;
		public bool m_bPassable;
		public bool m_bIsSolid;
		public bool m_bPlaceable;
		public bool m_bDestroyable;
		public bool m_bIsClimbable;
		public bool m_bIsRecoverable;
		public int m_iHardness;
		public int m_iTypeID; 
		public int iTypeID
		{
			get {return m_iTypeID;} 
		}
		public AudioClip m_SoundEffect;
	}
	
	[System.Serializable]
	public class Level
	{
		public Texture2D m_tex2DLevel;
		public Color[,] m_arrcolLevel;
		public BlockType[,] m_BlockType;
	}

	public BlockType[] BlockTypes;
	public Level[] Levels;

	// Mauli: delete if not necessary
	// Use this for initialization
	void Start () 
	{
	}
	
	// Update is called once per frame
	void Update () 
	{
	}

	public void loadLevelsFromTexture()
	{
		// Debug.Log(" LoadLevelTexture: Levels.Length " + Levels.Length );

		for (int i = 0; i < Levels.Length; i++) 
		{
			// Debug.Log(" LoadLevelTexture: " + i );
			int LevelWidth = Levels[i].m_tex2DLevel.width;
			int LevelHeight = Levels[i].m_tex2DLevel.height;
			Levels[i].m_arrcolLevel = new Color[LevelWidth, LevelHeight];
			Levels[i].m_BlockType = new BlockType[LevelWidth, LevelHeight];

			int iPixelCounterLevel = 0;
			Color[] pix = Levels[i].m_tex2DLevel.GetPixels (0, 0, LevelWidth , LevelHeight);
			for (int m = 0; m < LevelWidth; m++) 
			{
				for (int n = 0; n < LevelHeight; n++) 
				{
					Levels[i].m_arrcolLevel [m, n] = pix [iPixelCounterLevel];
					iPixelCounterLevel++;

					for(int k = 0; k < BlockTypes.Length; k++)
					{
						if(Levels[i].m_arrcolLevel[m, n] == BlockTypes[k].m_colBlockColor)
						{
							// save Blocktype in Levelposition
							Levels[i].m_BlockType[n , m] = BlockTypes[k];
						}
					}
				}
			}
			// set Pixelcounter to zero for next loop
			iPixelCounterLevel = 0;
		}
	}

	public BlockType GetBlockTypeAtPositionFromTexture(int _iLevel, int _iXGrid, int _iYGrid)
	{
		BlockType tempBlockType = new BlockType();
		// read Blocktype in Levelposition and mapArray
		tempBlockType = Levels[_iLevel].m_BlockType[_iXGrid, _iYGrid];
		return tempBlockType;
	}

	public int GetBlockIDAtPositionFromTexture(int _iLevel, int _iXGrid, int _iYGrid)
	{
		// Debug.Log("LoadLevelTexture: _iLevel " + _iLevel);
		//Debug.Log("LoadLevelTexture: _iXGrid " + _iXGrid);
		//Debug.Log("LoadLevelTexture: _iYGrid " + _iYGrid);
		if(_iXGrid < Levels[_iLevel].m_BlockType.GetLength(0) && _iYGrid < Levels[_iLevel].m_BlockType.GetLength(1) &&
		   _iXGrid >= 0 && _iYGrid >= 0)
		{
			if(Levels[_iLevel].m_BlockType[_iXGrid, _iYGrid] != null)
			{
				return Levels[_iLevel].m_BlockType[_iXGrid, _iYGrid].iTypeID;
			}
		}
		return -1; // CHECK
	}

	// Mauli: Provide methods for return only the needed information
	//		  instead of returning a whole blocktype, for performance
	//		  reasons.

	public GameObject GetBlockPrefabByTypeID(int iTypeID)
	{
		for (int i = 0; i < BlockTypes.GetLength(0); ++i)
		{
			if (BlockTypes[i].iTypeID == iTypeID)
			{
				return BlockTypes[i].m_preBlock;
			}
		}

		return null;
	}

	public BlockType GetBlockTypeByTypeID(int iTypeID)
	{
		for (int i = 0; i < BlockTypes.GetLength(0); ++i)
		{
			if (BlockTypes[i].iTypeID == iTypeID)
			{
				return BlockTypes[i];
			}
		}
		
		return null;
	}
}
