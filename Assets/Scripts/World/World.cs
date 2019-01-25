using UnityEngine;
using Engine;
using Engine.UI;
using System;
using UnityEngine.SceneManagement;

public class World : MonoBehaviour
{
    public static event Action Initialized;
    public static World Instance { get; private set; }

    public WorldWindow Window { get; private set; }
    [CustomLevelSelector]
    public string customLevel;

    static string levelName;

    bool showFps;
    private void Awake()
    {
        Instance = this;
        levelName = customLevel;
        Window = UIWindow.GetWindow<WorldWindow>(); 
        if(Window!=null)
        {
            Debug.Log("Initialized world");
        }

        Character.CharacterCreated += InitCharacter;

        if(DataManager.Exists())
            showFps = DataManager.Settings.showFps;
        Initialized?.Invoke();
    }

    private void OnDestroy()
    {
        Character.CharacterCreated -= InitCharacter;
    }

    void InitCharacter(Character character)
    {
        character.transform.localScale = new Vector3(2, 2, 2);
    }

    [EventMethod]
    public static void BackToWorld()
    {
            LevelManager.ChangeLevel(LevelsConfig.GetSceneName("World"), LevelsConfig.GetLevelName(levelName));
    }

    private void OnGUI()
    {
       // Draw.TextColor(10, 300, 255, 0, 0, 1, "Mouse pos: " + PointerPosition);
        if (showFps)
        {
            Draw.DisplayFpsMedian(Screen.width / 2, 10, Color.red, 40);

        }
    }
}