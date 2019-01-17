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
        string[] names = fullName.Split(':');
        return names[1];
    }

    public static string GetSceneName(string fullName)
    {
        string[] names = fullName.Split(':');
        return names[0];
    }
}
[System.Serializable]
public class LevelGroup
{
    [LevelSelector]
    public string sceneName;
    public List<string> levels;
}


