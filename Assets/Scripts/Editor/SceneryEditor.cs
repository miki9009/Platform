using Engine;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Scenery), true)]
[CanEditMultipleObjects]
public class SceneryEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(GUILayout.Button("Get path"))
        {
            var scenery = (Scenery)target;
            Debug.Log(scenery.GetName());
        }
    }
}