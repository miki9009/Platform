using Engine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuThreeDe : MonoBehaviour
{
    public Material mat;
    public Color materialColor = Color.white;
    public Camera cam;
    static MenuThreeDe instance;
    private void Start()
    {
        instance = this;
        mat.color = materialColor;
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Menu3D"));
        var window = UIWindow.GetWindow(UIWindow.LOADING_SCREEN);
        if(window!= null)
        {
            window.Hide();
        }
    }

    [EventMethod]
    public static void RefreshCamera()
    {
        if(instance)
        {
            instance.cam.gameObject.SetActive(false);
            instance.cam.gameObject.SetActive(true);
        }
    }
}