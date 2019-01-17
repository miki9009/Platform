using Engine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuThreeDe : MonoBehaviour
{
    public Material mat;
    public Color materialColor = Color.white;
    private void Start()
    {
        mat.color = materialColor;
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Menu3D"));
        var window = UIWindow.GetWindow(UIWindow.LOADING_SCREEN);
        if(window!= null)
        {
            window.Hide();
        }
    }
}