// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/VertexColor_WithoutLight"
{
    Properties
    {
    	_Alpha ("Alpha", Range(0, 1)) = 1
    }
    SubShader
    {
        Tags{
            "Queue" = "Geometry"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
        }
        Lighting Off
        Fog{ Mode Off }
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            float _Alpha;

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);  
                o.color = v.color;
                return o;
            }
             
	         float4 frag (v2f i) : SV_Target
	         {
             	i.color.a = _Alpha;
             	return i.color;
         	}

            ENDCG
        }
    }
}