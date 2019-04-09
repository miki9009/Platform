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
    public class Level : MonoBehaviour
    {
        static LevelsConfig config;

        //public LevelSettings levelSettings;

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
                if(string.IsNullOrEmpty(_sceneName))
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
            while(contains)
            {
                id = UnityEngine.Random.Range(minID, maxID);
                contains = levelElementIDs.ContainsKey(id);
            }
            return id;
        }

        public static bool ContainsID(LevelElement element)
        {
            var elements = GameObject.FindObjectsOfType<LevelElement>();
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

                if(string.IsNullOrEmpty(SceneName))
                {
                    Debug.LogError("Scene Name is empty, did not save");
                    return;
                }

                string levelsPath = Config.levelPaths + "/"+ SceneName;
                string partPath = Application.dataPath + "/Resources/" + levelsPath;
                if (!Directory.Exists(partPath))
                {
                    Directory.CreateDirectory(partPath);
                }
                string savePath = partPath + "/"+ levelName + ".txt";
                Data.Save(savePath, levelElements, true, true);
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

        public static void Clear()
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
            SceneName = scene;
            GameManager.GameMode = MissionsConfig.GetMode(LevelsConfig.GetFullName(scene, levelName));
            Load(levelName);
        }

        public static void Load(string levelName, bool compressed = true)
        {
            var elements = GameObject.FindObjectsOfType<LevelElement>();
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
            string partPath = Config.levelPaths + SceneName;
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

            object data = null;
            if (compressed)
                data = Data.DesirializeFile(bytes, true);
            else
                data = Data.DesirializeFile(bytes);

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

        public static void ReloadIDs()
        {
            var elements = GameObject.FindObjectsOfType<LevelElement>();
            foreach (var element in elements)
            {
                element.elementID = GetID();
            }
        }
#if UNITY_EDITOR
        public static void Play()
        {
            EditorApplication.ExecuteMenuItem("Edit/Play");
        }
#endif

        public static void StartLevelSequence()
        {
            Debug.Log("Loading...");
            SceneManager.sceneLoaded -= LoadInit;
            SceneManager.sceneLoaded += LoadInit;
        }

        static void LoadInit(Scene scene, LoadSceneMode mode)
        {
            if(scene.name == "init")
            {
                return;
            }
            else
            {
                SceneManager.sceneLoaded -= LoadInit;
                SceneManager.sceneLoaded += LoadMenu;
                SceneManager.LoadSceneAsync("init", LoadSceneMode.Single);
            }
        }

        static void LoadMenu(Scene scene, LoadSceneMode mode)
        {
            if(scene.name == "menu")
            {
                SceneManager.sceneLoaded -= LoadMenu;
                CoroutineHost.Start(TestLevelSequence());
            }
        }

        static IEnumerator TestLevelSequence()
        {
            bool loaded = false;
            while(!loaded)
            {
                if(SceneManager.GetSceneByName("menu3D").isLoaded)
                {
                    LevelManager.BeginCustomLevelLoadSequenceAdditive(Config.selectedScene, Config.selectedLevel);
                    loaded = true;
                }
                yield return null;
            }
            loaded = false;
            while (!loaded)
            {
                if (SceneManager.GetSceneByName(Config.selectedScene).isLoaded)
                {
                    UI.UIWindow.GetWindow("MainMenu").Hide();
                    loaded = true;
                }
                yield return null;
            }
        }
    }
#if UNITY_EDITOR
    public class ApplicationInitialization
    {
        [UnityEditor.InitializeOnLoadMethod]
        static void OnInit()
        {
            try
            {

                //Debug.Log("startApp: " + startApplication);

                if(Level.Config.testLevel)
                {
                    Debug.Log("Testing Level");
                    Level.StartLevelSequence();
                }

            }
            catch
            {

            }


        }
    }
#endif
}
