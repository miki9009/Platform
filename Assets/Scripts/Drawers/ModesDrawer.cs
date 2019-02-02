using UnityEngine;
using Engine.Config;
using System;
using System.Linq;
using System.Collections.Generic;


[SerializableAttribute]
public class ModesDrawer : Engine.PopUpAttribute
{
    ModeConfig config;

    public ModesDrawer()
    {
        if (config == null)
        {
            config = Config.GetConfigEditor<ModeConfig>(ModeConfig.key);
        }
        if (config == null)
        {
            items = new string[] { "No config" };
        }
        items = config.modes;

    }


}