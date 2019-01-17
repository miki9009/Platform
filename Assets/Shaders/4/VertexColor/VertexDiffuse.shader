Shader "VertexColor/Diffuse" {
Properties {
    _Color ("Main Color", Color) = (1,1,1,1)
}
 
SubShader {

LOD 200

CGPROGRAM
#pragma surface surf Lambert addshadow
fixed4 _Color;
 
struct Input {
    float3 vertColor : COLOR;
};
 

void surf (Input IN, inout SurfaceOutput o) {
    o.Albedo = IN.vertColor*_Color;
}
ENDCG
}
}
