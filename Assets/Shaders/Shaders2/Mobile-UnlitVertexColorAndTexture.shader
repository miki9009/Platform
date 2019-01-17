// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/VertexColorAndTexture_WithoutLight"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" { }
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

            #include "UnityCG.cginc"

            float _Alpha;
            sampler2D _MainTex;
            float4 _MainTex_ST;

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                half2 uv : TEXCOORD;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
                half2 uv : TEXCOORD;
            };

            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);  
                o.color = v.color;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                return o;
            }
             
	         float4 frag (v2f i) : SV_Target
	         {
	            half4 result = tex2D (_MainTex, i.uv) * i.color;
             	result.a *= _Alpha;
             	return result;
         	}

            ENDCG
        }
    }
}