using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(Rotate))]
public class RotateEditor : Editor
{
    Rotate script;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        script = (Rotate)target;
        if (script.points == null)
        {
            script.points.points = new List<Vector3>();
        }
        EditorGUILayout.LabelField("Distance");
        if (GUILayout.Button("Create Circle"))
        {
            CreateCircle(script);
        }
    }

    void CreateCircle(Rotate script)
    {
        script.points.points = new List<Vector3>();
        for (int i = 0; i < script.points.pointsNum; i++)
        {
            script.points.points.Add(new Vector3());
        }
        float angle = 365f / script.points.points.Count;
        var list = script.points.points;
        float y = 0;
        float x = 0;
        float z = 0;
        var pos = script.transform.position;
        var angleV = script.angle;
        for (int i = 0; i < list.Count; i++)
        {
            var dir = Vector3.forward;
            list[i] = pos + dir * script.points.distance;
            if (angleV.z > 0) z += angle;
            if (angleV.x > 0) x += angle;
            if (angleV.y > 0) y += angle;
        }
    }



}