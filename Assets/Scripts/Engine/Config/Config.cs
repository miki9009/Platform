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


        }

        public virtual void OnAfterDeserialize()
        {
            
        }
    }
}