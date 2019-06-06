using UnityEngine;
using UnityEditor;
using System.Collections;
using Engine;
using UnityEngine.SceneManagement;
using System.Threading;
using System;
using System.Collections.Generic;

class LevelWindow : EditorWindow
{
    [MenuItem("Window/Levels")]
    public static void ShowWindow()
    {
        var window = (LevelWindow)GetWindow(typeof(LevelWindow));
    }

    static int selected;
    static int levelSelected;
    static int customSceneSelected;
    static int currentModeIndex;
    bool firstTime = true;
    LevelSelector levels;

    void OnGUI()
    {
        if (Application.isPlaying) return;
        if (levels == null)
            levels = new LevelSelector();


            string sceneName = Level.Config.selectedScene;
            for (int i = 0; i < levels.items.Length; i++)
            {
                if (sceneName == levels.items[i])
                    selected = i;
            }

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("LEVELS", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Scene Name: ");
        selected = EditorGUILayout.Popup(selected, levels.items);
        EditorGUILayout.EndHorizontal();

        Level.SceneName = levels.items[selected];

        var levelGroup = Level.Config.GetLevel(Level.SceneName);
        string[] customLevels = new string[0];
        if (levelGroup != null && levelGroup.levels != null)
        {
            customLevels = levelGroup.levels.ToArray();


            EditorGUILayout.BeginHorizontal();
            string currentCustomLevel = Level.Config.selectedLevel;
            levelSelected = 0;
            for (int i = 0; i < customLevels.Length; i++)
            {
                if (customLevels[i] == currentCustomLevel)
                    levelSelected = i;
            }
            EditorGUILayout.LabelField("Custom Level: ");
            levelSelected = EditorGUILayout.Popup(levelSelected, customLevels);
            Level.Config.selectedLevel = customLevels[levelSelected];
            EditorGUILayout.EndHorizontal();
        }

        if (customLevels != null && customLevels.Length > levelSelected)
        {
            Level.levelName = customLevels[levelSelected];

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(levels.items[selected] +":" + Level.levelName);
            if (GUILayout.Button("Save"))
            {
                Level.Save(Level.levelName);
            }
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(levels.items[selected] + ":" + Level.levelName);
            if (GUILayout.Button("Load"))
            {
                Level.Load(Level.levelName);
            }
            EditorGUILayout.EndHorizontal();
        }
        if (GUILayout.Button("Clear"))
        {
            Level.Clear();
        }
        if (GUILayout.Button("Reload IDs"))
        {
            Level.ReloadIDs();
        }


        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("CUSTOM SCENES FOR: " + sceneName, EditorStyles.boldLabel);

        var sceneGroup = CustomScene.Config.GetSceneGroup(sceneName);
        CustomScene.Config.sceneName = sceneName;
        if (sceneGroup!=null)
        {
            var customScenes = new List<string>(sceneGroup.customScenes);
            customScenes.Insert(0, "none");

            EditorGUILayout.BeginHorizontal();
            string currentCustomScene = CustomScene.Config.selectedCustomScene;

            for (int i = 0; i < customScenes.Count; i++)
            {
                if (customScenes[i] == currentCustomScene)
                    customSceneSelected = i;
            }

            if (customScenes.Count <= customSceneSelected)
                customSceneSelected = 0;

                EditorGUILayout.LabelField("Custom Scene: ");
                customSceneSelected = EditorGUILayout.Popup(customSceneSelected, customScenes.ToArray());
                CustomScene.Config.selectedCustomScene = customScenes[customSceneSelected];
                EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(customScenes[customSceneSelected]);
            if (GUILayout.Button("Save"))
            {
                CustomScene.Save(sceneName, customScenes[customSceneSelected]);
            }
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(customScenes[customSceneSelected]);
            if (GUILayout.Button("Load"))
            {
                CustomScene.Load(sceneName, customScenes[customSceneSelected]);
            }
            EditorGUILayout.EndHorizontal();
        }
        if (GUILayout.Button("Clear Sceneries"))
        {
            CustomScene.Clear();
        }

        Level.Config.testLevel = EditorGUILayout.Toggle("Play on load", Level.Config.testLevel);
        EditorGUILayout.BeginHorizontal();

        if (Level.Config.testLevel)
        {
            EditorGUILayout.LabelField("Play Level: ");
            if (GUILayout.Button("Play"))
            {
                Level.Play();
            }
        }

        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Count GameObjects"))
        {
            var objects = GameObject.FindObjectsOfType<GameObject>();
            Debug.Log("Found: " + objects.Length + " Game Objects.");
        }
        //if (GUILayout.Button("Load Non compressed"))
        //{
        //    Level.Load(Level.levelName,false);
        //}
    }






}