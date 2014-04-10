using UnityEngine;
using UnityEditor;
using System.Collections;

public class ELoadTexture
{
	public int[,] LoadLevelFromTexture(string path, PrefabReferencer preref)
	{	
		Texture2D tex2DLevel = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/DHLevelDesign/TextureMaps/" + path, typeof(Texture2D));	

		int[,] arrTemp = new int[tex2DLevel.width, tex2DLevel.height];

		int LevelWidth = tex2DLevel.width;
		int LevelHeight = tex2DLevel.height;
		
		int iPixelIter = 0;
		Color[] pix = tex2DLevel.GetPixels (0, 0, LevelWidth , LevelHeight);

		for (int m = 0; m < LevelWidth; m++) 
		{
			for (int n = 0; n < LevelHeight; n++) 
			{
				Color col255 = new Color(pix[iPixelIter].r * 255, pix[iPixelIter].g * 255, pix[iPixelIter].b * 255, pix[iPixelIter].a * 255);

				int iID = preref.GetValueByColor(col255);

				if (iID > -1)
				{
					arrTemp[n, m] = iID;
				}
				iPixelIter++;
			}
		}

		return arrTemp;
	}
}
