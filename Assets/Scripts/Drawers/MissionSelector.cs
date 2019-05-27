using UnityEngine;
using Engine.Config;
using System;
using System.Linq;
using System.Collections.Generic;


[SerializableAttribute]
public class MissionSelector : Engine.PopUpAttribute
{
    MissionsConfig config;

    public MissionSelector()
    {
        if (config == null)
        {
            config = Config.GetConfigEditor<MissionsConfig>(MissionsConfig.key);
        }
        if (config == null)
        {
            items = new string[] { "No config" };
        }
        var list = new List<string>();
        foreach (var container in config.containers)
        {
            foreach(var mission in container.missions)
            {
                list.Add(mission.ToString());
            }
        }
        items = list.ToArray();
    }
}