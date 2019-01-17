using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;

[Serializable]
[CustomEditor(typeof(EditorUtilities))]
public class EditorUtilitiesInspector : Editor
{
    public int index = 0;

    bool randomRotation = false;
    bool useX = true;
    bool useY = true;
    bool useZ = true;

    bool randomScale = false;

    float min = 1;

    float max = 1;


    private Vector3 rotationValue;
    private Vector3 scaleValue;
    EditorUtilities script;

    public override void OnInspectorGUI()
    {
        script = (EditorUtilities)target;

        script.randomRotation = EditorGUILayout.Toggle("Use Random Rotation",script.randomRotation);
        useX = EditorGUILayout.Toggle("Use X axis", useX);
        useY = EditorGUILayout.Toggle("Use Y axis", useY);
        useZ = EditorGUILayout.Toggle("Use Z axis", useZ);

        if (!script.randomRotation)
        {
            rotationValue = EditorGUILayout.Vector3Field("Use rotation", rotationValue);
        }

        index = EditorGUILayout.Popup(index, UnityEditorInternal.InternalEditorUtility.tags);
        if (GUILayout.Button("Rotate"))
        {
            if (!RotateObjects())
            {
                Debug.LogError("Cannot rotate objects of tag: Untagged");
            }
        }

        GUILayout.Label("SCALE");

        randomScale = EditorGUILayout.Toggle("Use Random Scale", randomScale);

        if (!randomScale)
        {
            scaleValue = EditorGUILayout.Vector3Field("Use scale", scaleValue);
        }

        min = EditorGUILayout.FloatField("Min scale Factor",min);

        max = EditorGUILayout.FloatField("Max scale Factor", max);


        if (GUILayout.Button("Scale"))
        {
            if (!ScaleObjects())
            {
                Debug.LogError("Cannot scale objects of tag: Untagged");
            }
        }

        if (GUILayout.Button("Get All Materials"))
        {
            if (!ScaleObjects())
            {
                script.GetAllActiveMaterials();
            }
        }


    }

    bool RotateObjects()
    {
        string tag = UnityEditorInternal.InternalEditorUtility.tags[index];
        if (tag == "Untagged")
        {
            return false;
        }
        var objects = GameObject.FindGameObjectsWithTag(tag);
        foreach (GameObject obj in objects)
        {
            obj.transform.rotation = Quaternion.Euler((script.randomRotation ? new Vector3(useX ? UnityEngine.Random.Range(0, 359) : obj.transform.eulerAngles.x, useY ? UnityEngine.Random.Range(0, 359) : obj.transform.eulerAngles.y, useZ ? UnityEngine.Random.Range(0, 359) : obj.transform.eulerAngles.z) : rotationValue));
        }
        return true;
    }

    bool ScaleObjects()
    {
        string tag = UnityEditorInternal.InternalEditorUtility.tags[index];
        if (tag == "Untagged")
        {
            return false;
        }
        var objects = GameObject.FindGameObjectsWithTag(tag);
        foreach (GameObject obj in objects)
        {
            if (randomScale)
            {
                var scale = UnityEngine.Random.Range(min, max);
                obj.transform.localScale = new Vector3(scale,scale,scale);
            }
            else
            {
                obj.transform.localScale = scaleValue;
            }
        }
        return true;
    }
}