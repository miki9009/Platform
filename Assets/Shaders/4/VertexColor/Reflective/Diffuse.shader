// Reflective Diffuse using vertex color as tint
//
// (C) Aidem Media 2015
// Author: Szymon Sirocki
//
// Shader writes to Z-Buffor, and sets Alpha to 0
// Shader fallbacks to Reflective/VertexLit

Shader "VertexColor/Reflective/Diffuse" 
{
	Properties {
		_ReflectColor ("Reflection Color", Color) = (1,1,1,0.5)
		_MainTex ("Base (RGB) RefStrength (A)", 2D) = "white" {} 
		_Cube ("Reflection Cubemap", Cube) = "_Skybox" { TexGen CubeReflect }
	}
	SubShader {
		LOD 200
		Tags { "RenderType"="Opaque" }
		
		CGPROGRAM
		#pragma surface surf Lambert

		uniform sampler2D _MainTex;
		uniform samplerCUBE _Cube;

		uniform fixed4 _ReflectColor;

		struct Input {
			float2 uv_MainTex;
			float3 worldRefl;
			fixed4 color : COLOR;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
			fixed4 c = tex * IN.color;
			o.Albedo = c.rgb;
			
			fixed4 reflcol = texCUBE (_Cube, IN.worldRefl);
			reflcol *= tex.a;
			o.Emission = reflcol.rgb * _ReflectColor.rgb;
			o.Alpha = 0;
		}
		ENDCG
	}
		
	FallBack "Reflective/VertexLit"
} 