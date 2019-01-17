using System;
using UnityEngine;
namespace Engine.UI
{


    public class UIInitializer : MonoBehaviour
    {
        static UIInitializer Instance { get; set; }
        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        public string mainMenuName;

        public static void AddToHierarchy(UIWindow window)
        {
            if (Instance == null) return;
            if(window.transform.parent != Instance.transform)
                window.transform.SetParent(Instance.transform);
        }
    }
}