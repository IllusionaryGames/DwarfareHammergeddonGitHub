using UnityEngine;
using UnityEditor;
using System.Collections;

public class FillingWindow : EditorWindow
{
	public GridEditor editor;

	public int m_iPositionX = -1;
	public int m_iPositionY = -1;

	public int m_iExtendX = -1;
	public int m_iExtendY = -1;

	public byte m_byActionID = 0;

	public short m_sTileID = 0;

	public PrefabProfile profile;

	void OnGUI()
	{
		GUILayout.BeginHorizontal();
		GUILayout.Label (" Start Position X ");
		m_iPositionX = Mathf.Max (EditorGUILayout.IntField(m_iPositionX, GUILayout.Width (50)));
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label (" Start Position Y ");
		m_iPositionY = Mathf.Max (EditorGUILayout.IntField(m_iPositionY, GUILayout.Width (50)));
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label (" m_iExtendX ");
		m_iExtendX = Mathf.Max (EditorGUILayout.IntField(m_iExtendX, GUILayout.Width (50)));
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label (" m_iExtendY ");
		m_iExtendY = Mathf.Max (EditorGUILayout.IntField(m_iExtendY, GUILayout.Width (50)));
		GUILayout.EndHorizontal();

		if (GUILayout.Button("Apply", GUILayout.Width(255)))
		{
			switch (m_byActionID)
			{
			case 1:
				editor.FillArea(m_iPositionX, m_iPositionY, m_iExtendX, m_iExtendY, m_sTileID , profile);
				break;
			case 2:
				editor.ClearArea(m_iPositionX, m_iPositionY, m_iExtendX, m_iExtendY);
				break;
			default:
				break;
			}
		}
	}
}
