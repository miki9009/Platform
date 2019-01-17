using UnityEngine;
using Engine;
using Engine.Config;
using System.Collections;

public class GameInit : MonoBehaviour
{
    [LevelSelector]
    public string firstScene;
    [LevelSelector]
    public string benchmarkScene;
    public bool disableFirstRunInEditor;

    private void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);        
    }

    private void Start()
    {
        var data = (Settings.Container)DataManager.Instance.GetData(DataManager.Containers.SETTINGS);
#if !UNITY_EDITOR
        if (data.firstStart)
        {
            data.firstStart = false;
            DataManager.SaveData();
        }
#endif
        Init();
        DataManager.LoadData();
    }

    private void OnDestroy()
    {
        DataManager.Loaded -= Init;
    }

    private void Init()
    {
        if (Application.isPlaying)
        {
            DataManager.Loaded += () =>
            {
                var settings = (Settings.Container)DataManager.Instance.GetData(DataManager.Containers.SETTINGS);
#if UNITY_EDITOR
                if (!disableFirstRunInEditor)
                {
                    if (!settings.runBenchmark)
                    {
                        Screen.SetResolution((int)settings.resolution.x, (int)settings.resolution.y, true);
                        LevelManager.GoToSingleScene(firstScene, false);
                    }
                    else
                    {
                        LevelManager.GoToSingleScene(benchmarkScene,false);
                    }
                }
                else
                {
                    LevelManager.GoToSingleScene(firstScene,false);
                    LevelManager.LoadMenu3D();
                }
#else
                if (!settings.runBenchmark)
                {
                    Screen.SetResolution((int)settings.resolution.x, (int)settings.resolution.y, true);
                    LevelManager.GoToSingleScene(firstScene);
                    LevelManager.LoadMenu3D();
                }
                else
                {
                    LevelManager.GoToSingleScene(benchmarkScene);
                }
#endif
            };
        }
    }


}