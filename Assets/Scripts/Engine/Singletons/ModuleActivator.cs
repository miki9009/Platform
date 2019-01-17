using System.Reflection;
using UnityEngine;
using System.Linq;
using System;
using System.Collections.Generic;

namespace Engine.Singletons
{
    public class ModuleActivator : MonoBehaviour
    {
        static List<Module> modules = new List<Module>();

        public static void AddModule(Module module)
        {
            modules.Add(module);
        }

        public static T GetModule<T>() where T : Module
        {
            for (int i = 0; i < modules.Count; i++)
            {
                if (modules[i].GetType() == typeof(T))
                    return (T)modules[i];
            }
            return null;
        }

        private void Awake()
        {
            var singletons = Assembly
             .GetAssembly(typeof(Module))
             .GetTypes()
             .Where(t => t.IsSubclassOf(typeof(Module)));

            foreach (var module in singletons)
            {
                Activator.CreateInstance(module);
            }
        }
    }
}