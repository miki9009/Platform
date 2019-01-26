using UnityEngine;

using Engine.UI;
using UnityEngine.SceneManagement;
using Engine;

public class WorldWindow: UIWindow
{
    public WorldCameraMovement movement;
    public GameObject backToFullViewButton;

    public override void Start()
    {
        base.Start();
        Level.LevelLoaded += Level_LevelLoaded;    
    }

    private void OnDestroy()
    {
        Level.LevelLoaded -= Level_LevelLoaded;
    }

    private void Level_LevelLoaded()
    {
        if(Visible && SceneManager.GetActiveScene().name != "World")
        {
            Console.WriteLine("Hiding World.", Console.LogColor.Lime);
            Hide();
        }
    }

    public void GoToWorldFromMenu()
    {
        LevelManager.LoadLevelAdditive("World");
    }

    [EventMethod]
    public static void GoToWorldFromPause()
    {
        LevelManager.ReturnToMenu(false);
        LevelManager.LoadLevelAdditive("World");
    }

    public void ReturnFromWorld()
    {
        var window = UIWindow.GetWindow(UIWindow.LOADING_SCREEN);
        if (window != null)
            window.Show();
        SceneManager.UnloadSceneAsync("game");
        SceneManager.UnloadSceneAsync("World");
        LevelManager.LoadMenu3D();
        SceneManager.sceneLoaded += RemoveLoadingScreen;
    }

    void RemoveLoadingScreen(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= RemoveLoadingScreen;
        var window = UIWindow.GetWindow(UIWindow.LOADING_SCREEN);
        if (window != null)
            window.Hide();
    }



}