using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;
using Engine;
using Engine.UI;
using Engine.Config;
using System;
using UnityEngine.Experimental.Rendering;

[DefaultExecutionOrder(-100)]
public class Controller : MonoBehaviour
{
    public float downFactor = 100;
    public BloomOptimized bloom;
    public Antialiasing antialiasing;
    public enum GameType { Perspective, Ortographic}
    public GameType gameType = GameType.Perspective;
    public GameObject gameUI;
    Vector2 defaultResolution;
    public Color levelColor;
    public Checkpoint LastCheckpoint { get; set; }
    public int restarts = 5;
    public static Controller Instance
    {
        get;
        private set;
    }
    public bool IsRestarting { get; private set; }

    static SpawnsConfig spawnsConfig;
    public static SpawnsConfig SpawnsConfig
    {
        get
        {
            if(spawnsConfig == null)
            {
                spawnsConfig = ConfigsManager.GetConfig<SpawnsConfig>();
            }
            return spawnsConfig;
        }
    }

    public float aspectRatio = 1;

    public bool ButtonMovement { get; set; }
    bool showFps;

    [HideInInspector] public Character character;

    [HideInInspector] public List<Character> characters = new List<Character>();

    public GameCamera gameCamera;
    [HideInInspector] public GameObject GUI;
    Vector2 startResolution;


    public VignetteAndChromaticAberration chromaticAberration;
    public MotionBlur motionBlur;
    public Vortex vortex;

    private void Awake()
    {
        Instance = this;
        if (gameCamera == null)
        {
            Debug.LogError("Main camera is not set");
        }
        GUI = transform.parent.gameObject;
        GameManager.Restart += OnRestart;
        LevelManager.BeforeSceneLoading += ResetMaterial;
        if (DataManager.Exists())
        {
            ButtonMovement = DataManager.Settings.buttonMovement;
            showFps = DataManager.Settings.showFps;
        }
    }

    private void ResetMaterial()
    {
        material.color = Color.white;
    }

    private void OnDestroy()
    {
        GameManager.Restart -= OnRestart;
        LevelManager.BeforeSceneLoading -= ResetMaterial;
        material.color = new Color32(248, 230, 195,255);
    }

    void OnRestart()
    {
        IsRestarting = false;
        LastCheckpoint = null;
        RestartCharacter(Character.GetLocalPlayer());
    }

    // Use this for initialization
    void Start()
    {
        //Application.targetFrameRate = 60;
        defaultResolution = new Vector2(Screen.width, Screen.height);
        aspectRatio = (float)Screen.width / (float)Screen.height;
        startResolution = new Vector2(Screen.width, Screen.height);
        GameManager.GameReady += DeactivateActionButton;
        PlayerDead += DeactivateActionButtonOnPlayerDeath;
        Draw.ResetMedianFps();
        SetGraphicsLevel();
    }

    void DeactivateActionButtonOnPlayerDeath(Character character)
    {
        DeactivateActionButton();
        ActivationTrigger.activatedTriggers = 0;
    }

    void DeactivateActionButton()
    {
        //Debug.Log("Went off");
        var button = GameGUI.GetButtonByName("Action");
        if (button != null && button.gameObject != null)
        {
            button.gameObject.SetActive(false);
        }
    }

    public event System.Action<Character> PlayerDead;
    public void OnPlayerDead(Character character)
    {
        if(character.IsLocalPlayer)
        {
            float time = character.IsDead ? 3 : 0f;
            StartCoroutine(PlayerDeadCoroutine(character, time));
            PlayerDead?.Invoke(character);
        }
    }

    IEnumerator PlayerDeadCoroutine(Character character, float waitTime)
    {
        if (IsRestarting)
            yield break;
        IsRestarting = true;
        yield return new WaitForSeconds(waitTime);

        int currentRestarts = CollectionManager.Instance.GetCollection(character.ID, CollectionType.Restart);
        var collections = DataManager.Collections;
        #region DEBUG
        collections.restarts = 1;
        #endregion

        if (LastCheckpoint!=null && collections.restarts > 0 || currentRestarts > 0)
        {
            RestartCharacter(character);
        }
        else
        {
            //Destroy(character);
            //yield return null;
            //yield return null;
            UIWindow.GetWindow(UIWindow.END_SCREEN).RestartLevel();
        }
        yield return null;
    }

