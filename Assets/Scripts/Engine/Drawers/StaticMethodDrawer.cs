using Engine;
using Engine.Config;
using Engine.Events;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System.Linq;
using Engine.UI;

[SerializableAttribute]
public class StaticMethodDrawer : PopUpAttribute
{
    static List<string> list;
    public StaticMethodDrawer()
    {
        if (list == null)
        {
            list = new List<string>();

            //    var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            //    foreach (var assembly in assemblies)
            //    {
            //        var methods = assembly.GetTypes()
            //            .ToList()
            //            .SelectMany(t => t.GetMethods())
            //            .Where(m => m.GetCustomAttributes(typeof(EventMethodAttribute), false).Length > 0 && m.IsStatic && m.IsPublic)
            //            .ToList();
            //        methods.ForEach(x => list.Add(x.DeclaringType + "." + x.Name));
            //        foreach (var method in methods)
            //        {
            //            Debug.Log(method);
            //        }
            //    }
            //}
            var assembly = AppDomain.CurrentDomain.GetAssemblies().
           SingleOrDefault(x => x.GetName().Name == "Assembly-CSharp");

            if (assembly == null)
            {
                Debug.LogError("Assembly not found, change type");
            }
            else
            {
                var methods = assembly.GetTypes()
                        .ToList()
                        .SelectMany(t => t.GetMethods())
                        .Where(m => m.GetCustomAttributes(typeof(EventMethodAttribute), false).Length > 0 && m.IsStatic && m.IsPublic)
                        .ToList();
                methods.ForEach(x => list.Add(x.DeclaringType + "." + x.Name));
            }
        
        }

        items = list.ToArray();
    }

}