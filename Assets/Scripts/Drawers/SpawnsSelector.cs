using UnityEngine;
using Engine.Config;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

[SerializableAttribute]
public class SpawnsSelector : Engine.PopUpAttribute
{
    LevelsConfig config;
    const string PATH = "/Resources/LevelElements";
    public SpawnsSelector()
    {
#if UNITY_EDITOR
        var info = new DirectoryInfo(Application.dataPath + PATH);
        var fileInfo = info.GetFiles();
        var list = new List<string>();
        foreach (var file in fileInfo)
        {
            if(!file.Name.Contains(".meta"))
            {
                string name = Path.GetFileNameWithoutExtension(file.Name);
                list.Add(name);
            }
        }

        items = list.ToArray();
#endif
        if (items == null)
            items = new string[] { "Empty" };
    }


}