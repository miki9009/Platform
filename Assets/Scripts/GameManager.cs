using Engine;
using Engine.UI;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(999)]
public class GameManager : MonoBehaviour
{
    public static event Action LevelClear;
    public static event Action GameReady;
    public static event Action<string> LevelChanged;
    public static event Action GameFinished;
    public static event Action Restart;
    public static bool IsSceneLoaded { get; private set; }
    public static bool isLevel;
    public static string CurrentLevel { get; set; }
    
    public static GameManager Instance { get; private set; }


    private string currentName;
    public string LevelName
    {
        get
        {
            return SceneManager.GetActiveScene().name;
        }
    }

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
        LevelChanged?.Invoke(LevelName);
    }

    void OnLevelChanged(Scene scene, Scene scene2)
    {
        OnLevelChangedEvent(LevelName);
        Debug.Log("Game Manager: Level Changed to: " + LevelName);
    }


    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Game Manager: Level Loaded: " + LevelName);
        if(scene.name == CurrentLevel)
            StartCoroutine(LevelLoadedCor());
    }

    public static void OnRestart()
    {
        Restart?.Invoke();
    }

    public static void OnLevelClear()
    {
        Console.WriteLine("On Level Clear", Console.LogColor.Green);
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

    public enum GameState
    {
        Idle,
        Completed, 
        Failed
    }

    public static GameState State
    {
        get; private set;
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
        //levelName = SceneManager.GetActiveScene().name;
        //SceneManager.UnloadSceneAsync(levelName);
        //SceneManager.sceneUnloaded += Restart;
        OnLevelClear();
        //Character character = Character.GetLocalPlayer();
        //if (character != null)
        //    Destroy(character.gameObject);
        //if (!string.IsNullOrEmpty(LevelManager.Instance.LastCustomLevel))
        //    Level.LoadWithScene(SceneManager.GetActiveScene().name, LevelManager.Instance.LastCustomLevel);
        LevelManager.LoadOnlyCusomLevel(LevelManager.Instance.LastCustomLevel);
        if (Instance != null)
        {
            OnRestart();
        }
    }



}