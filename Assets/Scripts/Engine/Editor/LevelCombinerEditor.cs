using UnityEditor;
using UnityEngine;

public class LevelCombinerEditor:Editor
{
    [CustomEditor(typeof(LevelCombiner))]
    public class ButtonInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            var script = (LevelCombiner)target;

            DrawDefaultInspector();
            if (GUILayout.Button("Execute"))
            {
                script.Execute();
            }
        }
    }
}