using UnityEngine;
using System.Collections;

public class GUISetup : MonoBehaviour {

	//Bools to check for active GUI
	int GUIID = 0;

	public Texture2D[] Cursor;

	float ScreenWidth;
	float ScreenHeight;
	float BackgroundWidth;
	float BackgroundHeight;
	float ImageWidth;
	float ImageHeight;

	int GUIStage = 0;

	public string VersionNumber;

	public GUIStyle TextFont;
	public GUIStyle ButtonStyle;
	
	public MovieTexture movie;
	public MovieTexture credits;

	public Texture2D Logo;

	public Texture2D[] Background;

	public float XOffsetPercentageMain;
	public float YOffsetPercentageMain;
	public Texture2D ButtonBlank;
	public Texture2D[] ButtonsMain;
	public Texture2D[] ButtonsServer;
	public Texture2D ButtonBack;
	public Texture2D[] ButtonsHost;
		string Chat = "H..h..hallo??";
		public Texture2D[] ButtonsMaps;
		public string[]	Map;
		public string Mapname = " ";
		public Texture2D[] ButtonsFriends;
		public bool MapsActive = true;
		bool InviteActive = false;
	public Texture2D[] ButtonsClassSelect;
	public Texture2D[] ClassIcons;
	public string[] ClassNames;
	bool ClassesInit = false;
	public int[] ClassIDsTeam1;
	public int[] ClassIDsTeam2;

	public float ButtonsOffsetX;

	bool m_bIsCreditsPressed = false;

	Vector2 scrollPosition = Vector2.zero;
	public Texture2D[] ButtonsOptions;
	public Texture2D[] ButtonsOptions2;
	public float h_SliderValue = 0.2f;
	bool m_bShowSlider = false;

	public float LabelWidth = 0.62f;
	public float LabelHeight = 0.15f;
	public Texture2D[] Label;

	bool IsKeyDown = false;
	bool IsKeyReleased = false;

	int CharacterCount = 0;
	
	public float XOffsetPercentage;
	public float YOffsetPercentage;

	public GUIStyle m_CharacterSelection;

	void Start() {
		Mapname = Map[PlayerPrefs.GetInt("MapSelection")];
			
		Screen.showCursor = false;
	
		movie.Play();

		h_SliderValue = PlayerPrefs.GetFloat ("h_SliderValue");

		//Debug.Log (PlayerPrefs.GetInt ("m_iGameQuit"));
		if (!PlayerPrefs.HasKey ("m_iGameQuit"))
						PlayerPrefs.SetInt ("m_iGameQuit", 2);

		if (PlayerPrefs.GetInt ("m_iGameQuit") == 2) 
				{
						GUIStage = 2;
				} 
				else 
				{
						GUIStage = 0;
				}

	}

