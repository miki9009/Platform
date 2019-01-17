using UnityEngine;

public class WorldGraphicSettings : MonoBehaviour
{
    public Material material;
    public Shader lowDetailShader;
    public Color materialColor = Color.white;

    private void Awake()
    {
        QualitySettings.SetQualityLevel(6);
        material.shader = lowDetailShader;
        material.color = materialColor;
    }
}