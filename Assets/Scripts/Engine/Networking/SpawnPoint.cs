using Engine;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : LevelElement
{
    public static List<SpawnPoint> spawnPoints = new List<SpawnPoint>();

    public int index = 0;

    private void Awake()
    {
        spawnPoints.Add(this);
    }

    private void OnDestroy()
    {
        spawnPoints.Remove(this);
    }

    public override void OnLoad()
    {
        base.OnLoad();
        if(data.ContainsKey("Index"))
        {
            index = (int)data["Index"];
        }
    }

    public override void OnSave()
    {
        base.OnSave();
        data.Add("Index", index);
    }

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, 1);
    }
#endif
}