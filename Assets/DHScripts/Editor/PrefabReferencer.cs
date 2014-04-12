using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

public struct KeyInfo
{
	[XmlAttribute("Key")]
	public string m_cKey;
	[XmlAttribute("ObjectPath")]
	public string m_strObjectPath;
	[XmlAttribute("PrefabSize")]
	public string m_strPrefabSize;
	[XmlAttribute("Index")]
	public int m_iIndex;
	[XmlAttribute("Color")]
	public string m_strColor;
}
public struct PrefabProfile
{
	public GameObject m_goPrefab;
	public int m_iSizeX;
	public int m_iSizeY;
	public char m_cKey;
	public int m_iTileID;
	public Color m_Color;
}

[XmlRoot("PrefabReference")]
public class PrefabReferencer
{
	private Dictionary<int, PrefabProfile> m_dicReferences;

	[XmlArray("Info")]
	public KeyInfo[] m_arrstrInfos;

	public void Save(string path)
	{
		XmlSerializer serializer = new XmlSerializer(typeof(PrefabReferencer));
		
		using (FileStream stream = new FileStream(path, FileMode.Create))
		{
			serializer.Serialize(stream, this);
		}
	}
	
	public static PrefabReferencer Load(string path)
	{
		XmlSerializer serializer = new XmlSerializer(typeof(PrefabReferencer));
		using (FileStream stream = new FileStream(path, FileMode.Open))
		{
			return (PrefabReferencer)serializer.Deserialize(stream);
		}
	}

	public void Extract()
	{
		// first entry is not used because 0 is preserved for empty fields on the map grid
		//m_dicKeyControls = new char[m_arrstrInfos.Length + 1];
		m_dicReferences = new Dictionary<int, PrefabProfile>();
		// therefore starting with one and going one entry further
		for (int i = 0; i < m_arrstrInfos.Length; ++i)
		{
			// compensate bei subtracting 1

			string[] tempString = new string[2];
			tempString = m_arrstrInfos[i].m_strPrefabSize.Split(',');

			PrefabProfile tempProfile;
			tempProfile.m_goPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(m_arrstrInfos[i].m_strObjectPath, typeof(GameObject));
			tempProfile.m_iSizeX = int.Parse(tempString[0]);
			tempProfile.m_iSizeY = int.Parse(tempString[1]);
			tempProfile.m_cKey = m_arrstrInfos[i].m_cKey.ToLower().ToCharArray()[0];
			tempProfile.m_iTileID = m_arrstrInfos[i].m_iIndex;

			tempString = new string[4];

			tempString = m_arrstrInfos[i].m_strColor.Split(',');

			tempProfile.m_Color = new Color(byte.Parse (tempString[0]), byte.Parse (tempString[1]), byte.Parse (tempString[2]), byte.Parse (tempString[3]));

			m_dicReferences.Add(i, tempProfile);
		}
	}

	public GameObject GetObjectByValue(int iVal)
	{
		for (int i = 0; i < m_dicReferences.Count; ++i)
		{
			if (m_dicReferences[i].m_iTileID == iVal)
				return m_dicReferences[i].m_goPrefab;
		}
		return null;
	}

	public short GetObjectByChar(char cKey, out GameObject goObj)
	{
		// first entry is not used because 0 is preserved for empty fields on the map grid
		for (short i = 0; i < m_dicReferences.Count; ++i)
		{
			if (m_dicReferences[i].m_cKey == cKey)
			{
				PrefabProfile Output;
				if (m_dicReferences.TryGetValue(i, out Output))
				{
					goObj = Output.m_goPrefab;
					return (short)Output.m_iTileID;
				}
			}
		}

		goObj = null;
		return 0;
	}

	public PrefabProfile GetProfileByValue(int iVal)
	{
		PrefabProfile temp = new PrefabProfile();
		for (int i = 0; i < m_dicReferences.Count; ++i)
		{
			if (m_dicReferences[i].m_iTileID == iVal)
				return m_dicReferences[i];
		}
		return temp;
	}

	public short GetProfileByChar(char cKey, out PrefabProfile profile)
	{
		profile = new PrefabProfile();
		for (short i = 0; i < m_dicReferences.Count; ++i)
		{
			if (m_dicReferences[i].m_cKey == cKey)
			{
				PrefabProfile Output;
				if (m_dicReferences.TryGetValue(i, out Output))
				{
					profile = Output;
					return (short)profile.m_iTileID;
				}
			}
		}
		
		return 0;
	}

	public bool ContainsKey(int a)
	{
		for (int i = 0; i < m_dicReferences.Count; ++i)
		{
			if (m_dicReferences[i].m_iTileID == a)
				return true;
		}
		return false;
	}

	public int ContainsGameobject(GameObject goObj)
	{
		if (goObj != null)
		{
			for (int i = 0; i < m_dicReferences.Count; ++i)
			{
				if (goObj.name.Equals(m_dicReferences[i].m_goPrefab.name))
				{
					return m_dicReferences[i].m_iTileID;
				}
			}
		}
		return -1;
	}

	public GameObject GetGameobjectByColor(Color c)
	{
		for (int i = 0; i < m_dicReferences.Count; ++i)
		{
			if (m_dicReferences[i].m_Color == c)
			{
				return m_dicReferences[i].m_goPrefab;
			}
		}

		return null;
	}

	public int GetValueByColor(Color c)
	{
		for (int i = 0; i < m_dicReferences.Count; ++i)
		{
			if (m_dicReferences[i].m_Color == c)
			{
				return m_dicReferences[i].m_iTileID;
			}
		}
		
		return -1;
	}
}
