// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//Sebastian Zalewski
//Unlit transparent with color tint

Shader "Unlit/Transparent Color Alpha"
{
	Properties
	{
		_Color ("Main Color", Color) = (1,1,1,1)
		_Alpha ("Alpha", Range(0,1)) = 1
	}
	SubShader
	{
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}

        LOD 100
		ZWrite Off
	    Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata_t
            {
				fixed4 vertex : POSITION;
			};

			struct v2f
            {
				fixed4 vertex : SV_POSITION;
			};

			fixed4 _Color;
			fixed _Alpha;

			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
			    fixed4 color = _Color;
			    color.a *= _Alpha;
				return color;
			}
			ENDCG
		}
	}
}
