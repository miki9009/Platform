using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Significant))]
public class SignificantEditor : Editor
{

    static readonly char[] chars = new char[] { 'a', 'b', 'c','d','e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'r', 's', 't', 'u',
    'w', 'x', 'y','z','q','1', '2', '3','4','5','6', '7', '8','9','0','!', '@', '#','$','%','^', '&', '*','(',')','_', '-', '=','+','|',
    ';', ':', '"','.',',','>', '<','A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','R','S','T','W','U','Y','X','Z','Q'};

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        var script = (Significant)target;

        if (GUILayout.Button("Generate Key"))
        {
            int count = chars.Length - 1;
            string key = "";
            int length = script.keyLength;
            for (int i = 0; i < length; i++)
            {
                key += chars[Random.Range(0, count)];
            }
            script.propertyKey = key;
        }
    }
}