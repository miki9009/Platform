using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Chunk))]
public class ChunkEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        var script = (Chunk)target;

        if (GUILayout.Button("Set size"))
        {
            script.SetSize();
        }
    }
}