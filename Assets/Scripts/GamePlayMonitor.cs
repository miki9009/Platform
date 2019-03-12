using Engine;
using UnityEngine;

public class GamePlayMonitor : MonoBehaviour
{
    public static GamePlayMonitor Instance { get; private set; }
    public RaceManager raceManager;

    private void Awake()
    {
        Instance = this;
        GameManager.GameReady += Initialize;
    }

    private void Initialize()
    {
        if(GameManager.GameMode.Contains("Race"))
        {
            raceManager.Activate();
            Console.WriteLine("Activated Race Mode", Console.LogColor.Orange);
        }
    }

    private void OnDestroy()
    {
        GameManager.GameReady -= Initialize;
    }
}