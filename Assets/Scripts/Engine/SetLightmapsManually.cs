using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class SetLightmapsManually : MonoBehaviour
{
	public int m_lightmapIndex = 255;
	public float m_lightmapTilingX = 1f;
	public float m_lightmapTilingY = 1f;
	public float m_lightmapOffsetX = 0f;
	public float m_lightmapOffsetY = 0f;
	public bool m_getLightmapValues = false;
	public bool m_setLightmapValues = false;
	public Texture2D[] m_lightmapArray;
	public bool m_setLightmapArray = false;

	void Update ()
	{
		if (m_setLightmapValues)
		{
			m_setLightmapValues = false;

			Renderer r;

			if (r = GetComponent<Renderer>())
			{
				r.lightmapIndex = m_lightmapIndex;
				r.lightmapScaleOffset = new Vector4(m_lightmapTilingX, m_lightmapTilingY, m_lightmapOffsetX, m_lightmapOffsetY);
			}
		}

		if (m_getLightmapValues)
		{
			m_getLightmapValues = false;

			Renderer r;
			
			if (r = GetComponent<Renderer>())
			{
				m_lightmapIndex = r.lightmapIndex;
				m_lightmapTilingX = r.lightmapScaleOffset.x;
				m_lightmapTilingY = r.lightmapScaleOffset.y;
				m_lightmapOffsetX = r.lightmapScaleOffset.z;
				m_lightmapOffsetY = r.lightmapScaleOffset.w;
			}
		}

		if (m_setLightmapArray)
		{
			m_setLightmapArray = false;

			if (m_lightmapArray.Length > 0)
			{
				LightmapData[] lightmapData = new LightmapData[m_lightmapArray.Length];

				for (int i=0; i<lightmapData.Length; i++)
				{
					lightmapData[i] = new LightmapData();
					lightmapData[i].lightmapColor = m_lightmapArray[i];
				}

				LightmapSettings.lightmaps = lightmapData;
			}
		}
	}
}
