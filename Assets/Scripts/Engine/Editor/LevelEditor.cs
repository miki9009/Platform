using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace Engine
{
    [CustomEditor(typeof(Level))]
    public class LevelEditor : Editor
    {
        static int selected;
        static int levelSelected;
        static int currentModeIndex;
        bool firstTime = true;
        LevelSelector levels;
        //ModeConfig modeConfig;

        public override void OnInspectorGUI()
        {
            //var script = (Level)target;
            
            if(levels == null)
                levels = new LevelSelector();

            if (firstTime)
            {
                string sceneName = SceneManager.GetActiveScene().name;
                for (int i = 0; i < levels.items.Length; i++)
                {
                    if (sceneName == levels.items[i])
                        selected = i;
                }
                firstTime = false;
            }

            EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Scene Name: ");
                selected = EditorGUILayout.Popup(selected, levels.items);
            EditorGUILayout.EndHorizontal();

            Level.SceneName = levels.items[selected];

            var levelGroup = Level.Config.GetLevel(Level.SceneName);
            string[] customLevels = new string[0];
            if(levelGroup != null)
            {
                customLevels = levelGroup.levels.ToArray();
            }

            EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Custom Level: ");
                levelSelected = EditorGUILayout.Popup(levelSelected, customLevels);
            EditorGUILayout.EndHorizontal();

            //EditorGUILayout.BeginHorizontal();
            //if(modeConfig == null)
            //{
            //    modeConfig = Config.Config.GetConfigEditor<ModeConfig>(ModeConfig.key);
            //}
            //EditorGUILayout.LabelField("Mode: ");
            //    currentModeIndex = EditorGUILayout.Popup(currentModeIndex, modeConfig.modes);
            //    LevelSettings.CurrentLevelSettings.mode = modeConfig.modes[currentModeIndex];
            //EditorGUILayout.EndHorizontal();

            if (customLevels!=null&& customLevels.Length > levelSelected)
            {
               // Color defaultColor = UnityEngine.GUI.backgroundColor;
               // UnityEngine.GUI.backgroundColor = Color.gray;
                Level.levelName = customLevels[levelSelected];

                if (GUILayout.Button("Save"))
                {
                    Level.Save(Level.levelName);
                }
                if (GUILayout.Button("Load"))
                {
                    Level.Load(Level.levelName);
                }
            }
            if (GUILayout.Button("Clear"))
            {
                Level.Clear();
            }
            if (GUILayout.Button("Reload IDs"))
            {
                Level.ReloadIDs();
            }
        }
    }


}