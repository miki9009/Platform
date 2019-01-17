using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Engine
{
    [ExecuteInEditMode]
    public class Level : MonoBehaviour
    {
        static LevelsConfig config;

        public static LevelsConfig Config
        {
            get
            {
                if(config == null)
                {
                    config = Engine.Config.Config.GetConfigEditor<LevelsConfig>(LevelsConfig.key);
                }
                return config;
            }
        }

        public static event Action LevelLoaded;


        public static string LevelElementsPath
        {
            get
            {
                return Config.levelElementsPath;
            }
        }
        public static Dictionary<int, LevelElement> loadedElements = new Dictionary<int, LevelElement>();
        static Dictionary<object, string> levelElements;
        [CustomLevelSelector]
        public string levelName;

        public static string sceneName;

        static Dictionary<int, string> levelElementIDs = new Dictionary<int, string>();

        public static int GetID()
        {
            bool contains = true;
            int id = -1;
            int maxID = 9999999;
            int i = maxID;
            while(contains && maxID > 0)
            {
                id = UnityEngine.Random.Range(0, maxID);
                contains = levelElementIDs.ContainsKey(id);
                maxID--;
            }
            if (maxID == 0)
                Debug.LogError("All ID's used");
            return id;
        }

        public static void RemoveID(int id)
        {
            if (levelElementIDs.ContainsKey(id))
                levelElementIDs.Remove(id);
        }

        public static void Save(string levelName)
        {
            var ids = new Dictionary<int, bool>();
            levelElements = new Dictionary<object, string>();
            var elements = GameObject.FindObjectsOfType<LevelElement>();
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

                if(string.IsNullOrEmpty(sceneName))
                {
                    Debug.LogError("Scene Name is empty, did not save");
                    return;
                }

                string levelsPath = Config.levelPaths + "/"+ sceneName;
                string partPath = Application.dataPath + "/Resources/" + levelsPath;
                if (!Directory.Exists(partPath))
                {
                    Directory.CreateDirectory(partPath);
                }
                string savePath = partPath + "/"+ levelName + ".txt";
                Data.Save(savePath, levelElements, true);
            }
            catch(Exception ex)
            {
                Debug.LogError(ex);
                return;
            }
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif
            foreach (var element in elements)
            {
                if(element!=null && element.gameObject!=null)
                    DestroyImmediate(element.gameObject);
            }
            ClearIDs();
        }

        public void Clear()
        {
            var elements = GameObject.FindObjectsOfType<LevelElement>();
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

        public static void LoadWithScene(string scene, string levelName)
        {
            sceneName = scene;
            Load(levelName);
        }

        public static void Load(string levelName)
        {
            var elements = GameObject.FindObjectsOfType<LevelElement>();
#if UNITY_EDITOR
            var levelComponent = GameObject.FindObjectOfType<Level>();
            if (levelComponent != null)
                levelComponent.gameObject.name = sceneName + ":"+ levelName;
            else
                Debug.LogError("No Level Component found");
#endif
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
            string partPath = Config.levelPaths + sceneName;
            ClearIDs(); //CLEAR IDS
            string assetPath = partPath +"/" + levelName;
            TextAsset asset = Resources.Load(assetPath) as TextAsset;
            if(asset == null)
            {
                Debug.Log("Level was null");
                return;
            }
            Debug.Log("Path: " + assetPath);
            var bytes = asset.bytes;
            var data = Data.DesirializeFile(bytes);
            levelElements = (Dictionary<object, string>)data;
            loadedElements.Clear();
            foreach (var element in levelElements)
            {
                string path = Config.levelElementsPath + element.Value;
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
                    var levelElement = obj.GetComponent<LevelElement>();
                    if(levelElement!=null)
                    {
                        levelElement.data = (Dictionary<string, object>)element.Key;
                        levelElement.OnLoad();
                        if (!loadedElements.ContainsKey(levelElement.elementID))
                            loadedElements.Add(levelElement.elementID, levelElement);
                        else
                            Debug.LogError("EXCEPTION Caught: element with ID: " + levelElement.elementID + " already exists!");
                    }              
                }
                else
                {
                    Debug.LogError("Object was null, make sure it was in the LevelElements folder set in the config");
                }
            }
            foreach (var levelElement in loadedElements.Values)
            {
                levelElement.BuildHierarchy();
            }
            if(Application.isPlaying)
            {
                foreach (var levelElement in loadedElements.Values)
                {
                    levelElement.ElementStart();
                }
            }

            Console.WriteLine("LevelLoaded", Console.LogColor.Green);
            LevelLoaded?.Invoke();
        }
    }
}