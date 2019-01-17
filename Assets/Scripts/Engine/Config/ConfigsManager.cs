using System.Collections.Generic;
using UnityEngine;
using Engine.Config;
using System.Linq;
using System;

public class ConfigsManager : MonoBehaviour
{
    public List<UnityEngine.Object> configs;

    private static ConfigsManager instance;
    static ConfigsManager Instance
    {
        get
        {
            if (instance == null)
                instance = GameObject.FindObjectOfType<ConfigsManager>();
            return instance;
        }

        set
        {
            instance = value;
        }
    }


    private void Awake()
    {
        Instance = this;
    }

    public static T GetConfig<T>() where T : Config
    {
        if (Instance == null)
        {
            Debug.LogError("Instance was null");
            return null;
        }
        return (T)Instance.configs.SingleOrDefault(x => x.GetType() == typeof(T));
    }

    public static T GetConfigs<T>() where T : Config
    {
        return (T)Instance.configs.Where(x => x.GetType() == typeof(T));
    }

}
