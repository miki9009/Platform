using UnityEngine;

using Engine.UI;
using UnityEngine.SceneManagement;
using Engine;

public class WorldWindow: UIWindow
{
    //public WorldCameraMovement movement;
    //public GameObject backToFullViewButton;

    public static WorldWindow Instance { get; private set; }
    public override void Start()
    {
        base.Start();
        Instance = this;
    }

    public override void OnHidden()
    {
        base.OnHidden();
        Debug.Log("Hiding");
    }

    //private void OnDestroy()
    //{
    //    Level.LevelLoaded -= Level_LevelLoaded;
    //}

    //private void Level_LevelLoaded()
    //{
    //    if(Visible && SceneManager.GetActiveScene().name != "World")
    //    {
    //        Console.WriteLine("Hiding World.", Console.LogColor.Lime);
    //        Hide();
    //    }
    //}

    //public void GoToWorldFromMenu()
    //{
    //    LevelManager.LoadLevelAdditive("World");
    //}

    public void GoToWorldFromPause()
    {
        LevelManager.ReturnToMenu(true);
        //LevelManager.LoadLevelAdditive("World");

    }

    [EventMethod]
    public static void HideWorldWindow()
    {
        Instance.Hide();   
    }

    //public void ReturnFromWorld()
    //{
    //    var window = UIWindow.GetWindow(UIWindow.LOADING_SCREEN);
    //    if (window != null)
    //        window.Show();
    //    SceneManager.UnloadSceneAsync("game");
    //    SceneManager.UnloadSceneAsync("World");
    //    LevelManager.LoadMenu3D();
    //    SceneManager.sceneLoaded += RemoveLoadingScreen;
    //}

    //void RemoveLoadingScreen(Scene scene, LoadSceneMode mode)
    //{
    //    SceneManager.sceneLoaded -= RemoveLoadingScreen;
    //    var window = UIWindow.GetWindow(UIWindow.LOADING_SCREEN);
    //    if (window != null)
    //        window.Hide();
    //}



}