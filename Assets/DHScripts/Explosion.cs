using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Explosion : MonoBehaviour {

	public m_struExplosionProfile[] ExplosionProfile;
	public int m_ExplosionArraySize = 32;
	public int m_iExplosionDamageFactorToBase; // standard 12

	// references 
	private Team refTeam;
	private Map refMap;
	private LoadLevelTexture refLoadLevelTexture;

	// for ExplosionTextureReading
	public string strFirstExplosionName;
	public int m_iFirstExplosionHardness;
	public float m_fFirstExplosionDamage;
	public string strSecondExplosionName;
	public int m_iSecondExplosionHardness;
	public float m_fSecondExplosionDamage;
	public int iExplosionAmountWithoutUpgrade = 2;
	public Texture2D tex2DFirstExplosion0;
	public Texture2D tex2DFirstExplosion1;
	public Texture2D tex2DFirstExplosion2;
	public Texture2D tex2DFirstExplosion3;
	public Texture2D tex2DSecondExplosion0;
	public Texture2D tex2DSecondExplosion1;
	public Texture2D tex2DSecondExplosion2;
	public Texture2D tex2DSecondExplosion3;
	public int m_iExplosionTextureAmount = 8;
	public Color[,] ColorArray;
	private int iPixelCounter = 0; // standard 0
	
	public struct m_struExplosionProfile
	{
		public string strExplosionName;
		public int iUpgradeStage;
		public bool[,] arrbExplosionbehaviour;
		public int iExplosionHardness;
		public float fFirstExplosionDamage;
		public float fSecondExplosionDamage;
	}

	void Awake()
	{
		refTeam = GetComponent<Team>();
		refMap = GetComponent<Map>();
		refLoadLevelTexture = GetComponent<LoadLevelTexture>();
	}

	// Use this for initialization
	void Start () 
	{
		if (m_iExplosionTextureAmount == 0)
		{
			Debug.Log ("Map: correct m_iExplosionTextureAmount"); 
		}
		ExplosionProfile = new m_struExplosionProfile[m_iExplosionTextureAmount];
		initializeExplosionProfiles ();	
	}

	public void detonate (Vector2 _vec2DetonationMiddle, string _ExplosionName, int _UpgradeStage)
	{
		// only detonate, if _vec2DetonationMiddle is inside LevelSize
		if(_vec2DetonationMiddle.x > 0 && _vec2DetonationMiddle.y > 0 &&
		   _vec2DetonationMiddle.x < refLoadLevelTexture.Levels[refMap.m_iCurrentLevel].m_tex2DLevel.width &&
		   _vec2DetonationMiddle.y < refLoadLevelTexture.Levels[refMap.m_iCurrentLevel].m_tex2DLevel.height)
		{
			// go through all ExplosionProfiles to find the right Explosion
			for (int b = 0; b < m_iExplosionTextureAmount; b++)
			{
				if(_ExplosionName == ExplosionProfile[b].strExplosionName)
				{
					if(_UpgradeStage == ExplosionProfile[b].iUpgradeStage)
					{
						// go through Map
						for (int x = 0; x < refLoadLevelTexture.Levels[refMap.m_iCurrentLevel].m_tex2DLevel.width; x++) 
						{
							for (int y = 0; y < refLoadLevelTexture.Levels[refMap.m_iCurrentLevel].m_tex2DLevel.height; y++) 
							{												
								// Find Detonation Middle
								if(refMap.m_arrMap[x, y].iXIndex == _vec2DetonationMiddle.x && refMap.m_arrMap[x, y].iYIndex == _vec2DetonationMiddle.y)
								{
									int iMiddleX = (int)_vec2DetonationMiddle.x;
									int iMiddleY = (int)_vec2DetonationMiddle.y;
									
									List<Vector2> lisDetonationPositions = new List<Vector2>();	
									List<Vector2> lisCheckedPointsToAllDirections = new List<Vector2>();
									int lisDetonationPositionsCounter = 0;
									
									// Startpoint for explosion
									lisDetonationPositions.Add (new Vector2(_vec2DetonationMiddle.x, _vec2DetonationMiddle.y));
									
									for(int d = 0; d < lisDetonationPositions.Count; d++)
									{
										if(lisDetonationPositionsCounter < 200) // change CounterEnd for better Performance, but smaller explosions
										{
											int iDetonationX = (int)lisDetonationPositions[d].x;
											int iDetonationY = (int)lisDetonationPositions[d].y;

											Vector2 vec2UpToExplosion = new Vector2(iDetonationX,  iDetonationY + 1);
											Vector2 vec2DownToExplosion = new Vector2(iDetonationX,  iDetonationY - 1);
											Vector2 vec2LeftToExplosion = new Vector2(iDetonationX - 1,  iDetonationY);
											Vector2 vec2RightToExplosion = new Vector2(iDetonationX + 1,  iDetonationY);

											int iXUp    = iDetonationX;
											int iXDown  = iDetonationX;
											int iXLeft  = iDetonationX - 1;
											int iXRight = iDetonationX + 1;
											int iYLeft  = iDetonationY;
											int iYRight = iDetonationY;
											int iYDown  = iDetonationY - 1;
											int iYUp    = iDetonationY + 1;

											int iDiffX  = iDetonationX - iMiddleX;
											int iDiffY  = iDetonationY - iMiddleY;
											
											//CHECK UP
											if(ExplosionProfile[b].arrbExplosionbehaviour[ 16 + iDiffX, 16 + iDiffY + 1]) // should block explode because of explosiontexture
											{
												if(refMap.m_arrMap.GetLength(1) > iDetonationY + 1)
												{
													LoadLevelTexture.BlockType CurrentBlock = refMap.GetBlockTypeByID(refMap.m_arrMap[iXUp, iYUp].iBlockID);

													// is it Base
													if(refMap.IsBaseTeam1AtPosition(iXUp, iYUp))
													{
														// one Damage is 0
														refTeam.DamageBase(-ExplosionProfile[b].fFirstExplosionDamage * m_iExplosionDamageFactorToBase, 1);
														refTeam.DamageBase(-ExplosionProfile[b].fSecondExplosionDamage * m_iExplosionDamageFactorToBase, 1);													
														Debug.Log("Explotion: team1baseHP " + refTeam.m_arrTeams[0].fHitPointsBase);														
													}
													else if(refMap.IsBaseTeam2AtPosition(iXUp, iYUp))
													{
														// one Damage is 0
														refTeam.DamageBase(-ExplosionProfile[b].fFirstExplosionDamage * m_iExplosionDamageFactorToBase, 2);
														refTeam.DamageBase(-ExplosionProfile[b].fSecondExplosionDamage * m_iExplosionDamageFactorToBase, 2);
														Debug.Log("Explotion: team2baseHP " + refTeam.m_arrTeams[1].fHitPointsBase);
													}
													else // no base so bigger Explosion
													{
														if(CurrentBlock != null) // check block up
														{
															if(CurrentBlock.m_bDestroyable) // is destroyable
															{
																if(CurrentBlock.m_iHardness <= ExplosionProfile[b].iExplosionHardness)
																{
																	DestroyBlockAndUpdateMap(iXUp, iYUp);  // destroyed Prefab up

																	// if new ExplosionPoint is not in List
																	if(!lisDetonationPositions.Contains(vec2UpToExplosion))
																	{
																		lisDetonationPositions.Add(vec2UpToExplosion);
																	}
																	lisDetonationPositionsCounter++;;
																	
																}
															}
														}
													}
												}
											}
											
											//CHECK Left
											if(ExplosionProfile[b].arrbExplosionbehaviour[16 + iDiffX - 1, 16 + iDiffY]) // should block explode because of explosiontexture
											{
												LoadLevelTexture.BlockType CurrentBlock = refMap.GetBlockTypeByID(refMap.m_arrMap[iXLeft, iYLeft].iBlockID);

												// is it Base
												if(refMap.IsBaseTeam1AtPosition(iXLeft, iYLeft))
												{
													// one Damage is 0
													refTeam.DamageBase(-ExplosionProfile[b].fFirstExplosionDamage * m_iExplosionDamageFactorToBase, 1);
													refTeam.DamageBase(-ExplosionProfile[b].fSecondExplosionDamage * m_iExplosionDamageFactorToBase, 1);
													Debug.Log("Explotion: team1baseHP " + refTeam.m_arrTeams[0].fHitPointsBase);
													
												}
												else if(refMap.IsBaseTeam2AtPosition(iXLeft, iYLeft))
												{
													// one Damage is 0
													refTeam.DamageBase(-ExplosionProfile[b].fFirstExplosionDamage * m_iExplosionDamageFactorToBase, 2);
													refTeam.DamageBase(-ExplosionProfile[b].fSecondExplosionDamage * m_iExplosionDamageFactorToBase, 2);
													Debug.Log("Explotion: team2baseHP " + refTeam.m_arrTeams[1].fHitPointsBase);
												}
												else
												{							
													if(CurrentBlock != null) // check block left
													{
														if(CurrentBlock.m_bDestroyable) // is destroyable
														{
															if(CurrentBlock.m_iHardness <= ExplosionProfile[b].iExplosionHardness)
															{
																DestroyBlockAndUpdateMap(iXLeft, iYLeft);  // destroyed Prefab left

																// if new ExplosionPoint is not in List
																if(!lisDetonationPositions.Contains(vec2LeftToExplosion))
																{
																	lisDetonationPositions.Add(vec2LeftToExplosion);
																}
																lisDetonationPositionsCounter++;
															}
														}
													}
												}
											}
											
											//CHECK Right
											if(ExplosionProfile[b].arrbExplosionbehaviour[16 + iDiffX + 1, 16 + iDiffY]) // should block explode because of explosiontexture
											{
												LoadLevelTexture.BlockType CurrentBlock = refMap.GetBlockTypeByID(refMap.m_arrMap[iXRight, iYRight].iBlockID);
												// is it Base
												if(refMap.IsBaseTeam1AtPosition(iXRight, iYRight))
												{
													// one Damage is 0
													refTeam.DamageBase(-ExplosionProfile[b].fFirstExplosionDamage * m_iExplosionDamageFactorToBase, 1);
													refTeam.DamageBase(-ExplosionProfile[b].fSecondExplosionDamage * m_iExplosionDamageFactorToBase, 1);
													Debug.Log("Explotion: team1baseHP " + refTeam.m_arrTeams[0].fHitPointsBase);
													
												}
												else if(refMap.IsBaseTeam2AtPosition(iXRight, iYRight))
												{
													// one Damage is 0
													refTeam.DamageBase(-ExplosionProfile[b].fFirstExplosionDamage * m_iExplosionDamageFactorToBase, 2);
													refTeam.DamageBase(-ExplosionProfile[b].fSecondExplosionDamage * m_iExplosionDamageFactorToBase, 2);
													Debug.Log("Explotion: team2baseHP " + refTeam.m_arrTeams[1].fHitPointsBase);
												}
												else
												{	
													if(refMap.m_arrMap.GetLength(0) > iDetonationX + 1)
													{
														if(CurrentBlock != null) // check block right
														{
															if(CurrentBlock.m_bDestroyable) // is destroyable
															{
																if(CurrentBlock.m_iHardness <= ExplosionProfile[b].iExplosionHardness)
																{
																	DestroyBlockAndUpdateMap(iXRight, iYRight);

																	// if new ExplosionPoint is not in List
																	if(!lisDetonationPositions.Contains(vec2RightToExplosion))
																	{
																		lisDetonationPositions.Add(vec2RightToExplosion);
																	}
																	lisDetonationPositionsCounter++;
																}
															}
														}
													}
												}
											}
											
											//CHECK Down
											if(ExplosionProfile[b].arrbExplosionbehaviour[16 + iDiffX, 16 + iDiffY - 1]) // should block explode because of explosiontexture
											{
												LoadLevelTexture.BlockType CurrentBlock = refMap.GetBlockTypeByID(refMap.m_arrMap[iXDown, iYDown].iBlockID);

												// is it Base
												if(refMap.IsBaseTeam1AtPosition(iXDown, iYDown))
												{
													// one Damage is 0
													refTeam.DamageBase(-ExplosionProfile[b].fFirstExplosionDamage * m_iExplosionDamageFactorToBase, 1);
													refTeam.DamageBase(-ExplosionProfile[b].fSecondExplosionDamage * m_iExplosionDamageFactorToBase, 1);
													Debug.Log("Explotion: team1baseHP " + refTeam.m_arrTeams[0].fHitPointsBase);
													
												}
												else if(refMap.IsBaseTeam2AtPosition(iXDown, iYDown))
												{
													// one Damage is 0
													refTeam.DamageBase(-ExplosionProfile[b].fFirstExplosionDamage * m_iExplosionDamageFactorToBase, 2);
													refTeam.DamageBase(-ExplosionProfile[b].fSecondExplosionDamage * m_iExplosionDamageFactorToBase, 2);
													Debug.Log("Explotion: team2baseHP " + refTeam.m_arrTeams[1].fHitPointsBase);
												}
												else
												{
													if(CurrentBlock != null) // check block down
													{
														if(CurrentBlock.m_bDestroyable) // is destroyable
														{
															if(CurrentBlock.m_iHardness <= ExplosionProfile[b].iExplosionHardness)
															{
																DestroyBlockAndUpdateMap(iXDown, iYDown);

																// if new ExplosionPoint is not in List
																if(!lisDetonationPositions.Contains(vec2DownToExplosion))
																{
																	lisDetonationPositions.Add(vec2DownToExplosion);
																}	
																lisDetonationPositionsCounter++;
															}
														}
													}
												}
											}
											// Delete Checked Points
											lisDetonationPositions.RemoveAt(d);
											d --;
											Vector2 vec2CheckedToAllDirections = new Vector2(iDetonationX, iDetonationY);
											lisCheckedPointsToAllDirections.Add(vec2CheckedToAllDirections);
										}
										else
										{
											break;
										}
									}
									
									// DEBUG START
									/*
									foreach(Vector2 iter in lisCheckedPointsToAllDirections)
									{
										//Debug.Log("Map: lisCheckedPointsToAllDirections[] = " + iter);
										GameObject goDetonationPoint = Instantiate(m_preExplosionShpere, new Vector3(iter.x * 1.6f, iter.y * 1.6f, 1.6f), Quaternion.identity) as GameObject;
									}
									foreach(Vector2 iter in lisDetonationPositions)
									{
										//Debug.Log("Map: lisDetonationPositions[] = " + iter);
										GameObject goDetonationPoint = Instantiate(m_preRasterSphere, new Vector3(iter.x * 1.6f, iter.y * 1.6f, 1.6f), Quaternion.identity) as GameObject;
										                                     
									}
									*/
									//Debug.Log("Map: lisDetonationPositions.Count = " + lisDetonationPositions.Count);
									//Debug.Log("Map: lisDetonationPositionsCounter at End =" + lisDetonationPositionsCounter);
									
									// DEBUG END
									
								}
							}
						}
					}
				}
			}
		}
	}

	void DestroyBlockAndUpdateMap(int _iXInGrid, int _iYInGrid)
	{
		Destroy(refMap.m_arrMap[_iXInGrid, _iYInGrid].prefab1); // destroyed Prefab down
		refMap.m_arrMap[_iXInGrid, _iYInGrid].iBlockID = 0; // 0 = no Block
		refMap.arriXML_Level[_iXInGrid, _iYInGrid] = 0;
	}

	void initializeExplosionProfiles ()
	{
		for (int i = 0; i < m_iExplosionTextureAmount; i++) 
		{
			ExplosionProfile [i] = new m_struExplosionProfile ();
			ExplosionProfile [i].arrbExplosionbehaviour = new bool[m_ExplosionArraySize, m_ExplosionArraySize];
			ExplosionProfile [i].iExplosionHardness = 0 ;

			
			// set all bools to false for no block destruction
			for (int y = 0; y < m_ExplosionArraySize; y++) 
			{
				for (int x = 0; x < m_ExplosionArraySize; x++) 
				{
					ExplosionProfile [i].arrbExplosionbehaviour [x, y] = false; 
				}
			}
			
			// allocate explosionname
			if(i >= 0 && i  < 4)
			{
				ExplosionProfile [i].strExplosionName = strFirstExplosionName;
				ExplosionProfile [i].iUpgradeStage = i;
				ExplosionProfile [i].iExplosionHardness = m_iFirstExplosionHardness;
				ExplosionProfile [i].fFirstExplosionDamage = m_fFirstExplosionDamage;;
				ExplosionProfile [i].fSecondExplosionDamage = 0;
			}
			else if(i >= 4 && i <= 8)
			{
				ExplosionProfile [i].strExplosionName = strSecondExplosionName;
				ExplosionProfile [i].iUpgradeStage = i - m_iExplosionTextureAmount/2; // - 4
				ExplosionProfile [i].iExplosionHardness = m_iSecondExplosionHardness;
				ExplosionProfile [i].fFirstExplosionDamage = 0;
				ExplosionProfile [i].fSecondExplosionDamage = m_fSecondExplosionDamage;
			}
			
			int p = Mathf.FloorToInt (0);
			int q = Mathf.FloorToInt (0);
			int width = Mathf.FloorToInt (m_ExplosionArraySize);
			int height = Mathf.FloorToInt (m_ExplosionArraySize);
			ColorArray = new Color[width, height];
			switch (i)
			{
			case 0:
				Color[] pix = tex2DFirstExplosion0.GetPixels (p, q, width, height);
				for (int m = 0; m < width; m++) {
					for (int n = 0; n < height; n++) {
						ColorArray [m, n] = pix [iPixelCounter];
						iPixelCounter++;
						if (ColorArray [m, n].a == 1) {
							ExplosionProfile [i].arrbExplosionbehaviour [m, n] = true;
						}
					}
				}
				break;
			case 1:
				Color[] pix1 = tex2DFirstExplosion1.GetPixels (p, q, width, height);
				for (int m = 0; m < width; m++) {
					for (int n = 0; n < height; n++) {
						ColorArray [m, n] = pix1 [iPixelCounter];
						iPixelCounter++;
						if (ColorArray [m, n].a == 1) {
							ExplosionProfile [i].arrbExplosionbehaviour [m, n] = true;
						}
					}
				}
				break;
			case 2:
				Color[] pix2 = tex2DFirstExplosion2.GetPixels (p, q, width, height);
				for (int m = 0; m < width; m++) {
					for (int n = 0; n < height; n++) {
						ColorArray [m, n] = pix2 [iPixelCounter];
						iPixelCounter++;
						if (ColorArray [m, n].a == 1) {
							ExplosionProfile [i].arrbExplosionbehaviour [m, n] = true;
						}
					}
				}
				break;
			case 3:
				Color[] pix3 = tex2DFirstExplosion3.GetPixels (p, q, width, height);
				for (int m = 0; m < width; m++) {
					for (int n = 0; n < height; n++) {
						ColorArray [m, n] = pix3 [iPixelCounter];
						iPixelCounter++;
						if (ColorArray [m, n].a == 1) {
							ExplosionProfile [i].arrbExplosionbehaviour [m, n] = true;
						}
					}
				}
				break;
			case 4:
				Color[] pix4 = tex2DSecondExplosion0.GetPixels (p, q, width, height);
				for (int m = 0; m < width; m++) {
					for (int n = 0; n < height; n++) {
						ColorArray [m, n] = pix4 [iPixelCounter];
						iPixelCounter++;
						if (ColorArray [m, n].a == 1) {
							ExplosionProfile [i].arrbExplosionbehaviour [m, n] = true;
						}
					}
				}
				break;
			case 5:
				Color[] pix5 = tex2DSecondExplosion1.GetPixels (p, q, width, height);
				for (int m = 0; m < width; m++) {
					for (int n = 0; n < height; n++) {
						ColorArray [m, n] = pix5 [iPixelCounter];
						iPixelCounter++;
						if (ColorArray [m, n].a == 1) {
							ExplosionProfile [i].arrbExplosionbehaviour [m, n] = true;
						}
					}
				}
				break;
			case 6:
				Color[] pix6 = tex2DSecondExplosion2.GetPixels (p, q, width, height);
				for (int m = 0; m < width; m++) {
					for (int n = 0; n < height; n++) {
						ColorArray [m, n] = pix6 [iPixelCounter];
						iPixelCounter++;
						if (ColorArray [m, n].a == 1) {
							ExplosionProfile [i].arrbExplosionbehaviour [m, n] = true;
						}
					}
				}
				break;
			case 7:
				Color[] pix7 = tex2DSecondExplosion3.GetPixels (p, q, width, height);
				for (int m = 0; m < width; m++) {
					for (int n = 0; n < height; n++) {
						ColorArray [m, n] = pix7 [iPixelCounter];
						iPixelCounter++;
						if (ColorArray [m, n].a == 1) {
							ExplosionProfile [i].arrbExplosionbehaviour [m, n] = true;
						}
					}
				}
				break;
			}
			// set Pixelcounter to 0 for next texture
			iPixelCounter = 0;
		}
		// Shows the Texture on Object, for gameplay not necessary
		/*Texture2D destTex = new Texture2D(width, height);
		destTex.SetPixels(pix);
		destTex.Apply();
		renderer.material.mainTexture = destTex;*/
	}
}
