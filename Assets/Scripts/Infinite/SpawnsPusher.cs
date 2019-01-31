using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SpawnsPusher : MonoBehaviour
{
    public List<SpawnPrefab> spawnPrefabs;

    void Awake()
    {
        foreach (var spawn in spawnPrefabs)
        {
            SpawnManager.AddSpawn(spawn.name, spawn.gameObject, spawn.count);
        }
    }

}
[Serializable]
public class SpawnPrefab
{
    public string name;
    public GameObject gameObject;
    public int count;
}

