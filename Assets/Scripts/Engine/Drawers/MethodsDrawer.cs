using Engine;
using Engine.Config;
using Engine.Events;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[SerializableAttribute]
public class MethodsDrawer : PopUpAttribute
{
    public MethodsDrawer()
    {
        var list = new List<string>();
        var obj = UnityEngine.Object.FindObjectOfType<MethodInvoker>().gameObject;

        var components = obj.GetComponents<Component>();
        // Iterate over each private or public instance method (no static methods atm)
        foreach (var component in components)
        {
            if (component.GetType().IsSubclassOf(typeof(MonoBehaviour)))
            {
                var type = component.GetType();
                foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
                {
                    if(method.IsPublic)
                    list.Add(method.Name);
                }
            }
        }
        items = list.ToArray();
    }

}