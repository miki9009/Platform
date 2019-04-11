using Engine.Config;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = key)]
public class MissionsConfig : Config
{
    public const string key = "Configs/MissionsConfig";
    public List<MissionContainer> containers;

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