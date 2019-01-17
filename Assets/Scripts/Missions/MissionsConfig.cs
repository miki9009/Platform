using Engine.Config;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = key)]
public class MissionsConfig : Config
{
    public const string key = "Configs/MissionsConfig";
    public List<Mission> missions;

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

    public Mission GetMission(string id)
    {
        for (int i = 0; i < missions.Count; i++)
        {
            if (missions[i].level == id)
                return missions[i];
        }
        Debug.LogError("Mission with ID " + id + " not found");
        return null;
    }
}

[Serializable]
public class Mission
{
    [CustomLevelSelector]
    public string level;
    public bool passed;
    public bool unlocked;
    public bool[] unlocks;
}