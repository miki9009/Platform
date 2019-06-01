using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Engine
{
    public class Scenery : MonoBehaviour
    {
        public CustomScene.SceneryContainer sceneryContainer = CustomScene.SceneryContainer.Level;

        public Dictionary<string, object> data;
        public bool castShadow = true;
        public bool recieveShadow = true;
        public string overridePath;

        public string GetName()
        {
            if(!string.IsNullOrEmpty(overridePath))
            {
                return overridePath;
            }
#if UNITY_EDITOR
            string rawPath = AssetDatabase.GetAssetPath(PrefabUtility.GetCorrespondingObjectFromSource(gameObject));
            if(rawPath.Length > 17)
            {
                rawPath = rawPath.Substring(17);
                rawPath = rawPath.Substring(0, rawPath.Length - 7);
                return rawPath;//.Trim(("Assets/Resources/").ToCharArray())
            }

#endif
            Debug.LogError("Path not found of Prefab doesn't exist, or Trying to get path during gameplay.");
            return "";
        }

        public virtual void OnSave()
        {
#if UNITY_EDITOR
            var origin = PrefabUtility.GetCorrespondingObjectFromSource(gameObject).GetComponent<Scenery>();
            if(origin == null)
            {
                Debug.LogError("Prefab not found for: " + name);
            }
            data = new Dictionary<string, object>();

            Vector pos = transform.position;
            data.Add("Position", pos);

            Float4 rotation = transform.rotation;
            data.Add("Rotation", rotation);

            Vector scale = transform.localScale;
            data.Add("Scale", scale);

            if (origin.sceneryContainer != sceneryContainer)
                data.Add("Container", sceneryContainer);
            if(!castShadow)
            {
                data.Add("CastShadow", castShadow);
                Debug.Log("Cast Shadow load;");
            }
#endif
        }

        public virtual void OnLoad()
        {
            transform.position = (Vector)data["Position"];
            transform.rotation = (Float4)data["Rotation"];
            transform.localScale = (Vector)data["Scale"];
            if(data.ContainsKey("Container"))
            {
                sceneryContainer = (CustomScene.SceneryContainer)data["Container"];
            }
            if (data.ContainsKey("RecieveShadow"))
            {
                var renderers = GetComponentsInChildren<MeshRenderer>();
                recieveShadow =  (bool) data["RecieveShadow"];
                foreach (var rend in renderers)
                {
                    rend.receiveShadows = recieveShadow;
                }
            }
            if(data.ContainsKey("CastShadow"))
            {
                var renderers = GetComponentsInChildren<MeshRenderer>();
                castShadow = (bool)data["CastShadow"];

                foreach (var rend in renderers)
                {
                    if (castShadow)
                        rend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                    else
                    {
                        rend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    }

                }
            }


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