using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

class Node
{
	public int m_iPreX;
	public int m_iPreY;
	public int m_iDistance;
	public int m_iTotalWeight;
	
	public int m_iPosX;
	public int m_iPosY;
	
	public bool m_bInAir;

	public Node (int prex, int prey, int distance, int posx, int posy)
	{
		this.m_iPreX = prex;
		this.m_iPreY = prey;
		this.m_iDistance = distance;
		this.m_iPosX = posx;
		this.m_iPosY = posy;
		this.m_bInAir = false;
	}
	
	public Node (int prex, int prey, int distance, int posx, int posy, bool bInAirFlag)
	{
		this.m_iPreX = prex;
		this.m_iPreY = prey;
		this.m_iDistance = distance;
		this.m_iPosX = posx;
		this.m_iPosY = posy;
		this.m_bInAir = bInAirFlag;
	}
}

public class Pathfinder
{
	private int m_iStartX;
	private int m_iStartY;
	private int m_iEndX;
	private int m_iEndY;
	
	private List<Node> m_OpenList;
	private List<Node> m_ClosedList;
	private List<Node> m_Successors;
	private List<Node> m_FinalPath;
	private List<Vector2> m_vec2FinalPath;

	private List<GameObject> m_DebugList;

	private Map m_Map;

	private GameObject m_goDebug;

	public bool m_bDebugMode;

	public Pathfinder(Map map, GameObject goDebug)
	{
		m_OpenList = new List<Node>();
		m_ClosedList = new List<Node>();
		m_Successors = new List<Node>();
		m_FinalPath = new List<Node>();
		m_vec2FinalPath = new List<Vector2>();

		m_DebugList = new List<GameObject>();

		m_Map = map;

		m_goDebug = goDebug;
	}

	public bool StartPathing(int iFromX, int iFromY, int iToX, int iToY)
	{
		m_OpenList.Clear();
		m_ClosedList.Clear();
		m_FinalPath.Clear();

		m_iStartX = iFromX;
		m_iStartY = iFromY;
		m_iEndX = iToX;
		m_iEndY = iToY;

		Node startNode = new Node(-1, -1, 0, iFromX, iFromY);
		startNode.m_iTotalWeight = GetEstimatedDistance(startNode, iToX, iToY);

		Node currentNode = startNode;

		m_OpenList.Add (currentNode);

		int iLoopLimit = 1000;
		int iSafetyIterator = 0;

		bool bPathFound = false;

		do
		{
			++iSafetyIterator;

			int iMinWeight = int.MaxValue;

			foreach (Node itNode in m_OpenList)
			{
				if (iMinWeight > itNode.m_iTotalWeight)
				{
					iMinWeight = itNode.m_iTotalWeight;
					currentNode = itNode;
				}
			}
			if (currentNode.m_iPosX == m_iEndX && currentNode.m_iPosY == m_iEndY)
			{
				bPathFound = true;
			}
			m_OpenList.Remove (currentNode);
			m_ClosedList.Add (currentNode);
			
			Expand(currentNode);
		} while (m_OpenList.Count > 0 && iSafetyIterator < iLoopLimit);

		if (bPathFound)
		{
			Node SortNode = m_ClosedList.First (x => x.m_iPosX == m_iEndX && x.m_iPosY == m_iEndY);
			while (SortNode != null)
			{
				m_FinalPath.Add (SortNode);
				m_vec2FinalPath.Add (new Vector2(SortNode.m_iPosX, SortNode.m_iPosY));
				if (m_bDebugMode)
				{
					if (m_goDebug != null)
					{
						m_DebugList.Add ((GameObject)Object.Instantiate(m_goDebug, Map.IndexToWorldOffset(SortNode.m_iPosX, SortNode.m_iPosY),
						                                    Quaternion.identity));
					}
					else
					{
						Debug.LogWarning ("No Debug Prefab assigend.");
					}
				}
				// used delegate here --> linq lambda throws InvalidOperationException
				// --> try-catch not efficient compared to delegate
				SortNode = m_ClosedList.Find (
					delegate (Node delNode)
					{
					return (delNode.m_iPosX == SortNode.m_iPreX && delNode.m_iPosY == SortNode.m_iPreY);
					}
					);
			}
		}

		return bPathFound;
	}

	public Vector2 GetNextPosition(Vector2 currentPos)
	{
		int iPosX = (int)currentPos.x;
		int iPosY = (int)currentPos.y;

		Node wantedNode = m_FinalPath.First (x => x.m_iPreX == iPosX && x.m_iPreY == iPosY);
		if (wantedNode != null)
			return new Vector2(wantedNode.m_iPosX, wantedNode.m_iPosY);
		else
			return new Vector2(-1, -1);
	}

	public void OverrideFinalPath(List<Vector2> path)
	{
		if (path == null)
			return;
		if (path.Count < 1)
			return;

		m_FinalPath.Clear();

		m_FinalPath.Add (new Node(-1, -1, 0, (int)path[0].x, (int)path[0].y));

		for (int i = 1; i < path.Count; ++i)
		{
			m_FinalPath.Add (new Node((int)path[i - 1].x, (int)path[i - 1].y, 0, (int)path[i].x, (int)path[i].y));

			if (m_bDebugMode)
			{
				m_DebugList.Add ((GameObject)Object.Instantiate(m_goDebug, Map.IndexToWorldOffset((int)path[i].x, (int)path[i].y),
				                                                Quaternion.identity));
			}
		}
	}

	public int GetPathLength()
	{
		if (m_FinalPath != null)
			return m_FinalPath.Count;

		return -1;
	}

	public List<Vector2> GetPath()
	{
		return m_vec2FinalPath;
	}

