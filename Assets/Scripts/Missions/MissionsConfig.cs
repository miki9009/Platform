using Engine.Config;
using System;
using System.Collections.Generic;
using UnityEngine;
using Engine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = key)]
public class MissionsConfig : Config
{
    public const string key = "Configs/MissionsConfig";
    public List<MissionContainer> containers;

    public string selectedScene;

    static MissionsConfig instance;
    public static MissionsConfig Instance
    {
        get
        {
            if (instance == null)
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                    instance = Config.GetConfigEditor<MissionsConfig>(key);
                else
#endif
                    instance = ConfigsManager.GetConfig<MissionsConfig>();
            }
            return instance;
        }
    }

    public List<Mission> GetMissions(string scene)
    {
        foreach (var container in containers)
        {
            if (container.scene == scene)
                return container.missions;
        }
        Debug.LogError("No missions on scene: " + scene);
        return null;
    }

    public Mission GetMission(string scene, string missionID)
    {
        var missions = GetMissions(scene);
        for (int i = 0; i < containers.Count; i++)
        {
            if (missions[i].level == missionID)
                return missions[i];
        }
        Debug.LogError("Mission with ID " + missionID + " not found");
        return null;
    }

    public Mission GetMission(string missionID)
    {
        var missions = GetMissions(LevelsConfig.GetSceneName(missionID));
        for (int i = 0; i < containers.Count; i++)
        {
            if (missions[i].ToString() == missionID)
                return missions[i];
        }
        Debug.LogError("Mission with ID " + missionID + " not found");
        return null;
    }


    public static string GetMode(Mission mission)
    {
        foreach (var container in Instance.containers)
        {
            if(container.scene == mission.scene)
            {
                foreach (var m in container.missions)
                {
                    if (m.level == mission.level)
                        return m.mode;
                }
            }
        }
        Debug.LogError("Mode not found for mission: " + mission);
        return "";
    }
}

[Serializable]
public class Mission
{
    public string scene;
    public string level;
    //[CustomSceneSelector]
    public string customScene;

    [ModesDrawer]
    public string mode;
    public bool unlocked;

    public bool IsValid
    {
        get
        {
            return !string.IsNullOrEmpty(scene);
        }
    }

    public Mission()
    {

    }

    public Mission(string ID)
    {
        var chain = ID.Split('_');
        if(chain.Length == 3)
        {
            scene = chain[0];
            level = chain[1];
            customScene = chain[2];
        }
    }

    public override string ToString()
    {
        return scene + "_" + customScene + "_" + level;
    }
}

[Serializable]
public class MissionContainer
{
    [LevelSelector]
    public string scene;
    public List<Mission> missions;
}

#if UNITY_EDITOR
[CustomEditor(typeof(MissionsConfig))]
public class MissionsConfigEditor : Editor
{
    LevelSelector levels;
    int sceneIndex;

    bool allMissionFold;
    int selectedScene;
    MissionsConfig config;
    LevelsConfig levelsConfig;
    CustomScenesConfig customSceneConfig;
    bool[] scenesFoldout;

    bool[] missionsFoldout;

