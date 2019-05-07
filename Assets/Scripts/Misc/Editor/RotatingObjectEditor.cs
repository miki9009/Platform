using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Engine;

[CustomEditor(typeof(RotatingObject))]
public class RotatingObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        //var script = (RotatingObject)target;
        //EditorGUILayout.BeginHorizontal();
        //EditorGUILayout.LabelField("Rotation Model: ", GUILayout.Width(100));

        //GameObject prevModel = script.model;
        //script.model = (GameObject)EditorGUILayout.ObjectField(script.model, typeof(GameObject), false);
        //if (script.model != prevModel)
        //{
        //    if (prevModel)
        //        DestroyImmediate(prevModel);
        //    if(script.model)
        //    {
        //        var model = (GameObject)PrefabUtility.InstantiatePrefab(script.model);
        //        var scenery = model.GetComponent<Scenery>();
        //        if (scenery)
        //        {
        //            script.modelID = scenery.GetName();
        //            DestroyImmediate(scenery);
        //        }
        //        else
        //        {
        //            Debug.LogError("Model needs to have scenery component");
        //        }
        //        script.SetModel(model);
        //    }

        //}
        //EditorGUILayout.EndHorizontal();
    }
}
