using UnityEngine;
using System.Collections;

public class Mouse2D : MonoBehaviour 
{
	public GameObject m_goLevelOrigin;

	public float fCameraSpeedFactorBorder = 10f; // standard 10
	public float fCameraSpeedFactorMiddleMouse = 0.1f; // standard 0.1
	public float m_fZOffset;
	public GameObject m_goPrieviewPlane;
	public GameObject m_goPrieviewPlaneClickedLeft;
	public GameObject m_goPrieviewPlaneClickedRight;

	public float m_fZoomLimitMin;
	public float m_fZoomLimitMax;
	public float m_fScrollXMax;
	public float m_fScrollYMax;

	public float m_fZoomMovementSpeed;

	private GameObject MainCamera;
		
	private Vector3 m_vec3MouseAtMiddlePress;

	private float fScrollborderQuotient = 16f; 
	private float m_fHitGridX = 0;
	private float m_fHitGridY = 0;
	private float m_fHitGridIntX = 0;
	private float m_fHitGridIntY = 0;
	private Vector3 vec3NewPreviewPlanPos;
	private Vector3 vec3PreviewPlaneStartPosition;
	private GameObject goPreviewPlane;
	private GameObject goPreviewPlaneClickedLeft;
	private GameObject goPreviewPlaneClickedRight;
	private bool bMousePressed;

	#region Zoom Variables
	private float m_fStartZ;
	public float m_fZoomSpeed;

	private float m_fMovementTreshold = 1f;
	private float m_fTime;
	private float m_fEntryPoint;

	private float m_fStartDistXYmag;
	private float m_fTotalSpeed;
	#endregion

	private Vector3 m_Vec3ZoomTarget;

	// References from other Scripts
	private Map refMap;
	private DwarfChar refDwarfChar;
	private HotSeat refHotSeat;
	private Team refTeam;
	private ActionConfigurator refActionConfigurator;

	public float HitGridX
	{
		get { return m_fHitGridX; }
	}
	public float HitGridY
	{
		get { return m_fHitGridY; }
	}
	public float HitGridIntX
	{
		get { return m_fHitGridIntX; }
	}
	public float HitGridIntY
	{
		get { return m_fHitGridIntY; }
	}

	void Awake()
	{
		refHotSeat = GetComponent<HotSeat>();
		refMap = GetComponent<Map> ();
		refDwarfChar = GetComponent<DwarfChar>();
		refTeam = GetComponent<Team>();
		refActionConfigurator = GetComponent<ActionConfigurator>();
		MainCamera = GameObject.FindWithTag("MainCamera");
	}
	// Use this for initialization
	void Start () 
	{
		// Instantiate Prieviewplane
		vec3PreviewPlaneStartPosition = new Vector3(-5, -5, -0.81f);
		vec3NewPreviewPlanPos         = new Vector3((float)m_fHitGridIntX * refMap.m_gridX,
		                                            (float)m_fHitGridIntY  * refMap.m_gridY, -0.81f);
		goPreviewPlane        = Instantiate(m_goPrieviewPlane, vec3NewPreviewPlanPos,
		                             m_goPrieviewPlane.transform.rotation) as GameObject;
		goPreviewPlaneClickedLeft = Instantiate(m_goPrieviewPlaneClickedLeft, vec3NewPreviewPlanPos,
		                                    m_goPrieviewPlane.transform.rotation) as GameObject;
		goPreviewPlaneClickedRight= Instantiate(m_goPrieviewPlaneClickedRight, vec3NewPreviewPlanPos,
		                                        m_goPrieviewPlane.transform.rotation) as GameObject;


		m_Vec3ZoomTarget = new Vector3(-1, -1, -1);
		Vector3 pos = Map.IndexToWorldOffset(1 * (int)refMap.m_gridX, 32 * (int)refMap.m_gridY);
		// Debug.Log("Mouse2D: pos " + pos);
		MainCamera.transform.position = new Vector3(pos.x, pos.y);

		m_fStartZ = MainCamera.camera.transform.position.z;

		m_fStartDistXYmag = -1f;
	}
	