	private void Expand(Node currentNode)
	{
		m_Successors.Clear();

		if (!currentNode.m_bInAir || m_Map.IsClimbable(currentNode.m_iPosX, currentNode.m_iPosY))
		{
			// right
			if (m_Map.IsPassable(currentNode.m_iPosX + 1, currentNode.m_iPosY))
			{
				// right on floor
				if (!m_Map.IsInAir(currentNode.m_iPosX + 1, currentNode.m_iPosY)|| m_Map.IsClimbable(currentNode.m_iPosX + 1, currentNode.m_iPosY))
				{
					//Debug.Log ("test right: " + (currentNode.m_iPosX + 1) + ", " + currentNode.m_iPosY);
					m_Successors.Add (new Node(-1, -1, currentNode.m_iDistance + 1, currentNode.m_iPosX + 1, currentNode.m_iPosY, false));
				}
				// right falling one block down
				else
				{
					m_Successors.Add (new Node(-1, -1, currentNode.m_iDistance + 1, currentNode.m_iPosX + 1, currentNode.m_iPosY, true));
				}
			}
			// left
			if (m_Map.IsPassable(currentNode.m_iPosX - 1, currentNode.m_iPosY)|| m_Map.IsClimbable(currentNode.m_iPosX - 1, currentNode.m_iPosY))
			{
				// left on floor
				if (!m_Map.IsInAir(currentNode.m_iPosX - 1, currentNode.m_iPosY))
				{
					//Debug.Log ("test left: " + (currentNode.m_iPosX - 1) + ", " + currentNode.m_iPosY);
					m_Successors.Add (new Node(-1, -1, currentNode.m_iDistance + 1, currentNode.m_iPosX - 1, currentNode.m_iPosY, false));
				}
				// left falling one block down
				else
				{
					m_Successors.Add (new Node(-1, -1, currentNode.m_iDistance + 1, currentNode.m_iPosX - 1, currentNode.m_iPosY, true));
				}
			}
		}
		// down
		if (m_Map.IsPassable(currentNode.m_iPosX, currentNode.m_iPosY - 1))
		{
			//Debug.Log ("test down: " + currentNode.m_iPosX + ", " + (currentNode.m_iPosY - 1));
			if (!m_Map.IsInAir(currentNode.m_iPosX, currentNode.m_iPosY - 1))
				m_Successors.Add (new Node(-1, -1, currentNode.m_iDistance + 1, currentNode.m_iPosX, currentNode.m_iPosY - 1, false));
			else
				m_Successors.Add (new Node(-1, -1, currentNode.m_iDistance + 1, currentNode.m_iPosX, currentNode.m_iPosY - 1, true));
		}

		// up
		if (m_Map.IsClimbable(currentNode.m_iPosX, currentNode.m_iPosY))
		{
			m_Successors.Add (new Node(-1, -1, currentNode.m_iDistance + 1, currentNode.m_iPosX, currentNode.m_iPosY + 1, false));
		}
		//else if (m_Map.IsPassable(currentNode.m_iPosX, currentNode.m_iPosY + 1))
		//{
			/*
			bool bInAir = true;
			if (m_Map.IsClimbable(currentNode.m_iPosX, currentNode.m_iPosY))
				bInAir = false;
			m_Successors.Add (new Node(-1, -1, currentNode.m_iDistance + 1, currentNode.m_iPosX, currentNode.m_iPosY + 1, bInAir));
			*/
			//if (m_Map.IsClimbable(currentNode.m_iPosX, currentNode.m_iPosY))
				//m_Successors.Add (new Node(-1, -1, currentNode.m_iDistance + 1, currentNode.m_iPosX, currentNode.m_iPosY + 1, false));
		//}

		foreach (Node itNode in m_Successors)
		{
			//if (currentNode.m_bInAir && !currentNode.m_bDropping && itNode.m_bInAir)
				//continue;

			bool bContinuePlease = false;
			foreach (Node cLNode in m_ClosedList)
			{
				if (cLNode.m_iPosX == itNode.m_iPosX && cLNode.m_iPosY == itNode.m_iPosY)
				{
					//Debug.Log("closed list with " + nod.posx + ", " + nod.posy);
					bContinuePlease = true;
				}
			}
			if (bContinuePlease)
				continue;

			int iTotalEstimatedDistance = currentNode.m_iDistance + GetEstimatedDistance(itNode, m_iStartX, m_iStartY); // add f for distance to goal
			bContinuePlease = false;
			foreach (Node oLNode in m_OpenList)
			{
				if (oLNode.m_iPosX == itNode.m_iPosX && oLNode.m_iPosY == itNode.m_iPosY)
				{
					if (iTotalEstimatedDistance >= itNode.m_iDistance)
					{
						//Debug.Log ("continueing");
						bContinuePlease = true;
					}
				}
			}
			if (bContinuePlease)
				continue;
			
			itNode.m_iPreX = currentNode.m_iPosX;
			itNode.m_iPreY = currentNode.m_iPosY;
			itNode.m_iTotalWeight = iTotalEstimatedDistance;

			if (m_OpenList.Any (x => x.m_iPosX == itNode.m_iPosX && x.m_iPosY == itNode.m_iPosY))
				continue;	
			//Debug.Log ("push " + nod.posx + ", " + nod.posy);
			m_OpenList.Add (itNode);
		}
	}

	private int GetEstimatedDistance(Node currentNode, int iToX, int iToY)
	{
		return (Mathf.Abs(currentNode.m_iPosX - iToX)) + (Mathf.Abs(currentNode.m_iPosY - iToY));
	}

	public void DeleteDebugSphere()
	{
		foreach(GameObject go in m_DebugList)
		{
			Object.Destroy(go);
		}
		m_DebugList.Clear ();
	}
}