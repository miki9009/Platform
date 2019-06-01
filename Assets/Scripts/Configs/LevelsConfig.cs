using Engine.Config;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = key)]
public class LevelsConfig : Config
{
    public static string currentScene;

    public const string key = "Configs/LevelsConfig";
    public bool testLevel;
    public string selectedScene;
    public string selectedLevel;
    public string levelPaths;
    public string levelElementsPath;
    public List<LevelGroup> levels;

    public LevelGroup GetLevel(string levelGroupName)
    {
        for (int i = 0; i < levels.Count; i++)
        {
            if (levels[i].sceneName == levelGroupName)
                return levels[i];
        }
        return null;
    }

    public static string GetFullName(string sceneName, string levelName)
    {
        return sceneName + ":" + levelName;
    }

    public static string GetLevelName(string fullName)
    {
        string[] names = fullName.Split('_');
        return names[2];
    }

    public static string GetSceneName(string fullName)
    {
        string[] names = fullName.Split('_');
        return names[0];
    }

    public List<string> GetAllLevels(string sceneName)
    {
        for (int i = 0; i < levels.Count; i++)
        {
            if(levels[i].sceneName == sceneName)
            {
                return levels[i].levels;
            }
        }
        return new List<string>();
    }
}
[System.Serializable]
public class LevelGroup
{
    [LevelSelector]
    public string sceneName;
    public List<string> levels;
}


