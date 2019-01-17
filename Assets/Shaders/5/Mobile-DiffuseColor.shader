Shader "Mobile/Diffuse Color"
{
    Properties
    {
		_Color("Color",COLOR) = (1,1,1,1)
    }
     
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 150
        CGPROGRAM
        #pragma surface surf Lambert noforwardadd
     
        fixed4 _Color;
     
        struct Input
        {
			bool fill;
        };
     
        void surf (Input IN, inout SurfaceOutput o)
        {
            o.Albedo = _Color.rgb;
            o.Alpha = _Color.a;
        }
        ENDCG
    }
    Fallback "Mobile/VertexLit"
}
