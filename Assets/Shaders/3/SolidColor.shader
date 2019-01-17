Shader "MyShader/SolidColor" {
	
	Properties 
	{
		_Color ("Solid Color (A-Blend)", Color) = (1,0,0)
		//_Number ("Number", Range(0,1)) = 1
	}
	
	SubShader 
	{
		//Color [_Color]
		BindChannels 
			{
				Bind "Color", color
				Bind "Vertex", vertex
			}


		Pass
		{
			SetTexture[_]
			{
				ConstantColor  [_Color]
				Combine constant Lerp(constant) primary
			}
		
		
		
		}		
	} 

}
