using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Engine
{
    public class Scenery : MonoBehaviour
    {
        public string Path
        {
            get
            {
                return GetName();
            }
        }

        public int elementID = -1;
        public Dictionary<string, object> data;


        public string GetName()
        {

#if UNITY_EDITOR
            string rawPath = AssetDatabase.GetAssetPath(PrefabUtility.GetCorrespondingObjectFromSource(gameObject)).Substring(17);
            rawPath = rawPath.Substring(0, rawPath.Length - 7);
            return rawPath ;//.Trim(("Assets/Resources/").ToCharArray())
#endif
            Debug.LogError("Path not found of Prefab doesn't exist, or Trying to get path during gameplay.");
            return "";
        }

        public virtual void OnSave()
        {
            if (elementID > Level.maxID)
            {
                elementID = Level.GetID();
            }
            data = new Dictionary<string, object>();
            Vector pos = transform.position;
            data.Add("Position", pos);
            Float4 rotation = transform.rotation;
            data.Add("Rotation", rotation);
            Vector scale = transform.localScale;
            data.Add("Scale", scale);
            data.Add("ID", elementID);
        }

        public virtual void OnLoad()
        {
            GameManager.LevelClear += OnLevelClear;
            transform.position = (Vector)data["Position"];
            transform.rotation = (Float4)data["Rotation"];
            transform.localScale = (Vector)data["Scale"];
            elementID = (int)data["ID"];

//#if UNITY_EDITOR
//            name += " (" + elementID + ")";
//#endif

        }

        protected virtual void OnLevelClear()
        {
            if (this == null || gameObject == null) return;
            Destroy(gameObject);
        }

    }
}