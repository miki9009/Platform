using System;
using UnityEngine;
using Engine;
using Engine.Config;
using System.Collections.Generic;

[SerializableAttribute]
public class SpawnsNamesAttribute : PopUpAttribute
{
    SpawnsConfig config;

    public SpawnsNamesAttribute()
    {
        if(config == null)
        {
            config = ConfigsManager.GetConfig<SpawnsConfig>();
        }
        if(config == null || config.spawnsNames == null)
        {
            items = new string[] { "empty" };
        }
        items = config.spawnsNames;
    }
}
