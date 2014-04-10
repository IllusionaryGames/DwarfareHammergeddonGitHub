using UnityEngine;
using System.Collections;

public class Controlls : MonoBehaviour {

	private GameObject Player;
	private float fPlayerPositionStartInGridX;
	private float fPlayerPositionStartInGridY;
	private int iPlayerPositionBeforeInGridX;
	private int iPlayerPositionBeforeInGridY;
	private Map map;
	
	void Awake()
	{
		map = GetComponent<Map> ();
		Player = GameObject.FindWithTag("Player");
	}
	// Use this for initialization
	void Start () 
	{
		fPlayerPositionStartInGridX = Player.transform.position.x / map.m_gridX;
		fPlayerPositionStartInGridY = Player.transform.position.y / map.m_gridY;
		iPlayerPositionBeforeInGridX = (int)fPlayerPositionStartInGridX;
		iPlayerPositionBeforeInGridY = (int)fPlayerPositionStartInGridY;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetMouseButtonDown (1)) 
		{

			// Player.transform.Translate( 0 , (float)map.m_gridY, (float)map.m_gridX);
		}
		if(Input.GetButtonDown("Horizontal"))
		{
			float fValueX = Input.GetAxis ("Horizontal"); 
			if(fValueX < 0)
			{
				fValueX = -1;
			}
			else if(fValueX > 0)
			{
				fValueX = 1;
			}

//			iTween.MoveTo(Player, iTween.Hash( "x"   , iPlayerPositionBeforeInGridX * map.m_gridX + map.m_gridX * fValueX));
			iPlayerPositionBeforeInGridX = iPlayerPositionBeforeInGridX + (int)fValueX;
		}
		else if(Input.GetButtonDown("Vertical"))
		{
			float fValueY = Input.GetAxis ("Vertical");
			if(fValueY < 0)
			{
				fValueY = -1;
			}
			else if(fValueY > 0)
			{
				fValueY = 1;
			}
			//iTween.MoveTo(Player, iTween.Hash( "y"   , iPlayerPositionBeforeInGridY * map.m_gridY + map.m_gridY * fValueY));
			iPlayerPositionBeforeInGridY = iPlayerPositionBeforeInGridY + (int)fValueY;

		}
	}
}
