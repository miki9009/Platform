using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace Engine
{
    [ExecuteInEditMode]
    public class CustomScene : MonoBehaviour
    {
        public enum SceneryContainer
        {
            Level,
            WallColliders,
            Triggers,
            Sprites,
            Clouds,
            NavSurface
        }

        static CustomScenesConfig config;
        public static Dictionary<SceneryContainer, Transform> sceneryContainers;

        public static CustomScenesConfig Config
        {
            get
            {
                if (config == null)
                {
                    config = Engine.Config.Config.GetConfigEditor<CustomScenesConfig>(CustomScenesConfig.key);
                }
                return config;
            }
        }



        public static Dictionary<int, Scenery> loadedElements = new Dictionary<int, Scenery>();
        static Dictionary<object, string> sceneryElements;
        public static Dictionary<object, string> LevelElements
        {
            get
            {
                return sceneryElements;
            }
        }

        public static string levelName;

        static string _sceneName;
        public static string SceneName
        {
            get
            {
                if (string.IsNullOrEmpty(_sceneName))
                {
                    _sceneName = Config.sceneName;
                }
                return _sceneName;
            }

            set
            {
                _sceneName = value;
                Config.sceneName = _sceneName;
            }
        }

        public static void Save(string scene, string customScene)
        {
            var ids = new Dictionary<int, bool>();
            sceneryElements = new Dictionary<object, string>();
            var elements = GameObject.FindObjectsOfType<Scenery>();
            try
            {
                foreach (var element in elements)
                {
                    element.OnSave();
                    sceneryElements.Add(element.data, element.GetName());
                }

                if (string.IsNullOrEmpty(SceneName))
                {
                    Debug.LogError("Scene Name is empty, did not save");
                    return;
                }


                string partPath = Application.dataPath + "/" + Config.customScenesPath + scene;

                if (!Directory.Exists(partPath))
                {
                    Directory.CreateDirectory(partPath);
                }
                string savePath = partPath + "/" + customScene + ".txt";
                Debug.Log("Save to Path: " + savePath);
                Data.Save(savePath, sceneryElements, true, true);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return;
            }
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif
            //foreach (var element in elements)
            //{
            //    //Debug.Log(element.name);
            //    if (element != null && element.gameObject != null)
            //        DestroyImmediate(element.gameObject);
            //}
            //ClearContainers();
        }

        public static void ClearContainers()
        {
#if UNITY_EDITOR
            if (sceneryContainers == null) return;
            foreach (var item in sceneryContainers)
            {
                if (item.Value)
                    DestroyImmediate(item.Value.gameObject);
            }
            sceneryContainers.Clear();
#endif
        }

        public static void Clear()
        {
            Debug.Log("Clear Custom Scene");
            var elements = GameObject.FindObjectsOfType<Scenery>();
            foreach (var element in elements)
            {
                if (element != null && element.gameObject != null)
                    DestroyImmediate(element.gameObject);
            }
            ClearContainers();
        }

        public static void Load(string scene, string customScene, bool compressed = true)
        {
            Clear();
            if (sceneryContainers == null)
                sceneryContainers = new Dictionary<SceneryContainer, Transform>();
            var enums = Enum.GetValues(typeof(SceneryContainer));
            foreach (var element in enums)
            {
                var type = (SceneryContainer)element;
                if (!sceneryContainers.ContainsKey(type))
                {
                    sceneryContainers.Add((SceneryContainer)element, new GameObject(type.ToString()).transform);
                }
                if(sceneryContainers[type] == null)
                {
                    sceneryContainers[type] = new GameObject(type.ToString()).transform;
                }
            }

            var elements = GameObject.FindObjectsOfType<Scenery>();
            if (Application.isPlaying)
            {
                for (int i = 0; i < elements.Length; i++)
                {
                    Destroy(elements[i].gameObject);
                }
            }
            else
            {
                for (int i = 0; i < elements.Length; i++)
                {
                    DestroyImmediate(elements[i].gameObject);
                }
            }
            string partPath = "CustomScenes/" + scene;
            string assetPath = partPath + "/" + customScene;
            Debug.Log("Loading from Path: " + assetPath);
            TextAsset asset = Resources.Load(assetPath) as TextAsset;
            if (asset == null)
            {
                Debug.Log("Scenery was null");
                return;
            }

            var bytes = asset.bytes;

            object data = null;
            if (compressed)
                data = Data.DesirializeFile(bytes, true);
            else
                data = Data.DesirializeFile(bytes);

            sceneryElements = (Dictionary<object, string>)data;
            loadedElements.Clear();
            foreach (var element in sceneryElements)
            {
                //string path = Config.levelElementsPath + element.Value;
                string path = element.Value;
                var res = Resources.Load(path) as GameObject;
                GameObject obj = null;

                if (Application.isPlaying)
                    obj = Instantiate(res);
                else
                {
#if UNITY_EDITOR
                    obj = (GameObject)PrefabUtility.InstantiatePrefab(res);               
#endif
                }
                if (obj != null)
                {              
                    var sceneryElement = obj.GetComponent<Scenery>();
#if UNITY_EDITOR
                    try
                    {
                        obj.transform.SetParent(sceneryContainers[sceneryElement.sceneryContainer]);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError(ex.Message);
                    }
#else
                     obj.transform.SetParent(sceneryContainers[sceneryElement.sceneryContainer]);
#endif
                    if (sceneryElement != null)
                    {
                        sceneryElement.data = (Dictionary<string, object>)element.Key;
                        sceneryElement.OnLoad();
                    }
                }
                else
                {
                    Debug.LogError("Object was null, make sure it was in the LevelElements folder set in the config");
                }
            }
            if (Application.isPlaying)
            {
                var level = sceneryContainers[SceneryContainer.NavSurface];
                var navSurface = level.gameObject.AddComponent<UnityEngine.AI.NavMeshSurface>();
                
                navSurface.BuildNavMesh();
                foreach (var rootTransform in sceneryContainers.Values)
                {
                    StaticBatchingUtility.Combine(rootTransform.gameObject);
                }
            }

            Engine.Log.Print("CustomSceneLoaded", Engine.Log.Color.Green);
            GameManager.OnCustomSceneLoaded();
        }
    }
}
