using UnityEngine;

using Engine.UI;
using UnityEngine.SceneManagement;

public class WorldWindow: UIWindow
{
    public WorldCameraMovement movement;
    public GameObject backToFullViewButton;
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