using System;
using UnityEngine;
using Engine;
using Engine.Config;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

[SerializableAttribute]
public class ConfigsDrawer : PopUpAttribute
{
    public ConfigsDrawer()
    {
        var list = new List<string>();
        Type config = typeof(Config);
        Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => config.IsAssignableFrom(t))
            .ToList()
            .ForEach(x => list.Add(x.Name));
        items = list.ToArray();
    }
}
