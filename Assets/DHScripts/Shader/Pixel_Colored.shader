Shader "Custom/Pixel_Colored" {
	Properties
	{
		_NoiseMap("Noise Map", 2D) = "white" { }
		_Intensity("Intensifier", float) = 3.0
		_AlphaAdd("Fill Up Alpha", float) = 0.5
		_DisplayTreshold("Treshold Fog", float) = 1.5
		// length of the part where the transition from fog to no fog
		_TransitionLength("Transition Length", float) = 0.1
	}
	SubShader
	{
		//ZWrite Off
		Tags { "Queue"="Transparent+1" }
		LOD 200
		Blend SrcAlpha OneMinusSrcAlpha
		//Blend OneMinusDstColor One
		Cull Off

		CGPROGRAM
		#pragma surface surf Lambert
		
		sampler2D _NoiseMap;
		float _Intensity;
		float _AlphaAdd;
		float _DisplayTreshold;
		float _TransitionLength;
		
		struct Input {
			float2 uv_NoiseMap;
			float4 color : COLOR;
		};
		
		void surf (Input IN, inout SurfaceOutput o)
		{
			float2 uvCoords = IN.uv_NoiseMap;
		
			half3 col = tex2D(_NoiseMap, uvCoords).rgb;
			
			float alpha = IN.color.a * col.r * _Intensity + IN.color.a * _AlphaAdd;
			float transitionAdd = (_TransitionLength - clamp(uvCoords.y - _DisplayTreshold, 0.0, _TransitionLength)) / _TransitionLength;
			alpha = alpha * (step(uvCoords.y, _DisplayTreshold) + transitionAdd);
			
		 	o.Alpha = alpha; 
		}
		ENDCG
	} 
	FallBack "Transparent/VertexLit"
}
