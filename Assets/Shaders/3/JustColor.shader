// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "MyShader/JustColor" 
{
	Properties 
	{
		_Color ("Color" , Color) = (1,1,1,1)
	}

	SubShader 
	{
		//Tags{"Queue" = "Geometry"}
		//Blend SrcAlpha OneMinusSrcAlpha 
		ZWrite Off
	
		pass
		{
			CGPROGRAM 
			// Upgrade NOTE: excluded shader from OpenGL ES 2.0 because it does not contain a surface program or both vertex and fragment programs.
			#pragma exclude_renderers gles
			#pragma vertex vert 
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			
			
			uniform fixed4 _Color;
			
			struct vertexInput
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tangent: TANGENT; 
				float4 texcoord : TEXCOORD0; 
				float4 texcoord1 : TEXCOORD1; 
				
				fixed4 color : COLOR;
			};
			
			struct fragmentInput
			{
				float4 pos : SV_POSITION; 
				float4 color : COLOR0;
			};
			
			fragmentInput vert(vertexInput i){
				
				fragmentInput o; 
				o.pos = UnityObjectToClipPos(i.vertex); 
				//o.color = i.texcoord1 + float4(0.5, 0.5, 0.5, 0.0);  //ladny efekt w chuj 
				o.color = i.color;
				//o.color = float4(i.normal *0.5f + 0.5f  ,0.5f);
				
				return o;
			
			}
			
			half4 frag(fragmentInput i) : COLOR 
			{
				return i.color;
			}
			
			ENDCG	
		}
	} 
	
	FallBack "Diffuse"
}
