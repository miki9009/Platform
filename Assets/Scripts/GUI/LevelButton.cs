using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelButton : MonoBehaviour
{
    [CustomLevelSelector]
    public string customLevel;

    public string customScene;


    public void GoToLevelAdditive()
    {
        try
        {
            Camera.main.gameObject.SetActive(false);
        }
        catch { }
        WorldWindow.HideWorldWindow();
        string sceneName = LevelsConfig.GetSceneName(customLevel);
        if (customScene == "none")
            customScene = null;
        LevelManager.BeginCustomLevelLoadSequenceAdditive(sceneName, LevelsConfig.GetLevelName(customLevel), customScene);
    }

}