	// Update is called once per frame
	void Update () 
	{
		// UpdatePrieviewPlaneposition
		vec3NewPreviewPlanPos.x = m_fHitGridIntX  * refMap.m_gridX;
		vec3NewPreviewPlanPos.y = m_fHitGridIntY  * refMap.m_gridY;
		if(!bMousePressed)
		{
			goPreviewPlane.transform.position = vec3NewPreviewPlanPos;
		}
		if(Input.GetMouseButtonDown(0)) // left mouse pressed?
		{
			bMousePressed = true;
			goPreviewPlaneClickedLeft.transform.position = vec3NewPreviewPlanPos;
			/*
			refMap.IsDwarfAtPosition((int)m_fHitGridIntX, (int)m_fHitGridIntY);
			refMap.IsDwarfOfTeam1((int)m_fHitGridIntX, (int)m_fHitGridIntY);
			refMap.IsDwarfOfTeam2((int)m_fHitGridIntX, (int)m_fHitGridIntY);
			refMap.IsActiveDwarfAtPosition((int)m_fHitGridIntX, (int)m_fHitGridIntY);
			refMap.IsDwarfFromActiveTeam((int)m_fHitGridIntX, (int)m_fHitGridIntY);
			refHotSeat.SetActiveDwarf((int)m_fHitGridIntX, (int)m_fHitGridIntY);*/

			DwarfChar ClickedDwarf = refTeam.GetDwarfCharByPosition((int)m_fHitGridIntX, (int)m_fHitGridIntY);
			if(ClickedDwarf && !ClickedDwarf.Active && ClickedDwarf.iTeamID == refHotSeat.GetActiveTeam())
			{
				if(refTeam.DoesActiveTeamHaveEnoughActionpoints(refActionConfigurator.ChangeActiveDwarf))
				   {
					refHotSeat.SetAllDwarfesToNotActive();
					refTeam.AdjustActionpointsInTeam(-refActionConfigurator.ChangeActiveDwarf,ClickedDwarf.iTeamID);
					ClickedDwarf.Active = true;
					refHotSeat.ActivateOutlineShader(ClickedDwarf);
					Debug.Log("You clicked a non active dwarf");
				}
				else
					Debug.Log("Not enough Actionpoints for Activation other Dwarf of your Team");
			}



			// to startposition
			goPreviewPlaneClickedRight.transform.position = vec3PreviewPlaneStartPosition;
			goPreviewPlane.transform.position = vec3PreviewPlaneStartPosition;
		}
		if(Input.GetMouseButtonDown(1)) // right mouse pressed?
		{
			bMousePressed = true;
			goPreviewPlaneClickedRight.transform.position = vec3NewPreviewPlanPos;

			goPreviewPlaneClickedLeft.transform.position = vec3PreviewPlaneStartPosition;
			goPreviewPlane.transform.position = vec3PreviewPlaneStartPosition;
		}
		if(Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1)) // left or right mouse pressed?
		{
			bMousePressed = false;
			goPreviewPlaneClickedLeft.transform.position = vec3PreviewPlaneStartPosition;
			goPreviewPlaneClickedRight.transform.position = vec3PreviewPlaneStartPosition;
			goPreviewPlane.transform.position = vec3NewPreviewPlanPos;
		}

		// Camera
		Transform camTrans = MainCamera.transform;
		Vector2 vec2CamOffset = new Vector2();
		vec2CamOffset.x = Mathf.Cos(MainCamera.camera.fieldOfView) * (camTrans.position.z - m_fStartZ);
		vec2CamOffset.y = vec2CamOffset.x / MainCamera.camera.aspect;

