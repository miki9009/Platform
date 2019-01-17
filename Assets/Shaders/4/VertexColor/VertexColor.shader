    Shader "VertexColor/Unlit" 
	{
		Properties{

		

		}

		Category 
		{
			BindChannels 
			{
				Bind "Color", color
				Bind "Vertex", vertex
			}
			SubShader 
			{ 
				Pass { }
			}
		}
	}