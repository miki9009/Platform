using UnityEngine;
using Engine.Config;
using System;
using System.Linq;
using System.Collections.Generic;


[SerializableAttribute]
public class CustomSceneSelector : Engine.PopUpAttribute
{
    CustomScenesConfig config;

    public CustomSceneSelector()
    {
        var list = CustomScenesConfig.GetAllCustomScenes();
        items = list.ToArray();
    }
}