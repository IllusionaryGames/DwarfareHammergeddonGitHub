Shader "Custom/Vertex Colored"
{
	Properties
	{
	}
	
	SubShader
	{
		ZWrite Off
		Alphatest Greater 0
		Tags {Queue=Transparent}
		Blend SrcAlpha OneMinusSrcAlpha
		
		Pass
		{
			ColorMaterial AmbientAndDiffuse
		}
	}
}
