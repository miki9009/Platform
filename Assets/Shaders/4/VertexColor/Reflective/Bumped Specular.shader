// Bumped Reflective Specular using vertex color as tint
//
// (C) Aidem Media 2015
// Author: Szymon Sirocki
//
// Shader writes to Z-Buffor, and sets Alpha to 0
// Shader fallbacks to Reflective/Bumped Diffuse

Shader "VertexColor/Reflective/BumpedSpecular" 
{
	Properties {
		_SpecColor ("Specular Color", Color) = (0.5,0.5,0.5,1)
		_Shininess ("Shininess", Range (0.01, 1)) = 0.078125
		_ReflectColor ("Reflection Color", Color) = (1,1,1,0.5)
		_MainTex ("Base (RGB) RefStrGloss (A)", 2D) = "white" {}
		_Cube ("Reflection Cubemap", Cube) = "" { TexGen CubeReflect }
		_BumpMap ("Normalmap", 2D) = "bump" {}
	}

	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 400
		CGPROGRAM
		#pragma surface surf BlinnPhong
		#pragma target 3.0
		//input limit (8) exceeded, shader uses 9
		#pragma exclude_renderers d3d11_9x

		uniform sampler2D _MainTex;
		uniform sampler2D _BumpMap;
		uniform samplerCUBE _Cube;

		uniform fixed4 _ReflectColor;
		uniform half _Shininess;

		struct Input {
			float2 uv_MainTex;
			float3 worldRefl;
			fixed4 color : COLOR;
			INTERNAL_DATA
		};

		void surf (Input IN, inout SurfaceOutput o) {
			fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
			fixed4 c = tex * IN.color;
			o.Albedo = c.rgb;
			
			o.Gloss = tex.a;
			o.Specular = _Shininess;
			
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
			
			float3 worldRefl = WorldReflectionVector (IN, o.Normal);
			fixed4 reflcol = texCUBE (_Cube, worldRefl);
			reflcol *= tex.a;
			o.Emission = reflcol.rgb * _ReflectColor.rgb;
			o.Alpha = 0;
		}
		ENDCG
	}
	FallBack "Reflective/Bumped Diffuse"
}