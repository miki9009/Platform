using Engine.Config;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = key)]
public class ModeConfig : Config
{
    public const string key = "Configs/ModesConfig";
    public string[] modes;

    static ModeConfig instance;
    public static ModeConfig Instance
    {
        get
        {
            if (instance == null)
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                    instance = Config.GetConfigEditor<ModeConfig>(key);
                else
#endif
                    instance = ConfigsManager.GetConfig<ModeConfig>();
            }
            return instance;
        }
    }

}
