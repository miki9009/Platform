
using Engine.Config;
using Engine.Singletons;
using System;
using UnityEngine;

public class GameQualitySettings : Module
{
    public enum GameQuality
    {
        VeryLow = 0,
        Low = 1,
        Meduim = 2,
        High = 3,
        Ultra = 4
    }

    static QualityConfig config;
    static int qualitySettings;
    static GameQualitySettings instance;
    public static GameQuality CurrentMode { get; private set; }
    public static Vector2 NativeResolution { get; private set; }

    public static float Aspect { get; private set; }

    public static event Action<GameQuality> QualityChanged;

    public static void ChangeQuality(int quality)
    {
        SetQualitySettings(quality);
        QualityChanged?.Invoke((GameQuality)quality);
    }

    public static void ChangeQuality(GameQuality quality)
    {
        ChangeQuality((int)quality);
    }

    public override void Initialize()
    {
        instance = this;
        NativeResolution = new Vector2(Screen.width, Screen.height);
        config = ConfigsManager.GetConfig<QualityConfig>();
        Aspect = Camera.main.aspect;
        Console.WriteLine("Aspect: " + Aspect);
        DataManager.Loaded += () =>
        {
            qualitySettings = DataManager.Settings.graphicsLevel;
        };
    }

    public override void BeforeDestroyed()
    {
        base.BeforeDestroyed();
#if UNITY_EDITOR
        SetQualitySettings((int)GameQuality.Ultra);
#endif
    }

    static void SetQualitySettings(int quality)
    {
        CurrentMode = (GameQuality)quality;
        qualitySettings = quality;
        Debug.Log("Changed quality to: " + CurrentMode);
        if (qualitySettings < 0 || qualitySettings >= config.qualityModes.Count)
            Debug.LogError("Index out of range on setting quality.");

        var qualityMode = config.qualityModes[qualitySettings];
        QualitySettings.SetQualityLevel(qualityMode.UnityQualitySettings);
        foreach (var materialReplacement in qualityMode.materialReplacements)
        {
            materialReplacement.material.shader = materialReplacement.shader;
        }

       // SetResolution();
    }

    public static void SetResolution()
    {
        var qualityMode = config.qualityModes[qualitySettings];
        int width = (int)(NativeResolution.x * qualityMode.resolutionRange);
        int height = (int)(NativeResolution.y * qualityMode.resolutionRange);
        Screen.SetResolution(width, height, true);
    }

    public static QualityMode GetMode(int quality)
    {
        if (qualitySettings < 0 || qualitySettings >= config.qualityModes.Count)
            Debug.LogError("Index out of range on setting quality.");

        return config.qualityModes[qualitySettings];
    }

    public static QualityMode GetMode(GameQuality quality)
    {
        return GetMode((int)quality);
    }



    static Vector2[] _cachedResolutions;
    public static Vector2[] CachedResolutions
    {
        get
        {
            if (_cachedResolutions == null)
            {
                _cachedResolutions = new Vector2[]
                {
                    new Vector2((int)(440 * Aspect), 440),
                    new Vector2((int)(568 * Aspect), 568),
                    new Vector2((int)(696 * Aspect), 696),
                    new Vector2((int)(824 * Aspect), 824),
                    new Vector2((int)(952 * Aspect), 952),
                    new Vector2((int)(1080 * Aspect), 1080),
                };
            }
            return _cachedResolutions;
        }      
    }
}