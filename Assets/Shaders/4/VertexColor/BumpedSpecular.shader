// Bumped Specular using vertex color as tint
//
// (C) Aidem Media 2015
// Author: Szymon Sirocki
//
// _MainTex as a Texture to sample from
// _BumpMap as a Normal Map to sample from
// Shader writes to Z-Buffor, and sets Alpha to 0
// Shader fallbacks to Specular

Shader "VertexColor/BumpedSpecular" {
	Properties {
		_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)		// usage of this var is hidden in BlinnPhong
		_Shininess ("Shininess", Range (0.03, 1)) = 0.078125
		_MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
		_BumpMap ("Normalmap", 2D) = "bump" {}
	}
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 400
		
		CGPROGRAM
		#pragma surface surf BlinnPhong

		uniform sampler2D _MainTex;
		uniform sampler2D _BumpMap;
		uniform half _Shininess;

		struct Input {
			float2 uv_MainTex;
			float2 uv_BumpMap;
			fixed4 color : COLOR;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = tex.rgb * IN.color.rgb;
			o.Gloss = tex.a;
			o.Alpha = 0;
			o.Specular = _Shininess;
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
		}
		ENDCG
	}
	FallBack "Specular"
}
