using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SplineElement))]
public class SplineElementEditor : Editor
{
    float range;
    float prevRange;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        //var script = (SplineElement)target;

        //range = EditorGUILayout.FloatField(range, "Circle Range");

        //if(prevRange != range)
        //{
        //    prevRange = range;
        //    var points = script.points;
        //    float angle = 0;
        //    float angleFactor = 360f / points.Length;
        //    for (int i = 0; i < points.Length; i++)
        //    {
        //        angle += angleFactor * i;
        //        var dir = Quaternion.Euler(0, angle, 0) * Vector3.forward;
        //        points[i].localPosition = dir * range;

        //    }
        //}

        
    }
}