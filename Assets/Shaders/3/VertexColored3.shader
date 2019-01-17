Shader "MyShader/VertexColored2" {
    Properties {
        _Color ("Main Color", Color) = (0.5,0.5,0.5,1)
    }

    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200
        
CGPROGRAM
#pragma surface surf None

float4 _Color;

struct Input {
    float4 color : COLOR;
};

half4 LightingNone (SurfaceOutput s, half3 lightDir, half atten) {
    half4 c;
    c.rgb = s.Albedo;
    c.a = s.Alpha;
    return c;
}

void surf (Input IN, inout SurfaceOutput o) {
    half4 c = _Color * IN.color;
    o.Albedo = c.rgb;
    o.Alpha = c.a;
}
ENDCG
    } 

    Fallback "Diffuse"
}