using UnityEngine;
using Engine.Config;
using System;
using System.Linq;
using System.Collections.Generic;


[SerializableAttribute]
public class CustomLevelSelector : Engine.PopUpAttribute
{
    LevelsConfig config;

    public CustomLevelSelector()
    {
        if (config == null)
        {
            config = Config.GetConfigEditor<LevelsConfig>(LevelsConfig.key);
        }
        if (config == null)
        {
            items = new string[] { "No config" };
        }
        var list = new List<string>();
        foreach (var levelGroup in config.levels)
        {
            for (int i = 0; i < levelGroup.levels.Count; i++)
            {
                list.Add(LevelsConfig.GetFullName(levelGroup.sceneName, levelGroup.levels[i]));
            }
        }
        items = list.ToArray();

    }
  

}