using UnityEngine;
using Engine;
using Engine.UI;
using System;
using UnityEngine.SceneManagement;

public class World : MonoBehaviour
{
    public static event Action Initialized;
    public LayerMask collisionLayer;
    public static World Instance { get; private set; }

    public Camera worldCamera;
    public Transform pointerObject;

    public WorldWindow Window { get; private set; }

    private BoxCollider _box;
    public Bounds PointerBounds
    {
        get
        {
            return _box.bounds;
        }
    }

    bool showFps;
    private void Awake()
    {
        _box = pointerObject.GetComponent<BoxCollider>();
        Instance = this;
        Window = UIWindow.GetWindow<WorldWindow>(); 
        if(Window!=null)
        {
            Debug.Log("Initialized world");
        }

        if(DataManager.Exists())
            showFps = DataManager.Settings.showFps;
        Initialized?.Invoke();
    }

    private void Start()
    {
        GameManager.OnLevelClear();
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("World"));
        var window = UIWindow.GetWindow(UIWindow.LOADING_SCREEN);
        if (window != null)
            window.Hide();
        BackToFullViewButtonEnable(false);

    }

    public void BackToFullViewButtonEnable(bool val)
    {
        Window.backToFullViewButton.SetActive(val);
    }

    public Vector3 PointerPosition { get; private set; }
    private void Update()
    {
        PointerPosition = Engine.Mouse.GetMouse(worldCamera, collisionLayer.value, QueryTriggerInteraction.Ignore);
        pointerObject.position = PointerPosition;
    }

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(PointerPosition, 2);
    }
#endif

    private void OnGUI()
    {
       // Draw.TextColor(10, 300, 255, 0, 0, 1, "Mouse pos: " + PointerPosition);
        if (showFps)
        {
            Draw.DisplayFpsMedian(Screen.width / 2, 10, Color.red, 40);

        }
    }
}