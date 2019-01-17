Shader "VertexColor/Transparent/Diffuse" {
Properties {
//    _Color ("Main Color", Color) = (1,1,1,1)
//    _MainTex ("Base (RGB)", 2D) = "white" {}
    _Opacity ("Alpha", Range(0,1)) = 0.8
}
 
SubShader {
    Tags {"RenderType"="Transparent" "Queue"="Transparent -10"}
    LOD 150
 
CGPROGRAM
#pragma surface surf Lambert vertex:vert alpha:fade
 
//sampler2D _MainTex;
//fixed4 _Color;
float _Opacity;
 
struct Input {
//    float2 uv_MainTex;
    float3 vertColor;
};
 
void vert (inout appdata_full v, out Input o) {
//    UNITY_INITIALIZE_OUTPUT(Input, o);
   o.vertColor = v.color;
}
 
void surf (Input IN, inout SurfaceOutput o) {
//    fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
//    o.Albedo = c.rgb * IN.vertColor;
    o.Albedo = IN.vertColor;
    o.Alpha = _Opacity;
//    o.Alpha = c.a;
}
ENDCG
}
 
FallBack "Diffuse"
}