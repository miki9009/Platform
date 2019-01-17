using Engine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldLevel : MonoBehaviour
{
    [CustomLevelSelector]
    public string customLevel;
    public Light lght;
    public SphereCollider sphereCollider;

    private void Awake()
    {
        GetComponentInParent<MapFocused>().levels.Add(this);
    }

    private void Start()
    {
        World.Instance.Window.movement.PointerUp += WorldPointerUp;
    }

    private void OnDestroy()
    {
        World.Instance.Window.movement.PointerUp -= WorldPointerUp;
    }

    void WorldPointerUp(Vector3 position)
    {
        float dis = Vector3.Distance(transform.position, World.Instance.PointerPosition);
        if (gameObject.activeInHierarchy && dis < sphereCollider.radius)
        {
            Debug.Log("Tapped on " + name);
            Tapped();
        }

    }



    WorldWindow window;
    void Tapped()
    {
        window = UIWindow.GetWindow<WorldWindow>();
        window.Hidden += UnloadWorld;
        window.Hide(); 
    }

    void UnloadWorld()
    {
        window.Hidden -= UnloadWorld;
        try
        {
            Camera.main.gameObject.SetActive(false);
        }
        catch { }
        SceneManager.UnloadSceneAsync("World");
        SceneManager.sceneUnloaded += BeginLoading;
    }

    void BeginLoading(Scene scene)
    {
        SceneManager.sceneUnloaded -= BeginLoading;
        LevelManager.BeginCustomLevelLoadSequenceAdditive(LevelsConfig.GetSceneName(customLevel), LevelsConfig.GetLevelName(customLevel));
    }
}