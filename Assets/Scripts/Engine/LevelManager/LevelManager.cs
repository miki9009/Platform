﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Engine;
using System.IO;
using System.Collections;
using Engine.UI;

public class LevelManager : MonoBehaviour
{
    public static event Action BeforeSceneLoading;

    public static event Action LevelSelected;
    public static LevelManager Instance
    {
        get; private set;
    }

    static string _customLevelToLoad;
    public static string CurrentCustomLevel
    {
        get
        {
            return _customLevelToLoad;
        }

        set
        {
            _customLevelToLoad = value;
        }
    }

    public static string CurrentCustomScene{get; private set;}

    static string _sceneToLoad;
    public static string SceneToLoad
    {
        get
        {
            return _sceneToLoad;
        }
    }

    public int levelIndex;
    public string scenesPath;
    public List<string> scenes;
    public List<Scenes> levels;
    public static string CurrentScene;
    [LevelSelector]
    public string gameScene;

    public static string[] Scenes
    {
        get
        {
            return GetScenesStatic();
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SceneManager.activeSceneChanged += PrintSceneName;
        GameManager.GameReady += () =>
        {
            CurrentScene = SceneManager.GetActiveScene().name;
            bool isLevel = levels.Exists(x => x.sceneName == CurrentScene);
            if (isLevel)
            {
                for (int i = 0; i < levels.Count; i++)
                {
                    if (levels[i].sceneName == CurrentScene)
                    {
                        levelIndex = i;
                        Debug.Log("LEVEL LOADED: " + levels[levelIndex].sceneName);
                    }
                }
            }
            StartCoroutine(HideLoadingScreen());

        };
    }

    void PrintSceneName(Scene prevScene, Scene newScene)
    {
        Debug.Log("ACTIVE SCENE: " + SceneManager.GetActiveScene().name);
    }

    public static void LoadMenu3D()
    {
        var scene = SceneManager.GetSceneByName("Menu3D");
        if (!scene.isLoaded)
            SceneManager.LoadSceneAsync("Menu3D", LoadSceneMode.Additive);
    }

    public static void UnloadMenu3D()
    {
        var scene = SceneManager.GetSceneByName("Menu3D");
        if (scene.isLoaded)
            SceneManager.UnloadSceneAsync(scene);
    }

    public static void GoToSingleScene(string sceneName, bool showLoadingScreen = true)
    {
        LoadScene(sceneName, LoadSceneMode.Single, showLoadingScreen);
        LevelSelected?.Invoke();
    }

    public static void LoadLevelAdditive(string sceneName, bool showLoadingScreen = true)
    {
        if(showLoadingScreen)
        {
            var window = UIWindow.GetWindow(UIWindow.LOADING_SCREEN);
            if (window != null)
                window.Show();
        }
        var scene = SceneManager.GetSceneByName("Menu3D");
        if (scene.isLoaded)
            SceneManager.UnloadSceneAsync(scene);
        LoadScene(sceneName, LoadSceneMode.Additive, showLoadingScreen);
    }

    public void GetScenes()
    {
        if (scenesPath != null)
        {
            scenes.Clear();
            var path = Application.dataPath + "/" + scenesPath;
            if (Directory.Exists(path))
            {
                var files = Directory.GetFiles(path);

                foreach (var file in files)
                {
                    var f = file.Substring(path.Length + 1);
                    if (f.Contains(".unity") && !f.Contains(".meta"))
                    {
                        var str = f.Split('.');
                        scenes.Add(str[0]);
                    }
                }
            }
        }
    }

    public static string[] GetScenesStatic()
    {
        List<string> scenes = new List<string>();
        try
        {

            scenes.Clear();
            var path = Application.dataPath + "/Scenes";
            if (Directory.Exists(path))
            {
                var files = Directory.GetFiles(path);

                foreach (var file in files)
                {
                    var f = file.Substring(path.Length + 1);
                    if (f.Contains(".unity") && !f.Contains(".meta"))
                    {
                        var str = f.Split('.');
                        scenes.Add(str[0]);
                    }
                }
            }
            else
            {
                Debug.LogError("Make sure that /Scenes exist, and that scenes are inside of it");
            }
        }
        catch
        {

        }

        return scenes.ToArray();
    }

    static void LoadScene(string sceneName, LoadSceneMode mode, bool showLoadingScreen = true)
    {
        OnBeforeSceneLoading();
        if (showLoadingScreen)
        {
            var window = UIWindow.GetWindow(UIWindow.LOADING_SCREEN);
            if (window != null)
                window.Show();
            Debug.Log("Show loading screen");
        }

        SceneManager.LoadSceneAsync(sceneName, mode);
    }

    public static void BeginCustomLevelLoadSequenceAdditive(string sceneName, string customLevel, string customScene = null)
    {
        _sceneToLoad = sceneName;
        CurrentCustomLevel = customLevel;
        CurrentCustomScene = customScene;
        var scene = SceneManager.GetSceneByName("Menu3D");
        if(scene.isLoaded)
            SceneManager.UnloadSceneAsync(scene);

        Debug.Log("Current level set to: " + _sceneToLoad);
        GameManager.CurrentScene = _sceneToLoad;
        LoadScene(Instance.gameScene, LoadSceneMode.Additive);
        SceneManager.sceneLoaded += Instance.AddLevelScene;
        LoadCustomLevel += OnLoadCustomLevel;
    }

    public static void BeginCustomLevelLoadSequenceAdditive(Mission mission)
    {
        GameManager.CurrentMission = mission;
        BeginCustomLevelLoadSequenceAdditive(mission.scene, mission.level, mission.customScene);
    }

    public static void BeginCustomLevelLoadSequenceAdditive(string missionID)
    {
        var mission = MissionsConfig.Instance.GetMission(missionID);
        GameManager.CurrentMission = mission;
        if (mission.IsValid)
        {
            BeginCustomLevelLoadSequenceAdditive(mission.scene, mission.level, mission.customScene);
        }
        else
        {
            Debug.LogError("Mission invalid, ID was: " + missionID);
        }

    }

    public static void ChangeLevel(string sceneName, string customLevel)
    {
        GameManager.OnLevelClear();
        if(sceneName == GameManager.CurrentScene)
        {
            LoadOnlyCusomLevel(customLevel);
        }
        else
        {
            SceneManager.UnloadSceneAsync(GameManager.CurrentScene);
            _sceneToLoad = sceneName;
            CurrentCustomLevel = customLevel;
            Debug.Log("Current level set to: " + _sceneToLoad);
            GameManager.CurrentScene = _sceneToLoad;
            Instance.AddLevelScene(SceneManager.GetSceneByName(sceneName), LoadSceneMode.Additive);
            LoadCustomLevel += OnLoadCustomLevel;
        }
    }

    public static void LoadOnlyCusomLevel(string customLevel)
    {
        Character character = Character.GetLocalPlayer();
        if (character != null)
            Destroy(character.gameObject);
        if (!string.IsNullOrEmpty(CurrentCustomLevel) && customLevel != "none")
        {
            CurrentCustomLevel = customLevel;
            Level.LoadWithScene(SceneManager.GetActiveScene().name, CurrentCustomLevel, null);
        }
    }

    public static void ReturnToMenu(bool loadMenu3D = true)
    {
        OnBeforeSceneLoading();
        ShowLoadingScreen();
        GameManager.OnLevelClear();
        SceneManager.UnloadSceneAsync(Instance.gameScene);
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        var window = UIWindow.GetWindow("GameUI");
        if(window!=null)
        {
            window.FadeOut();
        }
        if(loadMenu3D)
            LoadMenu3D();
    }

    void AddLevelScene(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= AddLevelScene;
        LoadScene(_sceneToLoad, LoadSceneMode.Additive);
        SceneManager.sceneLoaded += SetActiveScene;
    }

    static event Action LoadCustomLevel;
    void SetActiveScene(Scene scene, LoadSceneMode mode)
    {
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(_sceneToLoad));
        SceneManager.sceneLoaded -= SetActiveScene;
        LoadCustomLevel?.Invoke();
    }