    void RestartCharacter(Character character)
    {
        Engine.Log.Print("Character Restart. ", Log.Color.Lime);
        if (character == null) return;
        character.movement.enabled = true;
        character.Health = character.stats.startHealth;
        //character.stats.health = 1;
       // character.movement.characterHealth.AddHealth(character.stats.health);
        character.movement.anim.Play("Idle");
        int currentRestarts = CollectionManager.Instance.GetCollection(character.ID, CollectionType.Restart);
        var collections = DataManager.Collections;
        if (currentRestarts > 0)
        {
            CollectionManager.Instance.SetCollection(character.ID, CollectionType.Restart, currentRestarts - 1);
        }
        else
        {
            collections.restarts--;
            if (character.IsLocalPlayer)
                DataManager.SaveData();

        }
        if(character.rb != null)
            character.rb.velocity = Vector3.zero;
        //character.rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionZ;

        gameCamera.GetComponent<GameCamera>().SetTarget(character.transform);
        if (LastCheckpoint != null)
        {
            LastCheckpoint.ResetToCheckpoint(character);
        }
        else
        {
            character.transform.position = character.movement.StartPosition;
        }
        character.movement.CharacterSetActive(true);
        IsRestarting = false;
    }

    public Material material;
    public Shader lowDetailShader;
    public Shader highDetailShader;
    
    void SetGraphicsLevel()
    {
        int graphicsLevel = DataManager.Settings.graphicsLevel;

        var mode = GameQualitySettings.CurrentMode;
        var quality = GameQualitySettings.GetMode(mode);
        gameCamera.mainCamera.farClipPlane = quality.cameraFarClip;

        switch (mode)
        {
            case GameQualitySettings.GameQuality.VeryLow: //Very Low
                //QualitySettings.SetQualityLevel(0);
                //gameCamera.mainCamera.farClipPlane = 60;
                //ChangeToLowDetailMaterial();
                bloom.enabled = false;
                antialiasing.enabled = false;
                break;
            case GameQualitySettings.GameQuality.Low: //Low
                //QualitySettings.SetQualityLevel(1);
                //gameCamera.mainCamera.farClipPlane = 90;
                //ChangeToLowDetailMaterial();
                bloom.enabled = false;
                antialiasing.enabled = false;
                break;
            case GameQualitySettings.GameQuality.Meduim: //Medium
                //QualitySettings.SetQualityLevel(6);
                //gameCamera.mainCamera.farClipPlane = 90;
                //ChangeToLowDetailMaterial();
                bloom.enabled = false;
                antialiasing.enabled = false;
                break;
            case GameQualitySettings.GameQuality.High: //High
                //QualitySettings.SetQualityLevel(6);
                //gameCamera.mainCamera.farClipPlane = 90;
                //ChangeToHighDetailMaterial();
                bloom.enabled = true;
                antialiasing.enabled = false;
                break;
            case GameQualitySettings.GameQuality.Ultra: //Very High
                //QualitySettings.SetQualityLevel(6);
                //gameCamera.mainCamera.farClipPlane = 90;
                //ChangeToHighDetailMaterial();
                bloom.enabled = true;
                antialiasing.enabled = true;
                break;
            default:
                Debug.LogError("Graphics level not set.");
                break;
        }
    }


    bool rayOn = true;
    GameObject[] rays;
    private void OnGUI()
    {
        if (showFps)
        {
            Draw.DisplayFpsMedian(Screen.width / 2, 10, Color.red, 40);

                //Engine.Draw.TextColorSize(10, 50, 255, 0, 0, 1, 20, "Native resolution: " + GameQualitySettings.NativeResolution);
                //Engine.Draw.TextColorSize(10, 80, 255, 0, 0, 1, 20, "Current resolution: " + Screen.currentResolution);
                //Engine.Draw.TextColorSize(10, 110, 255, 0, 0, 1, 20, "Aspect: " + GameQualitySettings.Aspect);
                //Engine.Draw.TextColorSize(10, 140, 255, 0, 0, 1, 20, "Quality: " + GameQualitySettings.CurrentMode);
            //Draw.DisplayMedianFps(Screen.width / 2 - Screen.width * 0.1f, 70);
        }

        //if (UnityEngine.GUI.Button(new Rect(10, 60, 100, 50), "Bloom: " + bloom.enabled))
        //{
        //    if (bloom != null) bloom.enabled = !bloom.enabled;
        //}
        //if (UnityEngine.GUI.Button(new Rect(110, 60, 100, 50), "Antialiasing: " + antialiasing.enabled))
        //{
        //    if (antialiasing != null) antialiasing.enabled = !antialiasing.enabled;
        //}




    }

}
