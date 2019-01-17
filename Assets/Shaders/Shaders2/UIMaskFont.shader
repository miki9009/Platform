// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Eldirfar/UIMaskFont"
{
	//Screen range for 1 (top) to -1 (bottom), 0 at middle
	//x - top start blur
	//y - top blur range
	//z - bottom start blur (should be negative)
	//w - bottom blur range (should be negative)

    Properties
    {
    	[PerRendererData]_MainTex ("Base (RGB)", 2D) = "white" { }
    	_Edge ("Blur", Vector) = (0.6, 1, -0.6, -1)

    	[HideInInspector]_StencilComp ("Stencil Comparison", Float) = 8
    	[HideInInspector]_Stencil ("Stencil ID", Float) = 0
    	[HideInInspector]_StencilOp ("Stencil Operation", Float) = 0
    	[HideInInspector]_StencilWriteMask ("Stencil Write Mask", Float) = 255
    	[HideInInspector]_StencilReadMask ("Stencil Read Mask", Float) = 255
    	[HideInInspector]_ColorMask ("Color Mask", Float) = 15
    }
    SubShader
    {
        Tags{
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
        }
        Cull Off
        Lighting Off
        ZWrite Off
        Fog{ Mode Off }
        Blend SrcAlpha OneMinusSrcAlpha

        Stencil
		{
			Ref [_Stencil]
			Comp [_StencilComp]
			Pass [_StencilOp] 
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
		}

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _MainTex;
            half4 _Edge;

            struct appdata
            {
                float4 vertex : POSITION;
                half4 color : COLOR;
                half4 uv : TEXCOORD;
            };
            	
            struct v2f
            {
                float4 vertex : SV_POSITION;
                half4 uv : TEXCOORD;
                float4 position : COLOR1;
                half4 color : COLOR;
            };

            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);  
                o.position = o.vertex;
                o.color = v.color;
                o.uv = v.uv;
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
            	half4 result = i.color;
            	result.a = tex2D (_MainTex, i.uv).a;
            	half a = result.a * i.color.a;

            	result.a = 1 - lerp(0, 1, (i.position.y - _Edge.x) / _Edge.y);
            	result.a = min(result.a, 1 - lerp(0, 1, (i.position.y - _Edge.z) / _Edge.w));
            	result.a = min(result.a, a);

            	//if (i.position.y >= _Edge.x && i.position.y <= _Edge.x + _Edge.y)
            	//{
            	//	result.x = 0;
            	//	result.y = 0;
            	//	result.z = 0;
        		//}

	         	return result;
         	}

            ENDCG
        }
    }
}