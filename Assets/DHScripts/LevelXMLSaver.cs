using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

[XmlRoot("LevelCollection")]
public class LevelXMLSaver
{
	[XmlArray("Level")]
	public string[] m_arrstrCompressedMap;

	public void Compress(int[,] map)
	{
		string strTemp = "";
		m_arrstrCompressedMap = new string[map.GetLength(0)];
		for (int x = 0; x < map.GetLength(0); ++x)
		{
			for (int y = 0; y < map.GetLength(1); ++y)
			{
				strTemp = strTemp + map[x, y] + ",";
			}
			m_arrstrCompressedMap[x] = strTemp;
			strTemp = "";
		}
	}

	public int[,] Extract()
	{
		string[] strLiterals = new string[m_arrstrCompressedMap[0].Length / 2];
		int[,] arriExtractedMap = new int[m_arrstrCompressedMap.Length, strLiterals.Length];

		for (int i = 0; i < m_arrstrCompressedMap.Length; ++i)
		{
			strLiterals = m_arrstrCompressedMap[i].Split(',');

			// skipping the last literal after the last comma because it's empty
			for (int j = 0; j < strLiterals.Length - 1; ++j)
			{
				arriExtractedMap[i, j] = int.Parse (strLiterals[j]);
			}
		}

		return arriExtractedMap;
	}

	public void Save(string path)
	{
		XmlSerializer serializer = new XmlSerializer(typeof(LevelXMLSaver));
		
		using (FileStream stream = new FileStream(path, FileMode.Create))
		{
			serializer.Serialize(stream, this);
		}
	}

	public static LevelXMLSaver Load(string path)
	{
		XmlSerializer serializer = new XmlSerializer(typeof(LevelXMLSaver));
		using (FileStream stream = new FileStream(path, FileMode.Open))
		{
			return (LevelXMLSaver)serializer.Deserialize(stream);
		}
	}

	public static LevelXMLSaver ReleaseLoad(StringReader xml)
	{
		XmlSerializer serializer = new XmlSerializer(typeof(LevelXMLSaver));

		return serializer.Deserialize(xml) as LevelXMLSaver;
	}
	
	public void PrintStringArray()
	{
		for (int i = 0; i < m_arrstrCompressedMap.Length; ++i)
		{
			Debug.Log (m_arrstrCompressedMap[i]);
		}
	}
}