	void OnGUI()
	{
		StartingScreen ();

		GUI.Label(new Rect(10,10,150,150),VersionNumber);

		AudioListener.volume = h_SliderValue;

				//Relative Position
				ScreenWidth = (Screen.width / 2);
				ScreenHeight = (Screen.height / 2);

				BackgroundWidth = ScreenWidth - (Screen.width * 0.28f);
				BackgroundHeight = ScreenHeight - (Screen.height * 0.475f);

				//Relative Resolution of Background
				ImageWidth = Screen.width * 0.925f;
				ImageHeight = Screen.height * 0.925f;

		float ButtonSizeX = (ImageWidth / 5);


		if (GUIStage == 2) {

						if (GUIID == 0) {
								GUI.Label (new Rect ((int)(ScreenWidth - (ImageHeight / 2)), (int)(BackgroundHeight), (int)(ImageWidth), (int)(ImageHeight)), Background [0], GUIStyle.none);
				GUI.Label (new Rect ((int)(ScreenWidth - (ButtonSizeX / 2)), (int)((ScreenHeight / 4) -25), (int)(ButtonSizeX), (int)(ImageHeight / 5)), Label [0], GUIStyle.none);

								for (int i = 0; i<ButtonsMain.GetLength(0); ++i) {

										// Make the first button. If it is pressed, Application.Loadlevel (1) will be executed
										if (GUI.Button (new Rect ((int)(ScreenWidth - ButtonSizeX / 2), (int)(BackgroundHeight + ImageHeight / YOffsetPercentageMain + (i * (ImageHeight * XOffsetPercentageMain))), (int)(ButtonSizeX), (int)(ImageHeight / 5 / 2)), ButtonsMain [i], ButtonStyle)) {

												switch (i) {

												//Tutorial
												case 0:
														//Debug.Log ("loading level");
														//PlayerPrefs.SetFloat ("h_SliderValue", h_SliderValue);
														//Application.LoadLevel (1);
														break;
												//Join
												case 1:
														GUIID = 1;
														break;
												//Host
												case 2:
														GUIID = 2;
														break;
												//Options
												case 3:
														GUIID = 3;
														break;
												case 4:
														m_bIsCreditsPressed = true;
														break;
												case 5:
														PlayerPrefs.SetInt ("m_iGameQuit", 1);
														Application.Quit ();
														break;
												}
										}	
//						// Make the second button.
//						if (GUI.Button (new Rect ((int)(ScreenWidth - (ImageWidth / 5 / 2)), (int)(ScreenHeight - ButtonOffsetY), (int)(ImageWidth / 5), (int)(ImageHeight / 5 / 2)), JoinButton, GUIStyle.none)) {
//								GUIID = 1;
//						}
//						if (GUI.Button (new Rect ((int)(ScreenWidth - (ImageWidth / 5 / 2)), (int)(ScreenHeight - ButtonOffsetY), (int)(ImageWidth / 5), (int)(ImageHeight / 5 / 2)), HostButton, GUIStyle.none)) {
//								GUIID = 2;
//						}	
//						// Make the second button.
//						if (GUI.Button (new Rect ((int)(ScreenWidth - (ImageWidth / 5 / 2)), (int)(ScreenHeight - ButtonOffsetY), (int)(ImageWidth / 5), (int)(ImageHeight / 5 / 2)), OptionenButton, GUIStyle.none)) {
//								GUIID = 3;
//						}
//						// Make the second button.
//						if (GUI.Button (new Rect ((int)(ScreenWidth - (ImageWidth / 5 / 2)), (int)(ScreenHeight - ButtonOffsetY), (int)(ImageWidth / 5), (int)(ImageHeight / 5 / 2)), BeendenButton, GUIStyle.none)) {
//								Application.Quit();
//						}
								}
						}
						//Join
						if (GUIID == 1) {
								GUI.Label (new Rect ((int)(ScreenWidth - (ImageHeight / 2)), (int)(BackgroundHeight), (int)(ImageWidth), (int)(ImageHeight)), Background [1], GUIStyle.none);
								GUI.Label (new Rect ((int)(ScreenWidth - (ImageWidth * 0.14f)), (int)((ScreenHeight - 255) * 0.26f), (int)(ImageWidth * LabelWidth), (int)(ImageHeight * LabelHeight)), Label [1], GUIStyle.none);

								//ScrollWindowfor Servers
								scrollPosition = GUI.BeginScrollView (new Rect ((int)(ScreenWidth - (ImageWidth * 0.15f)), (int)(BackgroundHeight + ImageHeight / 4.6), ImageHeight * 0.51f, ImageHeight * 0.48f), scrollPosition, new Rect (0, 0, 0, (int)(ButtonsServer.GetLength (0) * 20)));
								for (int i = 0; i<ButtonsServer.GetLength(0); ++i) {
										//GUI.Button (new Rect (0,0,100,20), "Top-left");
										//GUI.Button (new Rect (0,180,100,20), "Bottom-left");
				
										// Server selections
										if (GUI.Button (new Rect ((0), (int)(0 + (i * (ImageHeight * 0.05f))), 250, 20), ButtonBlank, ButtonStyle)) {
												switch (i) {
												case 0:
						//Select server here
														break;
						
												}
										}	
								}	


								// End the scroll view that we began above.
								GUI.EndScrollView ();


								if (GUI.Button (new Rect ((int)(ScreenWidth - (ImageWidth / 10.39f)), (int)(BackgroundHeight + ImageHeight / 1.72 + (ImageHeight * 0.17)), (int)(ImageWidth / 5), (int)(ImageHeight / 5 / 2)), ButtonBack, ButtonStyle)) {
										GUIID = 0;
								}	
			
						}
						//Host
						if (GUIID == 2) {
								GUI.Label (new Rect ((int)(ScreenWidth - (ImageHeight / 2)), (int)(BackgroundHeight), (int)(ImageWidth), (int)(ImageHeight)), Background [2], GUIStyle.none);
								GUI.Label (new Rect ((int)(ScreenWidth - (ImageWidth * 0.305)), (int)((ScreenHeight - 255) * 0.26f), (int)(ImageWidth * LabelWidth), (int)(ImageHeight * LabelHeight)), Label [2], ButtonStyle);

								//Map Selection
								GUI.Label (new Rect ((int)(ScreenWidth * 0.74), (int)(ScreenHeight * 0.43), (int)(ImageWidth), (int)(ImageHeight)), "Karte: "+Mapname, TextFont);
								//Chat = GUI.TextField (new Rect ((int)(ScreenWidth * 0.74f), (int)(ScreenHeight * 0.79), (int)(ImageWidth / 3), (int)(20)), Chat, TextFont);

								for (int i = 0; i<ButtonsHost.GetLength(0); ++i) {
										// Make the first button. If it is pressed, Application.Loadlevel (1) will be executed
										if (GUI.Button (new Rect ((int)(ScreenWidth - (ImageWidth * 0.14f)), (int)(BackgroundHeight + ImageHeight / 2f + (i * (ImageHeight * 0.09f))), (int)(ImageWidth * 0.13), (int)(ImageHeight / 5 / 4)), ButtonsHost [i], ButtonStyle)) {
												switch (i) {
												case 0:
														MapsActive = true;
														InviteActive = false;
														break;
												case 1:
														InviteActive = true;
														MapsActive = false;
														break;
												case 2:
														PlayerPrefs.SetFloat ("h_SliderValue", h_SliderValue);
														//Application.LoadLevel (1);
														GUIStage = 3;
														break;
							
												}
										}	
								}
								if (GUI.Button (new Rect ((int)(ScreenWidth - (ImageWidth * (-0.03f))), (int)(BackgroundHeight + ImageHeight / (-4.45f) + (ImageHeight)), (int)(ImageWidth / 8), (int)(ImageHeight / 5 / 4)), ButtonBack, ButtonStyle)) {
										GUIID = 0;
								}
								if (MapsActive == true) {
										int imaps = 0;
										//ScrollWindow for Maps
										scrollPosition = GUI.BeginScrollView (new Rect ((int)(ScreenWidth - (ImageWidth * (-0.01f))), (int)(ImageHeight * 0.485f), ImageWidth * 0.16f, ImageHeight * 0.28f), scrollPosition, new Rect (0, 0, 0, (int)(ButtonsMaps.GetLength (0) * 20)));
				
										for (imaps = 0; imaps<ButtonsMaps.GetLength(0); ++imaps) {
					
												//Map selections
												if (GUI.Button (new Rect (0, (int)(28 + (imaps * (ImageHeight * 0.04f))), 100, 20), Map [imaps], TextFont)) {
														//Debug.Log ("Maps should show");
														//MAP SELECTION!!
														Mapname = Map [imaps];
														PlayerPrefs.SetInt("MapSelection", imaps);
												}	
										}	
				
										// End the scroll view that we began above.
										GUI.EndScrollView ();

								}
								if (InviteActive == true) {
										scrollPosition = GUI.BeginScrollView (new Rect ((int)(ScreenWidth - (ImageWidth * (-0.01f))), (int)(ImageHeight * 0.485f), ImageWidth * 0.16f, ImageHeight * 0.28f), scrollPosition, new Rect (0, 0, 0, (int)(ButtonsFriends.GetLength (0) * 20)));
				
										for (int imaps = 0; imaps<ButtonsFriends.GetLength(0); ++imaps) {
					
												//Map selections
												if (GUI.Button (new Rect (0, (int)(0 + (imaps * (ImageHeight * 0.04f))), 100, 20), ButtonBlank, ButtonStyle)) {
														switch (imaps) {
														case 0:
						//Select friend here
																break;
							
														}
												}	
										}	
										// End the scroll view that we began above.
										GUI.EndScrollView ();
								}
						}
						//Options
						if (GUIID == 3) {
								GUI.Label (new Rect ((int)(ScreenWidth - (ImageHeight / 2)), (int)(BackgroundHeight), (int)(ImageWidth), (int)(ImageHeight)), Background [3], GUIStyle.none);
								GUI.Label (new Rect ((int)(ScreenWidth - (ImageWidth * 0.305f)), (int)((ScreenHeight - 255) * 0.26f), (int)(ImageWidth * LabelWidth), (int)(ImageHeight * LabelHeight)), Label [3], ButtonStyle);
			
								for (int i = 0; i<ButtonsOptions.GetLength(0); ++i) {
										// Make the first button. If it is pressed, Application.Loadlevel (1) will be executed
										if (GUI.Button (new Rect ((int)(ScreenWidth - (ImageWidth * 0.15f)), (int)(BackgroundHeight + ImageHeight / 4.6 + (i * (ImageHeight * 0.09f))), (int)(ImageWidth / 9), (int)(ImageHeight / 5 / 4)), ButtonsOptions [i], ButtonStyle)) {
											switch (i) {
											case 2:
												m_bShowSlider = true;
												break;
											}
										}	
								}
								for (int i = 0; i<ButtonsOptions2.GetLength(0); ++i) {
										// Make the first button. If it is pressed, Application.Loadlevel (1) will be executed
										if (GUI.Button (new Rect ((int)(BackgroundWidth + ImageWidth * 0.151f + (i * (ImageWidth * 0.18f))), (int)(ScreenHeight - (ImageHeight * (-0.25))), (int)(ImageWidth / 9), (int)(ImageHeight / 5 / 4)), ButtonsOptions2 [i], ButtonStyle)) {

												switch (i) {
												case 0:
														break;
												case 1:
														m_bShowSlider = true;
														GUIID = 0;
							
														break;
												}

										}	
								}
							if(m_bShowSlider)
							{
							GUI.Label(new Rect (ScreenHeight*1.67f,ScreenHeight*0.54f-30,150,30),"Gesamtlautstärke", TextFont);
							h_SliderValue = GUI.HorizontalSlider(new Rect(ScreenHeight*1.67f,ScreenHeight*0.54f,100,30),h_SliderValue,0.0f,1.0f);
							}	
						}
						Vector3 mousePos = Input.mousePosition;
						Rect pos = new Rect (mousePos.x, Screen.height - 5 - mousePos.y, Cursor [0].width / 12, Cursor [0].height / 12);
						if (Input.GetKey (KeyCode.Mouse0)) {
								GUI.Label (pos, Cursor [1]);
						} else {
								GUI.Label (pos, Cursor [0]);
						}
			if(m_bIsCreditsPressed)
			{
				GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), credits);
				credits.Play();
				if(Input.anyKeyDown)
				{
					m_bIsCreditsPressed = false;
				}
				if ( credits.isPlaying == false ) {
					m_bIsCreditsPressed = false;
				}
			}
		} 
		//INTRO
		if(GUIStage == 0) 
		{
			GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), movie);
			
			if ( movie.isPlaying == false ) {
				Debug.Log("Movie has ended");
				GUIStage = 1;
			}
		}
		//LOGO
		if(GUIStage == 1)
		{
			GUI.Label (new Rect ((int)(ScreenWidth - (ButtonSizeX/2)), (int)(ScreenHeight*1.5f), (int)(ButtonSizeX), (int)(ImageHeight / 5)), "Beliebige Taste Drücken", ButtonStyle);
			GUI.Label (new Rect ((int)(ScreenWidth - (ButtonSizeX*(3.45/2))), (int)(ScreenHeight/2*(-0.9)), (int)(ButtonSizeX*3.45), (int)(ImageHeight / 2.5*3.45)), Logo, ButtonStyle);
		}
		//CHARACTER SELECTION
		if(GUIStage == 3)
		{
			GUI.Label (new Rect ((int)(ScreenWidth - (ImageHeight / 2)), (int)(BackgroundHeight), (int)(ImageWidth), (int)(ImageHeight)), Background [0], GUIStyle.none);
			Screen.showCursor = true;
			for (int i = 0; i<ButtonsClassSelect.GetLength(0); ++i) {
				// Make the first button. If it is pressed, Application.Loadlevel (1) will be executed
				if (GUI.Button (new Rect ((int)(BackgroundWidth + ImageWidth * 0.151f + (i * (ImageWidth * 0.18f))), (int)(ScreenHeight - (ImageHeight * (-0.25))), (int)(ImageWidth / 9), (int)(ImageHeight / 5 / 4)), ButtonsClassSelect [i], ButtonStyle)) {
					
					switch (i) {
					case 0:
						PlayerPrefsX.SetIntArray("Team1IDs", ClassIDsTeam1);
						PlayerPrefsX.SetIntArray("Team2IDs", ClassIDsTeam2);
						Application.LoadLevel (1);
						break;
					case 1:
						for(int k = 0; k<ClassIDsTeam1.Length; ++k)
						{
							ClassIDsTeam1[k] = 0;
							ClassIDsTeam2[k] = 0;
						}
						ClassesInit = false;	
						CharacterCount = 0;
						GUIStage = 2;
						break;
					}
					
				}	
			}
			//Class Selection
			for (int i = 0; i<ClassIcons.GetLength(0); ++i) {
				
				if(!ClassesInit)
					GUI.Label (new Rect ((int)(BackgroundWidth*1.7f), (int)(ScreenHeight-100+ (i * (ImageWidth * 0.05f))), (int)(ImageWidth / 3), (int)(ImageHeight / 5)), ""+ClassNames [0], m_CharacterSelection);

				
				if (GUI.Button (new Rect ((int)(BackgroundWidth*1.42f + (i * (ImageWidth * 0.14f))), (int)(ScreenHeight*0.3f), (int)(ImageWidth / 9), (int)(ImageHeight / 5)), ClassIcons [i], GUIStyle.none)) {
					if(CharacterCount<=3)
						ClassIDsTeam1[CharacterCount] = i;
					if(CharacterCount>3&&CharacterCount<=7)
						ClassIDsTeam2[CharacterCount-4] = i;
					CharacterCount++;
					ClassesInit = true;		
				}	
			}
				for(int i = 0; i<ClassIDsTeam1.GetLength(0); ++i)
				{ 
					if(ClassesInit)
					GUI.Label (new Rect ((int)(BackgroundWidth*1.7f), (int)(ScreenHeight-70+ (i * (ImageWidth * 0.05f))), (int)(ImageWidth / 3), (int)(ImageHeight / 5)), ""+ClassNames [ClassIDsTeam1[i]+1], m_CharacterSelection);
				}
			
				for(int i = 0; i<ClassIDsTeam2.GetLength(0); ++i)
				{ 
					if(ClassesInit && CharacterCount>4)
					GUI.Label (new Rect ((int)(BackgroundWidth*2.5f), (int)(ScreenHeight-70+ (i * (ImageWidth * 0.05f))), (int)(ImageWidth / 3), (int)(ImageHeight / 5)), ""+ClassNames [ClassIDsTeam2[i]+1], m_CharacterSelection);
				}
				if(CharacterCount<=4)
				GUI.Label (new Rect ((int)(BackgroundWidth*1.5f), (int)(ScreenHeight-125), (int)(ImageWidth / 3), (int)(ImageHeight / 5)), "Spieler 1: Wähle deine Zwerge", m_CharacterSelection);
			
				if(CharacterCount>4)
				GUI.Label (new Rect ((int)(BackgroundWidth*2.25f), (int)(ScreenHeight-125), (int)(ImageWidth / 3), (int)(ImageHeight / 5)), "Spieler 2: Wähle deine Zwerge", m_CharacterSelection);
			
			
		}
	}
	void StartingScreen()
	{
		if(GUIStage == 0)
		{
			if (Input.anyKeyDown) 
			{
				IsKeyDown = true;
			} 
			else 
			{
				
				IsKeyReleased = IsKeyDown;
				IsKeyDown = false;
				if(IsKeyReleased)
					GUIStage = 1;
			}
		}
		if(GUIStage == 1)
		{
			if (Input.anyKeyDown) 
			{
				IsKeyDown = true;
			} 
			else 
			{
	
				IsKeyReleased = IsKeyDown;
				IsKeyDown = false;
				if(IsKeyReleased)
				GUIStage = 2;
			}
		}
	}
}
//		if(GUIID == 1)
//		{
//			GUI.Label (new Rect ((int)(BackgroundWidth - 10), (int)(BackgroundHeight), (int)(ImageWidth), (int)(ImageHeight)), JoinBackground, GUIStyle.none);
//			GUI.Label (new Rect ((int)(ScreenWidth - (ImageWidth / 5 / 2)), (int)(ScreenHeight - 255), (int)(ImageWidth / 5), (int)(ImageHeight / 5)), JoinImage, GUIStyle.none);
//
//			//SERVERLIST GOES INSTEAD OF THIS
//			GUI.Label (new Rect ((int)(ScreenWidth - (ImageWidth / 5 / 2)), (int)(ScreenHeight - 105), (int)(ImageWidth / 5), (int)(ImageHeight / 5)), "Netzwerkspiel ist zu diesem Zeitpunkt\nnicht implementiert", GUIStyle.none);
//
//			if (GUI.Button (new Rect ((int)(ScreenWidth - (ImageWidth / 5 / 2)), (int)(ScreenHeight + 145), (int)(ImageWidth / 5), (int)(ImageHeight / 5 / 2)), BackButton, GUIStyle.none)) {
//				GUIID = 0;
//			}
//		}
//		if(GUIID == 2)
//		{
//			GUI.Label (new Rect ((int)(BackgroundWidth - 10), (int)(BackgroundHeight), (int)(ImageWidth), (int)(ImageHeight)), HostBackground, GUIStyle.none);
//			GUI.Label (new Rect ((int)(ScreenWidth - (ImageWidth / 5 / 2)), (int)(ScreenHeight - 305), (int)(ImageWidth / 5), (int)(ImageHeight / 5)), HostImage, GUIStyle.none);
//
//			if (GUI.Button (new Rect ((int)(ScreenWidth - (ImageWidth / 5 / 2)), (int)(ScreenHeight + 145), (int)(ImageWidth / 5), (int)(ImageHeight / 5 / 2)), BackButton, GUIStyle.none)) {
//				GUIID = 0;
//			}
//		}
//		if(GUIID == 3)
//		{
//			GUI.Label (new Rect ((int)(BackgroundWidth - 10), (int)(BackgroundHeight), (int)(ImageWidth), (int)(ImageHeight)), OptionsBackground, GUIStyle.none);
//			GUI.Label (new Rect ((int)(ScreenWidth - (ImageWidth / 5 / 2)), (int)(ScreenHeight - 305), (int)(ImageWidth / 5), (int)(ImageHeight / 5)), OptionsImage, GUIStyle.none);
//			
//			if (GUI.Button (new Rect ((int)(ScreenWidth - (ImageWidth / 5 / 2)), (int)(ScreenHeight + 145), (int)(ImageWidth / 5/1.5), (int)(ImageHeight / 5 / 3)), BackButton, GUIStyle.none)) {
//				GUIID = 0;
//			}
//			if (GUI.Button (new Rect ((int)(ScreenWidth - (ImageWidth / 5 / 2)), (int)(ScreenHeight + 145), (int)(ImageWidth / 5/1.5), (int)(ImageHeight / 5 / 3)), StandartButton, GUIStyle.none)) {
//
//			}