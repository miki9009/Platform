using System;
using UnityEngine;

public class GameTime : MonoBehaviour
{
    public int timeToFinish;
    public bool timeBased;

    public int TimeLeft { get; private set; }
    float curTime = 0;
    int seconds = 0;

    public event Action TimeElapsed;
    public event Action<int> TimeAdded;
    public event Action EndOfTime;
    public static bool TimerRunning { get; set; }

    private static GameTime instance;
    public static GameTime Instance
    {
        get
        {
            if (instance == null)
            {
                var go = new GameObject("GameTime");
                go.AddComponent<GameTime>();
            }
            return instance;
        }
        private set
        {
            instance = value;
        }
    }



    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            Debug.LogError("Instance of GameTime was already defined");
        }
        GameManager.GameReady += Restart;
        GameManager.LevelClear += Restart;
    }

    private void Start()
    {
        if (!timeBased)
            enabled = false;
        PhotonManager.GlobalMessageReceived += CheckTimeOnMultiplayer;
    }

    private void CheckTimeOnMultiplayer(byte code, object time)
    {
        if (code == PhotonEventCode.TIME && !PhotonManager.IsMaster)
        {
            TimeLeft = (int)time;
            TimeElapsed?.Invoke();
        }

    }

    bool finished;
    private void FixedUpdate()
    {
        if (!PhotonManager.IsMultiplayer || PhotonManager.IsMaster)
            UpdateTimer();
    }

    void Restart()
    {
        TimerRunning = true;
        TimeLeft = timeToFinish;
    }



    void UpdateTimer()
    {
        if (TimerRunning)
        {
            curTime += Time.fixedDeltaTime;
            if (curTime >= 1 || finished)
            {
                curTime = 0;
                seconds++;
                if (TimeLeft > 0)
                {
                    TimeLeft--;
                }
                else
                {
                    Debug.Log("TIME ELAPSED");
                    finished = true;
                    TimerRunning = false;
                    EndOfTime?.Invoke();
                }
                TimeElapsed?.Invoke();
            }
        }
    }

    public void AddTime(int time)
    {
        TimeLeft += time;
        TimeAdded?.Invoke(time);
    }

    private void OnDestroy()
    {
        PhotonManager.GlobalMessageReceived -= CheckTimeOnMultiplayer;
        GameManager.LevelClear -= Restart;
        GameManager.GameReady -= Restart;
    }
}

