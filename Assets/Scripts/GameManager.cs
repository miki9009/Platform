using Engine;
using Engine.UI;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(999)]
public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        Idle,
        Completed,
        Failed,
        Paused
    }

    public static event Action LevelClear;
    public static event Action GameReady;
    public static event Action<string> LevelChanged;
    public static event Action GameFinished;
    public static event Action Restart;
    public static event Action<string> GameModeChanged;
    public static event Action CustomSceneLoaded;
    public static bool IsSceneLoaded { get; private set; }
    public static bool isLevel;
    public static string CurrentScene { get; set; }
    public static Mission CurrentMission { get; set; }
    public static int MissionIndex { get; set; }

    static string _gameMode;
    public static string GameMode
    {
        get
        {
            if (string.IsNullOrEmpty(_gameMode)) return "";
            return _gameMode;
        }
        set
        {
            _gameMode = value;
            GameModeChanged?.Invoke(value);
        }
    }
    static GameState prevState;
    static GameState _state;
    public static GameState State
    {
        get
        {
            return _state;
        }
        set
        {
            if (value != prevState)
                prevState = _state;
            _state = value;
        }
    }

    static bool _isPaused;
    public static bool IsPaused
    {
        get
        {
            return _isPaused;
        }
        set
        {
            _isPaused = value;
            if(_isPaused)
            {
                State = GameState.Paused;
            }
            else
            {
                if (State == GameState.Paused)
                    State = prevState;
            }
        }
    }

    public static string CustomLevelID
    {
        get
        {
            return LevelManager.CurrentCustomLevel;
        }
    }

    public static string CustomScene
    {
        get
        {
            return LevelManager.CurrentCustomScene;
        }
    }
    
    public static GameManager Instance { get; private set; }


    private string currentName;

    private void Awake()
    {
        Instance = this;
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.activeSceneChanged += OnLevelChanged;
        Level.LevelLoaded += OnLevelLoaded;
    }

    void OnLevelLoaded()
    {
        State = GameState.Idle;
    }

    private void Start()
    {
        IsSceneLoaded = true;
    }

    private void OnDestroy()
    {
        Level.LevelLoaded -= OnLevelLoaded;
    }

    public void OnLevelChangedEvent(string levelName)
    {
        LevelChanged?.Invoke(CurrentScene);
    }

    void OnLevelChanged(Scene scene, Scene scene2)
    {
        OnLevelChangedEvent(CurrentScene);
        Debug.Log("Game Manager: Level Changed to: " + CurrentScene);
    }

   

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Game Manager: Level Loaded: " + CurrentScene);
        if(scene.name == CurrentScene)
            StartCoroutine(LevelLoadedCor());
    }

    public static void OnRestart()
    {
        Restart?.Invoke();
    }

    public static void OnLevelClear()
    {
        Engine.Log.Print("On Level Clear", Engine.Log.Color.Green);
        LevelClear?.Invoke();
    }

    public static void OnGameReady()
    {
        if (GameReady != null)
        {
            Debug.Log("Scene Loaded: " + SceneManager.GetActiveScene().name);
            GameReady();
        }
    }

    IEnumerator LevelLoadedCor()
    {
        if (Controller.Instance == null) yield break;
        yield return Engine.Game.WaitForFrames(1);

        Resources.UnloadUnusedAssets();
        if(!PhotonManager.IsMultiplayer)
            OnGameReady();
        yield return null;
    }

    public void EndGame(GameState state)
    {
        State = state;
        OnGameFinished();
        UIWindow.GetWindow("EndGameScreen").Show();
    }


    void OnGameFinished()
    {
        OnLevelClear();
        var data = DataManager.Collections;
        var collectionManger = CollectionManager.Instance;
        int localID = Character.GetLocalPlayer().ID;
        data.coins += collectionManger.GetCollection(localID, CollectionType.Coin);
        data.emmeralds += collectionManger.GetCollection(localID, CollectionType.Emmerald);
        data.goldKeys += collectionManger.GetCollection(localID, CollectionType.KeyGold);
        data.silverKeys += collectionManger.GetCollection(localID, CollectionType.KeySilver);
        data.bronzeKeys += collectionManger.GetCollection(localID, CollectionType.KeyBronze);
        data.restarts += collectionManger.GetCollection(localID, CollectionType.Restart);

        DataManager.SaveData();
        Debug.Log("Game Saved");



        GameFinished?.Invoke();
    }

    public void RestartLevel()
    {
        CoroutineHost.Start(RestartLevelCoroutine());
    }

    IEnumerator RestartLevelCoroutine()
    {
        OnLevelClear();

        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        LevelManager.LoadOnlyCusomLevel(LevelManager.CurrentCustomLevel);

        if (Instance != null)
        {
            OnRestart();
        }
    }

    public static void OnCustomSceneLoaded()
    {
        CustomSceneLoaded?.Invoke();
    }


    //private void OnGUI()
    //{
    //    var characters = Character.allCharacters;
    //    if(characters!=null && characters.Count > 0)
    //    {
    //        for (int i = 0; i < characters.Count; i++)
    //        {
    //            Draw.TextColor(10, 50 * i, 255, 0, 0, 1, characters[i].gameProgress.CurrentWaypoint);
    //        }
    //    }
    //}


}