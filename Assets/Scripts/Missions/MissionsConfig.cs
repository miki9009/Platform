using Engine.Config;
using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = key)]
public class MissionsConfig : Config
{
    public const string key = "Configs/MissionsConfig";
    public List<MissionContainer> containers;

    public string selectedScene;

    static MissionsConfig instance;
    public static MissionsConfig Instance
    {
        get
        {
            if (instance == null)
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                    instance = Config.GetConfigEditor<MissionsConfig>(key);
                else
#endif
                    instance = ConfigsManager.GetConfig<MissionsConfig>();
            }
            return instance;
        }
    }

    public List<Mission> GetMissions(string scene)
    {
        foreach (var container in containers)
        {
            if (container.scene == scene)
                return container.missions;
        }
        Debug.LogError("No missions on scene: " + scene);
        return null;
    }

    public Mission GetMission(string scene, string missionID)
    {
        var missions = GetMissions(scene);
        for (int i = 0; i < containers.Count; i++)
        {
            if (missions[i].level == missionID)
                return missions[i];
        }
        Debug.LogError("Mission with ID " + missionID + " not found");
        return null;
    }

    public Mission GetMission(string missionID)
    {
        var missions = GetMissions(LevelsConfig.GetSceneName(missionID));
        for (int i = 0; i < containers.Count; i++)
        {
            if (missions[i].level == missionID)
                return missions[i];
        }
        Debug.LogError("Mission with ID " + missionID + " not found");
        return null;
    }

    public static string GetMode(string missionID)
    {
        var mission = Instance.GetMission(missionID);
        return mission.mode;
    }
}

[Serializable]
public class Mission
{
    [CustomLevelSelector]
    public string level;
    [ModesDrawer]
    public string mode;
    [CustomSceneSelector]
    public string customScene;
    public bool passed;
    public bool unlocked;
    public bool[] unlocks;
}

[Serializable]
public class MissionContainer
{
    [LevelSelector]
    public string scene;
    public List<Mission> missions;
}

#if UNITY_EDITOR
[CustomEditor(typeof(MissionsConfig))]
public class MissionsConfigEditor : Editor
{
    LevelSelector levels;
    int sceneIndex;

    public override void OnInspectorGUI()
    {

        base.OnInspectorGUI();

        var config = (MissionsConfig)target;
        GUILayout.Button("Custom");

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        if (levels == null)
            levels = new LevelSelector();

        string sceneName = config.selectedScene;
        for (int i = 0; i < levels.items.Length; i++)
        {
            if (sceneName == levels.items[i])
                sceneIndex = i;
        }

        EditorGUILayout.LabelField("SCENE", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Scene Name: ");
        sceneIndex = EditorGUILayout.Popup(sceneIndex, levels.items);
        EditorGUILayout.EndHorizontal();

    }
}
#endif