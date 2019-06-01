using UnityEngine;
using Engine;
using Engine.UI;
using System;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class World : MonoBehaviour
{
    public static event Action Initialized;
    public static World Instance { get; private set; }
    public static int CurrentIndex { get { return MissionConfigHandler.CurrentMissionIndex; } }

    public WorldWindow Window { get; private set; }

    static Dictionary<string, WorldLevel> worldLevels = new Dictionary<string, WorldLevel>();

    public static void AddWorldLevel(WorldLevel worldLevel)
    {
        if(!worldLevels.ContainsKey(worldLevel.mission))
        {
            worldLevels.Add(worldLevel.mission, worldLevel);
        }
    }

    public static void RemoveWorldLevel(WorldLevel worldLevel)
    {
        if (worldLevels.ContainsValue(worldLevel))
        {
            worldLevels.Remove(worldLevel.mission);
        }
    }

    public static WorldLevel FindTargetLevel(WorldLevel worldLevel)
    {
        foreach(var item in worldLevels.Values)
        {
            if(worldLevel.index == item.index-1)
            {
                return item;
            }
        }

        return null;
    }

    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        worldLevels.Clear();
    }

    



}