		if (m_Vec3ZoomTarget.x == -1f)
		{
			// move Camera if mouse is beside left border
			if(Input.mousePosition.x < Screen.width / fScrollborderQuotient)
			{
				camTrans.Translate(
					-((((Screen.width / fScrollborderQuotient) - Input.mousePosition.x)) / (Screen.width / fScrollborderQuotient))
					* fCameraSpeedFactorBorder * Time.deltaTime, 0, 0);
			}
			// move Camera if mouse is beside right border
			else if(Input.mousePosition.x > Screen.width - Screen.width / fScrollborderQuotient)
			{
				camTrans.Translate(
					(Input.mousePosition.x - (Screen.width - Screen.width / fScrollborderQuotient))/ (Screen.width / fScrollborderQuotient)
					* fCameraSpeedFactorBorder * Time.deltaTime, 0, 0);
			}
			// move Camera if mouse is beside down border
			if(Input.mousePosition.y < Screen.height / fScrollborderQuotient)
			{
				camTrans.Translate(
					0, -((((Screen.height / fScrollborderQuotient) - Input.mousePosition.y)) / (Screen.height / fScrollborderQuotient))
					* fCameraSpeedFactorBorder * Time.deltaTime, 0);
			}
			// move Camera if mouse is beside up border
			else if(Input.mousePosition.y > Screen.height - Screen.height / fScrollborderQuotient)
			{
				camTrans.Translate(
					0, (Input.mousePosition.y - (Screen.height - Screen.height / fScrollborderQuotient))/ (Screen.height / fScrollborderQuotient)
					* fCameraSpeedFactorBorder * Time.deltaTime, 0);
			}
			// Zoom main Camera 
			if (Input.GetAxis("Mouse ScrollWheel") > 0) // forward
			{
				camTrans.Translate(0, 0, 1);
			}
			if (Input.GetAxis("Mouse ScrollWheel") < 0) // back
			{
				camTrans.Translate(0, 0, -1);
			}

			// Middle Mouse Button
			if (Input.GetMouseButtonDown (2)) 
			{
				// saves the mouseposition at MiddleMousePress
				m_vec3MouseAtMiddlePress = Input.mousePosition;
			}

			if(Input.GetMouseButton(2))
			{
				// translates the cameraposition while middle mouse is pressed
				camTrans.Translate((Input.mousePosition.x - m_vec3MouseAtMiddlePress.x) * (1/fCameraSpeedFactorBorder) * Time.deltaTime,
				                   (Input.mousePosition.y - m_vec3MouseAtMiddlePress.y) * (1/fCameraSpeedFactorBorder) * Time.deltaTime, 
				                    0.0f);
			}
		}
		else
		{
			Vector2 fDifferenceXY = (new Vector2(m_Vec3ZoomTarget.x, m_Vec3ZoomTarget.y) - new Vector2(camTrans.position.x, camTrans.position.y));
			float fDifferenceXYMag = fDifferenceXY.sqrMagnitude;

			if (m_fStartDistXYmag <= 0f)
			{
				//Debug.Log ("setting");
				m_fStartDistXYmag = fDifferenceXYMag;
				m_fTotalSpeed = m_fStartDistXYmag / m_fZoomMovementSpeed;

				float avgSpeed = (m_fTotalSpeed + m_fMovementTreshold / m_fTotalSpeed) / 2.0f;
				m_fTime = m_fStartDistXYmag / avgSpeed;
				m_fEntryPoint = (m_fTime / 2f) * avgSpeed;

				m_fZoomSpeed = (m_fZoomLimitMax - camTrans.position.z) / m_fTime;

			}
			//Debug.Log ("m_fStartDistXYmag: " + m_fStartDistXYmag);
			//Debug.Log ("m_fTotalSpeed: " + m_fTotalSpeed);

			//Debug.Log ("fDifferenceXYMag: " + fDifferenceXYMag);
			float fRelativeSpeed = fDifferenceXYMag / m_fStartDistXYmag;

			Vector2 dir = fDifferenceXY / fDifferenceXYMag;
			float fRelativeZoomSpeed = -(fDifferenceXYMag - m_fEntryPoint) / m_fEntryPoint;
			//Debug.Log ("m_fTotalSpeed: " + m_fTotalSpeed);
			//Debug.Log ("fRelativeSpeed: " + fRelativeSpeed);
			//Debug.Log ("fRelativeZoomSpeed: " + fRelativeZoomSpeed);
			if (fRelativeSpeed > 0f)
			{
				Vector3 tempMove = new Vector3(m_fTotalSpeed * fRelativeSpeed * dir.x, m_fTotalSpeed * fRelativeSpeed * dir.y, fRelativeZoomSpeed * m_fZoomSpeed);
				camTrans.Translate (tempMove);
			}
	
			if (fDifferenceXYMag < m_fMovementTreshold)
			{
				m_Vec3ZoomTarget = new Vector3(-1f, -1f, -1f);
			}
		}

