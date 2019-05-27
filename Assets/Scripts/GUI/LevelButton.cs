using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelButton : MonoBehaviour
{
    public Mission mission;


    public void GoToLevelAdditive()
    {
        try
        {
            Camera.main.gameObject.SetActive(false);
        }
        catch { }
        WorldWindow.HideWorldWindow();
        Debug.Log("Loading mission: " + mission);
        LevelManager.BeginCustomLevelLoadSequenceAdditive(mission);
    }

}