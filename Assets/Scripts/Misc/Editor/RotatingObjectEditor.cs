using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RotatingObject))]
public class RotatingObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Place model"))
        {
            var rotatingObject = (RotatingObject)target;
            if(rotatingObject.model)
            {
                DestroyImmediate(rotatingObject.model);
            }


            var go = (GameObject)Instantiate(Resources.Load(rotatingObject.modelID));
            rotatingObject.SetModel(go);

        }
    }
}
