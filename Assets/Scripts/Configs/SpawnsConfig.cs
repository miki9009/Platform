using Engine.Config;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Configs/SpawnConfig")]
public class SpawnsConfig : Config
{
    public List<SpawnCollection> spawnCollections;
    public string[] spawnsNames;

    [System.Serializable]
    public class SpawnSettings
    {
        [Header("SPAWN SETTINGS")]
        [SpawnsNames]
        public string spawnName;
        [SpawnsCollectionSelector]
        public string spawnType;
        [Range(1,100)]
        public float chances;
        public GameObject prefab;
        public int numberOfSpawns;
        public bool randomAmount;
        public int amountSpawnAtOnce = 1;
    }

    [System.Serializable]
    public class SpawnCollection
    {
        [Header("COLLECTION")]
        [SpawnsCollectionSelector]
        public string collectionName;
        [Range(1, 100)]
        public float chances;
        public List<SpawnSettings> spawnSettings;
        public SpawnSettings[] SettingsCollection { get; set; }
    }
}

public class SpawnsCollectionType
{
    public const string COLLECTOIN = "Collection";
    public const string ENEMY = "Enemy";
    public const string POWER_UP = "PowerUp";
    public const string OBSTACLE = "Obstacle";
}
