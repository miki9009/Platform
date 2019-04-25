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
            Engine.Log.Print("Activated Race Mode", Engine.Log.Color.Orange);
        }
    }

    private void OnDestroy()
    {
        GameManager.GameReady -= Initialize;
    }
}