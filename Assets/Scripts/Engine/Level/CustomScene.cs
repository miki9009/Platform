using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Engine
{
    [ExecuteInEditMode]
    public class CustomScene : MonoBehaviour
    {
        static CustomScenesConfig config;

        //public LevelSettings levelSettings;

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

        public static event Action LevelLoaded;


        //public static string LevelElementsPath
        //{
        //    get
        //    {
        //        return Config.levelElementsPath;
        //    }
        //}
        public static Dictionary<int, Scenery> loadedElements = new Dictionary<int, Scenery>();
        static Dictionary<object, string> levelElements;
        public static Dictionary<object, string> LevelElements
        {
            get
            {
                return levelElements;
            }
        }
        //[CustomLevelSelector]
        public static string levelName;

        static string _sceneName;
        public static string SceneName
        {
            get
            {
                if (string.IsNullOrEmpty(_sceneName))
                {
                    _sceneName = Config.selectedScene;
                }
                return _sceneName;
            }

            set
            {
                _sceneName = value;
                Config.selectedScene = _sceneName;
            }
        }

        static Dictionary<int, string> levelElementIDs = new Dictionary<int, string>();
        public static int maxID = -2;
        public static int GetID()
        {
            bool contains = true;
            int id = -1;
            int minID = -9999999;
            int i = maxID;
            while (contains)
            {
                id = UnityEngine.Random.Range(minID, maxID);
                contains = levelElementIDs.ContainsKey(id);
            }
            return id;
        }

        public static bool ContainsID(Scenery element)
        {
            var elements = GameObject.FindObjectsOfType<Scenery>();
            for (int i = 0; i < elements.Length; i++)
            {
                if (elements[i].elementID == element.elementID && elements[i] != element)
                    return true;
            }
            return false;
        }

        public static void RemoveID(int id)
        {
            if (levelElementIDs.ContainsKey(id))
                levelElementIDs.Remove(id);
        }

        public static void Save(string scene, string customScene)
        {
            var ids = new Dictionary<int, bool>();
            levelElements = new Dictionary<object, string>();
            var elements = GameObject.FindObjectsOfType<Scenery>();
            try
            {
                foreach (var element in elements)
                {
                    if (ids.ContainsKey(element.elementID))
                        element.elementID = GetID();
                    ids.Add(element.elementID, true);
                    element.OnSave();
                    levelElements.Add(element.data, element.GetName());
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
                Data.Save(savePath, levelElements, true, true);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return;
            }
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif
            foreach (var element in elements)
            {
                if (element != null && element.gameObject != null)
                    DestroyImmediate(element.gameObject);
            }
            ClearIDs();
        }

        public static void Clear()
        {
            var elements = GameObject.FindObjectsOfType<Scenery>();
            foreach (var element in elements)
            {
                if (element != null && element.gameObject != null)
                    DestroyImmediate(element.gameObject);
            }
            ClearIDs();
        }

        static void ClearIDs()
        {
            levelElementIDs.Clear();
        }

        public static void Load(string scene, string customScene, bool compressed = true)
        {
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
            ClearIDs(); //CLEAR IDS
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

            levelElements = (Dictionary<object, string>)data;
            loadedElements.Clear();
            GameObject root = new GameObject("Level: " + customScene);
            var rootTransform = root.transform;
            rootTransform.position = new Vector3(0, 0, 0);
            foreach (var element in levelElements)
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
                    obj.transform.SetParent(rootTransform);
                    var levelElement = obj.GetComponent<Scenery>();
                    if (levelElement != null)
                    {
                        levelElement.data = (Dictionary<string, object>)element.Key;
                        levelElement.OnLoad();
                        if (!loadedElements.ContainsKey(levelElement.elementID))
                            loadedElements.Add(levelElement.elementID, levelElement);
                        else
                            Debug.LogError("EXCEPTION Caught: scenery with ID: " + levelElement.elementID + " already exists!");
                    }
                }
                else
                {
                    Debug.LogError("Object was null, make sure it was in the LevelElements folder set in the config");
                }
            }
            if(root)
                StaticBatchingUtility.Combine(root);
            Console.WriteLine("CustomSceneLoaded", Console.LogColor.Green);
            LevelLoaded?.Invoke();
        }

        public static void ReloadIDs()
        {
            var elements = GameObject.FindObjectsOfType<Scenery>();
            foreach (var element in elements)
            {
                element.elementID = GetID();
            }
        }
    }
}
