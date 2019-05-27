using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using Engine.Config;

[CreateAssetMenu(menuName = key)]
public class CustomScenesConfig : Config
{
    public static string currentScene;

    public const string key = "Configs/CustomScenesConfig";
    public string sceneName;
    public string selectedCustomScene = "0";
    public string customScenesPath = "Resources/CustomScenes/";
    public List<CustomSceneGroup> sceneGroups;

    static CustomScenesConfig _ins;
    static CustomScenesConfig Instance
    {
        get
        {
            if (_ins == null)
                _ins = GetConfigEditor<CustomScenesConfig>(key);
            return _ins;
        }
    }

    public CustomSceneGroup GetSceneGroup(string sceneName)
    {
        for (int i = 0; i < sceneGroups.Count; i++)
        {
            if (sceneGroups[i].sceneName == sceneName)
            {
                return sceneGroups[i];
            }
        }
        return null;
    }

    public static List<string> GetAllCustomScenes()
    {
        var list = new List<string>();
        list.Add("none");
        var sceneGroups = Instance.sceneGroups;
        for (int i = 0; i < sceneGroups.Count; i++)
        {
            foreach (var customScene in sceneGroups[i].customScenes)
            {
                list.Add(customScene);
            }
        }
        return list;
    }

    public List<string> GetAllCustomScenesForScene(string sceneName)
    {
        for (int i = 0; i < sceneGroups.Count; i++)
        {
            if (sceneGroups[i].sceneName == sceneName)
            {
                return sceneGroups[i].customScenes;
            }
        }
        return new List<string>();
    }

}
[System.Serializable]
public class CustomSceneGroup
{
    [LevelSelector]
    public string sceneName;
    public List<string> customScenes;
}