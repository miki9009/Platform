using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(MovingTransform))]
public class MovingTransformEditor : Editor
{
	MovingTransform script; 
	public override void OnInspectorGUI()
	{
        DrawDefaultInspector();
        script = (MovingTransform)target;



		if (GUILayout.Button(script.rotate ? "Rotation:ON" : "Rotation:OFF"))
		{
			script.rotate = !script.rotate;
		}
        if (GUILayout.Button("Add anchor"))
        {
            AddAnchor(script);
        }
        EditorGUILayout.LabelField("Number of points");
        script.numberOfPoints = EditorGUILayout.IntSlider(script.numberOfPoints, 1, 20);
        if (GUILayout.Button("Add anchors"))
        {
            for (int i = 0; i < script.numberOfPoints; i++)
            {
                AddAnchor(script);
            }
        }
        if (GUILayout.Button("Add Point"))
		{
            AddPoint(script);
		}


        if (GUILayout.Button("Generate Circle"))
        {
            CreateCircle(script);
        }
        if (GUILayout.Button("Refresh anchor points"))
		{
            AnchorsToPoints(script);
		}
        if (GUILayout.Button("Clear anchors"))
        {
            foreach (var anchor in script.anchors)
            {
                if (anchor != null)
                {
                    DestroyImmediate(anchor.gameObject);
                }
            }
            script.anchors = new List<Transform>();
        }
        if (GUILayout.Button(script.isSimulating ?  "Stop simulating" : "Simulate"))
        {
            if (script.isSimulating)
            {
                script.StopAllCoroutines();
                script.isSimulating = false;
            }
            else
            {
                Simulate(script);
                script.isSimulating = true;
            }
        }

        EditorGUILayout.LabelField("Distance");
        script.distance = (float)Math.Round(EditorGUILayout.Slider(script.distance, 0.1f, 100f), 1);



    }

	public void AddAnchor(MovingTransform script)
	{
		if (script.anchors == null)
		{
			script.anchors = new List<Transform>();
		}

		var obj = new GameObject("Anchor: " + script.anchors.Count);
		obj.transform.position = script.transform.position;
		obj.transform.parent = script.transform;
        script.anchors.Add(obj.transform);
	}

    void Simulate(MovingTransform script)
    {
        AddPoint(script);
        script.StopAllCoroutines();
        script.StartCoroutine(script.Move());
    }

    void AddPoint(MovingTransform script)
    {
        if (script.anchor == null)
        {
            var obj = new GameObject("Point");
            obj.AddComponent<Rigidbody>().useGravity = false;
            script.anchor = obj.transform;
            script.anchor.position = script.transform.position;
            script.anchor.parent = script.transform;
        }
    }

	void AnchorsToPoints(MovingTransform script)
	{
		script.points = new List<Vector3>();
		foreach (var anchor in script.anchors)
		{
            if (anchor != null)
            { 
			script.points.Add(anchor.position);
            }
		}
	}

    void CreateCircle(MovingTransform script)
    {
        if (script.anchors == null || script.anchors.Count == 0) return;
        float angle = 365f / script.anchors.Count;
        var list = script.anchors;
        float y = 0;
        var pos = script.transform.position;
        for (int i = 0; i < list.Count; i++)
        {
            var dir = Quaternion.Euler(0, y, 0) * Vector3.forward;
            list[i].position = pos + dir * script.distance;
            y += angle;
        }
    }



}
