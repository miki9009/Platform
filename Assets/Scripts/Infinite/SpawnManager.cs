using System.Collections.Generic;
using UnityEngine;

public static class SpawnManager 
{
    //static Dictionary<string, GameObject> spawnsOriginal = new Dictionary<string, GameObject>();

    static Dictionary<string, Stack<GameObject>> spawns = new Dictionary<string, Stack<GameObject>>();


    public static void AddSpawn(string name, GameObject prefab, int count)
    {
        var stack = new Stack<GameObject>();
        for (int i = 0; i < count; i++)
        {
            var obj = Object.Instantiate(prefab);
            var spawn = obj.AddComponent<Spawn>();
            obj.SetActive(false);
            spawn.SpawnName = name;
            spawn.SpawnInstance = obj.GetComponent<IPoolObject>();
            if(spawn.SpawnInstance == null)
            {
                Debug.LogError("IPoolObject not found on: " + prefab.name);
            }

            stack.Push(obj);
        }
        if(!spawns.ContainsKey(name))
        {
            spawns.Add(name, stack);
        }
        else
        {
            spawns[name] = stack;
        }
        //if (!spawnsOriginal.ContainsKey(name))
        //{
        //    spawnsOriginal.Add(name, prefab);
        //}
        //else
        //{
        //    spawnsOriginal[name] = prefab;
        //}
    }


    public static GameObject GetSpawn(string name, bool activate = true)
    {
        //Debug.Log("Spawn name Spawn Manager:" + name);
        if(!spawns.ContainsKey(name))
        {
            Debug.Log("No spawn with name: " + name);
            return null;
        }
        var stack = spawns[name];
        GameObject obj;
        if(stack.Count > 0)
        {
            obj = stack.Pop();
        }
        else
        {
            Debug.LogError("Not enough spawns "+name);
            return null;
        }
        obj.SetActive(activate);
        return obj;
    }

    static void PutSpawnBack(Spawn spawn)
    {
        if (!spawns.ContainsKey(spawn.SpawnName)) return;
        var stack = spawns[spawn.SpawnName];
        stack.Push(spawn.gameObject);
    }

    public static void Recycle(IPoolObject obj)
    {
        var spawn = obj.GameObject.GetComponent<Spawn>();
        PutSpawnBack(spawn);
        obj.Recycle();
        obj.GameObject.transform.position = Vector3.zero;
        if(obj.GameObject.activeInHierarchy)
            obj.GameObject.SetActive(false);
    }

    public static void ClearSpawns()
    {
        spawns.Clear();
    }
}

public interface IPoolObject
{
    GameObject GameObject { get; }
    void Recycle();
}




