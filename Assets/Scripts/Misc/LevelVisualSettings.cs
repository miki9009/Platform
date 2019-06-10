using Engine;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class LevelVisualSettings : LevelElement
{
#if UNITY_EDITOR
    public bool editorEnabled = true;
#endif


    Material skybox;

    [Header("BLUR")]
    public bool enableBloom = true;
    [Range(0,1.5f)]
    public float bloomThreshold = 0.6f;
    [Range(0, 2.5f)]
    public float intensity = 2;
    [Range(0, 5f)]
    public float blurSize = 3;
    [Range(0,4)]
    public int blurIterations = 1;
    public bool useGlobalFog;

    [Header("LIGHTNING")]
    public Vector3 lightRotation;
    [Range(0,5f)]
    public float lightIntensity;
    public Color lightColor;

    //[Range(0, 1)]
    //public float materialSmoothness = 0.7f; 

    public override void OnSave()
    {
        base.OnSave();
        //data.Add("Color", col);
        data.Add("BloomThreshold", bloomThreshold);
        data.Add("EnableBloom", enableBloom);
        data["BloomIntensity"] = intensity;
        data["BlurSize"] = blurSize;
        data["BlurIterations"] = blurIterations;
        data["GlobalFog"] = useGlobalFog;
        if (SceneLight.CurrentLight)
        {
            data["LightRotation"] = (Float3)SceneLight.CurrentLight.transform.eulerAngles;
            data["LightIntensity"] = SceneLight.CurrentLight.intensity;
            data["LightColor"] = (Engine.Colour)SceneLight.CurrentLight.color;
        }
        else
        {
            Debug.LogError("No Light found");
        }
    }

    public override void OnLoad()
    {
        base.OnLoad();
        if(data.ContainsKey("BloomThreshold"))
        {

            if(Controller.Instance!=null)
            {
                if(data.ContainsKey("BloomThreshold"))
                {
                    bloomThreshold = (float)data["BloomThreshold"];
                    Controller.Instance.bloom.threshold = bloomThreshold;
                }
                if (data.ContainsKey("BloomIntensity"))
                {
                    intensity = (float)data["BloomIntensity"];
                    Controller.Instance.bloom.intensity = intensity;
                }
                if (data.ContainsKey("BlurIterations"))
                {
                    blurIterations = (int)data["BlurIterations"];
                    Controller.Instance.bloom.blurIterations = blurIterations;
                }
                if (data.ContainsKey("BlurSize"))
                {
                    blurSize = (float)data["BlurSize"];
                    Controller.Instance.bloom.blurSize = blurSize;
                }

                if (data.ContainsKey("GlobalFog"))
                {
                    useGlobalFog = (bool)data["GlobalFog"];
                    Controller.Instance.UseGlobalFog = useGlobalFog;
                }


            }

        }
        if (data.ContainsKey("EnableBloom"))
        {
            enableBloom = (bool)data["EnableBloom"];
            if(Application.isPlaying && !enableBloom)
            {
                Controller.Instance.bloom.enabled = false;
            }
        }
        if(data.ContainsKey("LightRotation") && data.ContainsKey("LightIntensity"))
        {
            if(SceneLight.CurrentLight)
            {
                SceneLight.CurrentLight.transform.rotation = Quaternion.Euler((Float3)data["LightRotation"]);
                SceneLight.CurrentLight.intensity = (float)data["LightIntensity"];
                if (data.ContainsKey("LightColor"))
                    SceneLight.CurrentLight.color = (Engine.Colour)data["LightColor"];
            }
            else
            {
                Debug.LogError("No light found");
            }
        }
    }

}
#if UNITY_EDITOR
[CustomEditor(typeof(LevelVisualSettings))]
public class LevelVisualSettingsEditor : Editor
{


    public override void OnInspectorGUI()
    {

        var script = (LevelVisualSettings)target;
        if (!script.editorEnabled)
        {
            GUI.backgroundColor = Color.red;
            base.OnInspectorGUI();

            return;
        }
        if (!SceneLight.CurrentLight)
        {
            Debug.Log("No light detected");
            return;
        }
        script.lightRotation = SceneLight.CurrentLight.transform.rotation.eulerAngles;
        script.lightIntensity = SceneLight.CurrentLight.intensity;
        script.lightColor = SceneLight.CurrentLight.color;
        if(Controller.Instance)
        {
            script.intensity = Controller.Instance.bloom.intensity;
            script.blurSize = Controller.Instance.bloom.blurSize;
            script.bloomThreshold = Controller.Instance.bloom.threshold;
            script.blurIterations = Controller.Instance.bloom.blurIterations;
        }

        base.OnInspectorGUI();
        SceneLight.CurrentLight.transform.rotation = Quaternion.Euler(script.lightRotation);
        SceneLight.CurrentLight.intensity = script.lightIntensity;
        SceneLight.CurrentLight.color = script.lightColor;
        if (Controller.Instance)
        {
            Controller.Instance.bloom.intensity = script.intensity;
            Controller.Instance.bloom.blurSize = script.blurSize;
            Controller.Instance.bloom.threshold = script.bloomThreshold;
            Controller.Instance.bloom.blurIterations = script.blurIterations;
        }
    }
}
#endif