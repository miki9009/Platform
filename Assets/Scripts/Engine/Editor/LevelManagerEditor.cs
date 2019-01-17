using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Engine
{
    [CustomEditor(typeof(LevelManager))]
    public class LevelManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var script = (LevelManager)target;
            if (GUILayout.Button("Get Scenes"))
            {
                script.GetScenes();
            }
            DrawDefaultInspector();
        }




    }
}
