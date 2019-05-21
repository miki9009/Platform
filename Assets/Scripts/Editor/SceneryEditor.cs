using Engine;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Scenery), true)]
[CanEditMultipleObjects]
public class SceneryEditor : Editor
{
    bool prevCastShadow = true;
    bool recieveShadow = true;
    public override void OnInspectorGUI()
    {
        var scenery = (Scenery)target;
        base.OnInspectorGUI();
        if(GUILayout.Button("Get path"))
        {
            
            Debug.Log(scenery.GetName());
        }
        if(prevCastShadow != scenery.castShadow)
        {
            var renderers = scenery.GetComponentsInChildren<MeshRenderer>();
            foreach (var rend in renderers)
            {
                if(!scenery.castShadow)
                    rend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                else
                    rend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            }
        }
        if(recieveShadow != scenery.recieveShadow)
        {
            var renderers = scenery.GetComponentsInChildren<MeshRenderer>();
            foreach (var rend in renderers)
            {
                    rend.receiveShadows = scenery.recieveShadow;
            }
        }
        prevCastShadow = scenery.castShadow;
        recieveShadow = scenery.recieveShadow;
    }
}