		if ((camTrans.position.x - vec2CamOffset.x) < 0f)
			camTrans.position = new Vector3(vec2CamOffset.x, camTrans.position.y, camTrans.position.z);
		if ((camTrans.position.x + vec2CamOffset.x) > m_fScrollXMax)
			camTrans.position = new Vector3(m_fScrollXMax - vec2CamOffset.x, camTrans.position.y, camTrans.position.z);
		if ((camTrans.position.y - vec2CamOffset.y) < 0f)
			camTrans.position = new Vector3(camTrans.position.x, vec2CamOffset.y, camTrans.position.z);
		if ((camTrans.position.y + vec2CamOffset.y) > m_fScrollYMax)
			camTrans.position = new Vector3(camTrans.position.x, m_fScrollYMax - vec2CamOffset.y, camTrans.position.z);

		if (camTrans.position.z < m_fZoomLimitMin)
			camTrans.position = new Vector3(camTrans.position.x, camTrans.position.y, m_fZoomLimitMin);
		if (camTrans.position.z > m_fZoomLimitMax)
			camTrans.position = new Vector3(camTrans.position.x, camTrans.position.y, m_fZoomLimitMax);

		// find cube in Level
		//Debug.Log("Mouse2D: Cameraoffset " + m_fZOffset);
		Vector3 p = MainCamera.camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -camTrans.position.z - m_fZOffset));

		// RaycastHit hit;
		float fXPositionGrid = 0;
		float fYPositionGrid = 0;

		if (refMap != null)
		{
			if (refMap.m_arrMap == null)
			{
				Debug.Log("Mouse2D: probably no texture to LoadLevelTexture Script added");
			}
			//TEST
			if (refMap.m_arrMap != null)
			{
				for (int i = 0; i < refMap.m_arrMap.GetLength (0); i++) // width
				{ 
					for (int j = 0; j < refMap.m_arrMap.GetLength (1); j++) // height
					{ 
						fXPositionGrid = refMap.m_arrMap [i, j].iXIndex * refMap.m_gridX;
						fYPositionGrid = refMap.m_arrMap [i, j].iYIndex * refMap.m_gridY;
						if (fXPositionGrid - refMap.m_gridX / 2 <= p.x && 
							refMap.m_arrMap [i, j].iXIndex * refMap.m_gridX + refMap.m_gridX / 2 > p.x) 
						{
							m_fHitGridX = refMap.m_arrMap [i, j].iXIndex * refMap.m_gridX;
						}
						if (fYPositionGrid - refMap.m_gridY / 2 <= p.y &&
							fYPositionGrid + refMap.m_gridY / 2 > p.y) 
						{
							m_fHitGridY = refMap.m_arrMap [i, j].iYIndex * refMap.m_gridY;
						}
					}
				}
				m_fHitGridIntX = m_fHitGridX / refMap.m_gridX;
				m_fHitGridIntY = m_fHitGridY / refMap.m_gridY;
			}
		}
	}

	public void ZoomToPosition(Vector3 vec3Pos)
	{
		m_Vec3ZoomTarget = vec3Pos;
	}
	public void StopZoom()
	{
		m_Vec3ZoomTarget = new Vector3(-1, -1, -1);
	}
}


