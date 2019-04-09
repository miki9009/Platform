using Engine;
using Engine.Config;
using System;
using UnityEngine;

[SerializableAttribute]
public class DaysDrawer : PopUpAttribute
{
    public DaysDrawer()
    {
       // var config = Config.GetConfigEditor<LevelsConfig>();
        items = new string[] { "COS", "saaa" };
    }

}