    public override void OnInspectorGUI()
    {
        if (!levelsConfig)
        {
            levelsConfig = Config.GetConfigEditor<LevelsConfig>(LevelsConfig.key);
        }
        if (!customSceneConfig)
        {
            customSceneConfig = CustomScene.Config;
        }
        //base.OnInspectorGUI();



        config = (MissionsConfig)target;

        if (levels == null)
            levels = new LevelSelector();

        var missions = config.containers;

        if (scenesFoldout == null || scenesFoldout.Length != config.containers.Count)
        {
            scenesFoldout = new bool[config.containers.Count];
        }

        int count = 0;
        for (int i = 0; i < config.containers.Count; i++)
        {
            count += config.containers[i].missions.Count;
        }

        if (missionsFoldout == null || count != missionsFoldout.Length)
        {
            missionsFoldout = new bool[count];
        }

        int foldoutIndex = 0;

        GUIStyle style = EditorStyles.helpBox;
        var style2 = EditorStyles.foldout;
        style2.margin = new RectOffset(20, 10, 5, 5);
        var defaultColor = GUI.backgroundColor;
        var col = new Color(0.8f, 0.8f, 0.8f);

        EditorGUILayout.BeginVertical(style);

        allMissionFold = EditorGUILayout.Foldout(allMissionFold, "All Missions", true, style2);

        var modes = new ModesDrawer();

        if (allMissionFold)
        {
            EditorGUILayout.LabelField("Scene Name: ");
            selectedScene = EditorGUILayout.Popup(selectedScene, levels.items);

            if (GUILayout.Button("Add new Scene"))
            {
                bool canAdd = true;
                for (int i = 0; i < config.containers.Count; i++)
                {
                    if (config.containers[i].scene == levels.items[selectedScene])
                        canAdd = false;
                }
                if (canAdd)
                {
                    config.containers.Add(new MissionContainer() { scene = levels.items[selectedScene], missions = new List<Mission>() });
                    return;
                }
            }


            EditorGUILayout.BeginVertical(style);

            for (int i = 0; i < config.containers.Count; i++)
            {
                string sceneName = config.containers[i].scene;

                if(scenesFoldout[i])
                {
                    col = new Color(0.5f, 0.5f, 1f);

                }
                else
                {
                    col = new Color(1f, 1f, 1f);
                }

                GUI.backgroundColor = col;

                EditorGUILayout.BeginHorizontal(style);
                scenesFoldout[i] = EditorGUILayout.Foldout(scenesFoldout[i], config.containers[i].scene, true, style2);
                if (GUILayout.Button("X", GUILayout.Width(30)))
                {
                    config.containers.RemoveAt(i);
                }
                EditorGUILayout.EndHorizontal();



                if (scenesFoldout[i])
                {

                    EditorGUILayout.BeginVertical(style);

                    var customLevels = levelsConfig.GetAllLevels(sceneName);
                    var customScenes = customSceneConfig.GetAllCustomScenesForScene(sceneName);

                    if (customScenes == null || customLevels == null) continue;

  
                    for (int j = 0; j < config.containers[i].missions.Count; j++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        missionsFoldout[foldoutIndex] = EditorGUILayout.Foldout(missionsFoldout[foldoutIndex], "Mission ID: " + config.containers[i].missions[j].ToString(), true, style2);

                        if (GUILayout.Button("X", GUILayout.Width(30)))
                        {
                            config.containers[i].missions.RemoveAt(j);
                        }
                        EditorGUILayout.EndHorizontal();

                        //col = new Color(1f, 1f, 1f);
                        //GUI.backgroundColor = col;

                        if (missionsFoldout[foldoutIndex])
                        {
                            EditorGUILayout.BeginVertical(style);
                            config.containers[i].missions[j].scene = sceneName;
                            int customLevelIndex = 0;
                            string customLevelID = config.containers[i].missions[j].level;
                            for (int k = 0; k < customLevels.Count; k++)
                            {
                                if (customLevels[k] == customLevelID)
                                    customLevelIndex = k;
                            }
                            if(customLevels.Count == 0)
                            {
                                EditorGUILayout.LabelField("No custom levels detected.");
                                continue;
                            }
                            if (customLevelIndex > customLevels.Count - 1)
                            {
                                customLevelIndex = 0;
                            };

                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("Custom Level: ");
                            customLevelIndex = EditorGUILayout.Popup(customLevelIndex, customLevels.ToArray());
                            config.containers[i].missions[j].level = customLevels[customLevelIndex];
                            EditorGUILayout.EndHorizontal();

                            int customSceneIndex = 0;
                            string customSceneID = config.containers[i].missions[j].customScene;
                            for (int k = 0; k < customScenes.Count; k++)
                            {
                                if (customScenes[k] == customSceneID)
                                    customSceneIndex = k;
                            }

                            if (customScenes.Count == 0)
                            {
                                var c = GUI.color;
                                GUI.color = Color.red;
                                EditorGUILayout.LabelField("No custom scenes detected.");
                                GUI.color = c;
                                continue;
                            }

                            if (customSceneIndex > customScenes.Count - 1)
                            {
                                customSceneIndex = 0;
                            }

                            EditorGUILayout.BeginHorizontal();
                            
                            EditorGUILayout.LabelField("Custom Scene: ");
                            customSceneIndex = EditorGUILayout.Popup(customSceneIndex, customScenes.ToArray());
                           
                            config.containers[i].missions[j].customScene = customScenes[customSceneIndex];

                            EditorGUILayout.EndHorizontal();

                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("Mode: ");
                            int modeIndex = 0;
                            for (int l = 0; l < modes.items.Length; l++)
                            {
                                if (config.containers[i].missions[j].mode == modes.items[l])
                                    modeIndex = l;
                            }
                            modeIndex = EditorGUILayout.Popup(modeIndex, modes.items);
                            config.containers[i].missions[j].mode = modes.items[modeIndex];
                            EditorGUILayout.EndHorizontal();

                            EditorGUILayout.EndVertical();

                        }

                        foldoutIndex++;
                    }


                    if(GUILayout.Button("Add new mission"))
                    {
                        config.containers[i].missions.Add(new Mission() { scene = sceneName});
                    }
                    

                    EditorGUILayout.EndVertical();
                }
            }


            GUI.backgroundColor = defaultColor;
        }
        EditorGUILayout.EndVertical();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(config);
        }


        //ALL MISSION END
    }
}
#endif