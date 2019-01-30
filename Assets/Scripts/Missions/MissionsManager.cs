using Engine;
using Engine.Singletons;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MissionsManager : Module
{
    static DataProperty<bool[]> unlockedMissions;
    static MissionsConfig missionsConfig;
    static List<Mission> activeMissions;

    public override void Initialize()
    {
        missionsConfig = ConfigsManager.GetConfig<MissionsConfig>();
        var unlocked = new List<bool>();
        for (int i = 0; i < missionsConfig.containers.Count; i++)
        {
            missionsConfig.containers[i].missions.ForEach(x => unlocked.Add(x.unlocked));
        }
        var array = unlocked.ToArray();
        unlockedMissions = DataProperty<bool[]>.Get("UnlockedMissions", array);
        GameManager.GameReady += InitializeMissions;
        SceneManager.sceneLoaded += ClearMissions;
    }

    void InitializeMissions()
    {
        Console.WriteLine("Level Loaded (Missions Manager)", Console.LogColor.Blue);

    }

    void ClearMissions(Scene scene, LoadSceneMode mode)
    {
        //activeMissions = new List<Mission>();
    }
}