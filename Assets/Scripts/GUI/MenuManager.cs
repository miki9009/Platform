using Engine.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    //public GameObject cam;
    public UIWindow guiWindow;

    private void Start()
    {
        GameManager.GameReady += () => guiWindow.FadeIn();
    }

    
}
