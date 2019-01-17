// Bumped Diffuse using vertex color as tint
//
// (C) Aidem Media 2015
// Author: Szymon Sirocki
//
// _MainTex as a Texture to sample from
// _BumpMap as a Normal Map to sample from
// Shader writes to Z-Buffor, and sets Alpha to 0
// Shader fallbacks to Diffuse

Shader "VertexColor/BumpedDiffuse" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_BumpMap ("Normalmap", 2D) = "bump" {}
}

	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 300

		CGPROGRAM
		#pragma surface surf Lambert

		uniform sampler2D _MainTex;
		uniform sampler2D _BumpMap;
		
		struct Input {
			float2 uv_MainTex; 
			fixed4 color : COLOR;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * IN.color;
			o.Albedo = c.rgb;
			o.Alpha = 0;
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
		}
		ENDCG  
	}

	FallBack "Diffuse"
}