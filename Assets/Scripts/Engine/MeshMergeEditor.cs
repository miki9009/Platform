using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Engine
{
    [CustomEditor(typeof(MeshMerge))]
    public class ButtonInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            var script = (MeshMerge)target;

            DrawDefaultInspector();
            if (GUILayout.Button("Merge"))
            {
                script.Merge();
            }

        }
    }
}
#endif
