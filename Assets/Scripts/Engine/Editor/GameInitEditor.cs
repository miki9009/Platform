using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Engine
{
    [CustomEditor(typeof(DataManager))]
    public class GameInitEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var script = (DataManager)target;

            DrawDefaultInspector();

            if (GUILayout.Button("Save current data"))
            {
                DataManager.SaveData();
            }
            if (GUILayout.Button("Load data"))
            {
                DataManager.LoadData();
            }
            if (GUILayout.Button("Clear data"))
            {
                System.IO.File.Delete(script.dataFileName);
                PlayerPrefs.DeleteAll();
                DataManager.SaveData();
            }
        }
    }
}
