using Engine;
using UnityEngine;

public class LevelVisualSettings : LevelElement
{
    public UnityEngine.Color color;
    public Material material;
    public Material standardVertex;
    public bool rotateSkybox;
    public float rotationSpeed = 5;
    Material skybox;

    [Range(0,1.5f)]
    public float bloomThreshold = 0.6f;
    [Range(0, 1)]
    public float materialSmoothness = 0.7f; 

    public override void OnSave()
    {
        base.OnSave();
        Engine.Colour col = color;
        data.Add("Color", col);
        data.Add("BloomThreshold", bloomThreshold);
        data.Add("Smoothness", materialSmoothness);
        data.Add("UseSkyboxRotation", rotateSkybox);
    }

    public override void OnLoad()
    {
        base.OnLoad();
        if(data.ContainsKey("Color"))
        {
            color = (Engine.Colour)data["Color"];
            material.color = color;
        }
        if(data.ContainsKey("BloomThreshold"))
        {
            bloomThreshold = (float)data["BloomThreshold"];
            if(Controller.Instance!=null)
                Controller.Instance.bloom.threshold = bloomThreshold;
        }
        if(data.ContainsKey("UseSkyboxRotation"))
        {
            rotateSkybox = (bool)data["UseSkyboxRotation"];
            skybox = RenderSettings.skybox;
        }
        if(data.ContainsKey("Smoothness"))
        {
            try
            {
                materialSmoothness = (float)data["Smoothness"];
                material.SetFloat("_Glossiness", materialSmoothness);
            }
            catch { }

        }
    }

    //float rotation = 1;
    //private void Update()
    //{
    //    if(rotateSkybox)
    //    {
    //        if(rotation < 360)
    //        {
    //            rotation += Time.deltaTime * rotationSpeed;
    //        }
    //        else
    //        {
    //            rotation = 0;
    //        }

    //        skybox.SetFloat("_Rotation", rotation);
    //    }
    //}


}