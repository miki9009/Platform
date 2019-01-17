Shader "VertexColor/VertexStandard" {
	Properties{
	_Color("Tint", Color) = (1,1,1,1)
		_Glossiness("Smoothness", Range(0,1)) = 0.5
	}
		SubShader{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+1" }
		LOD 200

		CGPROGRAM

#pragma surface surf Standard addshadow

		struct Input {
		float3 color : COLOR;
	};

	fixed4 _Color;

	half _Glossiness;

	void surf(Input IN, inout SurfaceOutputStandard o) {
		o.Albedo = IN.color * _Color;
		o.Smoothness = _Glossiness;
	}
	ENDCG
	}
}
