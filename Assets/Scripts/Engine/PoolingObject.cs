using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Engine
{
    public class PoolingObject : MonoBehaviour
    {
        //public static PoolingObject PoolingManager
        //{
        //    get
        //    {
        //        if (PoolingManager == null)
        //        {
        //            PoolingManager = new GameObject("PoolingManager").AddComponent<PoolingObject>();
        //        }
        //        return PoolingManager;
        //    }
        //    private set
        //    {
        //        PoolingManager = value;
        //    }
        //}


        //void Awake()
        //{
        //    PoolingManager = this;
        //}

        static Dictionary<string, Stack<GameObject>> spawns = new Dictionary<string, Stack<GameObject>>();
        public static GameObject GetSpawn(string name, Vector3 position, Quaternion rotation)
        {
            if (spawns.ContainsKey(name) && spawns[name].Count > 0)
            {
                var spawn = spawns[name].Pop();
                spawn.SetActive(true);
                spawn.transform.position = position;
                spawn.transform.rotation = rotation;
                return spawn;
            }
            else
            {
                Debug.LogError("Stack was empty or null");
                return null;
            }
        }



        public static void AddSpawn(string name, GameObject gameObject)
        {
            if (gameObject == null)
            {
                Debug.LogError("GameObject not assigned to PoolingObject");
                return;
            }
            gameObject.SetActive(false);
            if (spawns.ContainsKey(name))
            {
                spawns[name].Push(gameObject);
            }
            else
            {
                spawns.Add(name, new Stack<GameObject>());
                spawns[name].Push(gameObject);
            }
        }

        public static void Recycle(string name, GameObject gameObject, Action resetMethod)
        {
            if (resetMethod != null)
            {
                resetMethod();
            }
            AddSpawn(name, gameObject);
        }
    }
}
public static class GameObjectExtension
{
    public static string GetName(this GameObject gameObject)
    {
        var str = gameObject.name.Split(' ');
        return str[0];
    }
}