using Engine;
using UnityEngine;

public class LevelVisualSettings : LevelElement
{


    public bool enableBloom = true;
    Material skybox;
    public bool changeLightSettings = false;

    Vector3 lightRotation;

    [Range(0,1.5f)]
    public float bloomThreshold = 0.6f;
    //[Range(0, 1)]
    //public float materialSmoothness = 0.7f; 

    public override void OnSave()
    {
        base.OnSave();
        //data.Add("Color", col);
        data.Add("BloomThreshold", bloomThreshold);
        data.Add("EnableBloom", enableBloom);
        data.Add("ChangeLightSettings", changeLightSettings);
        if (SceneLight.CurrentLight)
        {
            data["LightRotation"] = (Float3)SceneLight.CurrentLight.transform.eulerAngles;
        }
    }

    public override void OnLoad()
    {
        base.OnLoad();
        if(data.ContainsKey("BloomThreshold"))
        {
            bloomThreshold = (float)data["BloomThreshold"];
            if(Controller.Instance!=null)
                Controller.Instance.bloom.threshold = bloomThreshold;
        }
        if (data.ContainsKey("EnableBloom"))
        {
            enableBloom = (bool)data["EnableBloom"];
            if(Application.isPlaying && !enableBloom)
            {
                Controller.Instance.bloom.enabled = false;
            }
        }
        if(data.ContainsKey("ChangeLightSettings") && data.ContainsKey("LightRotation"))
        {
            if(SceneLight.CurrentLight)
            {
                SceneLight.CurrentLight.transform.rotation = Quaternion.Euler((Float3)data["LightRotation"]);
            }
        }
    }


}