    static void OnBeforeSceneLoading()
    {
        BeforeSceneLoading?.Invoke();
    }



    static void OnLoadCustomLevel()
    {
        CurrentScene = SceneManager.GetActiveScene().name;
        if (!string.IsNullOrEmpty(CurrentCustomScene))
        {
            CustomScene.Load(CurrentScene, CurrentCustomScene);
        }

        if(!string.IsNullOrEmpty(CurrentCustomLevel))
        {
            Level.LoadWithScene(CurrentScene, CurrentCustomLevel, CurrentCustomScene);
        }
        LoadCustomLevel -= OnLoadCustomLevel;
    }


    public string NextLevel()
    {
        int index = 0;
        if (levelIndex+1 < levels.Count)
        {
            index = levelIndex + 1;
        }
        return levels[index].sceneName;
    }

    public static void ShowLoadingScreen()
    {
        var window = UIWindow.GetWindow(UIWindow.LOADING_SCREEN);
        if (window != null)
            window.Show();
    }

    IEnumerator HideLoadingScreen()
    {
        var window = UIWindow.GetWindow(UIWindow.LOADING_SCREEN);
        if (window != null)
            window.Hide();
        Debug.Log("Screen Hide");
        yield return null;
    }
}

[Serializable]
public class Scenes
{
    [LevelSelector]
    public string sceneName;
}
