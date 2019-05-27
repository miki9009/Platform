using System;
using UnityEngine;


namespace Engine.Config
{
    [CreateAssetMenu(menuName = PATH + FILENAME)]
    public abstract class Config : ScriptableObject, ISerializationCallbackReceiver
    {
        public const string PATH = "Configs/";
        public const string FILENAME = "Base";


        protected string Name { get; set; }

        //public static T GetConfig<T>() where T : Config
        //{
        //    return (T)Resources.Load(PATH + typeof(T));
        //}

        public static T GetConfigEditor<T>(string key) where T : Config
        {
            return (T)Resources.Load(key);
        }

        //public static T GetConfig<T>() where T : Config
        //{
        //    var configs = Resources.FindObjectsOfTypeAll<T>();
        //    if(configs.Length > 1)
        //    {
        //        Debug.LogError("There are more than 1 configs of type " + typeof(T));
        //    }
        //    if(configs.Length > 0)
        //        return configs[0];
        //    Debug.LogError("Config not found of type:  " + typeof(T));
        //    return null;
        //}

        //        public static T GetConfigEditor<T>() where T : Config
        //        {
        //#if UNITY_EDITOR
        //            return UnityEditor.AssetDatabase.LoadAssetAtPath<T>("Assets/Resources/" + PATH + typeof(T) +".asset");
        //#endif
        //        }

        public virtual void OnBeforeSerialize()
        {
            //            if (Name != GetType().ToString())
            //            {
            //#if UNITY_EDITOR
            //                UnityEditor.AssetDatabase.RenameAsset(UnityEditor.AssetDatabase.GetAssetPath(this), GetType().ToString());
            //#endif
            //                Name = name;
            //            }
//#if UNITY_EDITOR
//            UnityEditor.AssetDatabase.Refresh();
//#endif

        }

        public virtual void OnAfterDeserialize()
        {
            
        }
    }
}