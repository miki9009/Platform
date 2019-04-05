
using System;
using UnityEngine;

public class GameQualitySettings : MonoBehaviour
{
    public enum GameQuality
    {
        VeryLow,
        Low,
        Meduim,
        High,
        Ultra
    }
    public static event Action<GameQuality> QualityChanged;

    public static void OnQualityChanged(GameQuality quality)
    {
        QualityChanged?.Invoke(quality);
    }
}