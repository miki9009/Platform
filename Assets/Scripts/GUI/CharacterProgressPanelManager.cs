using Engine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterProgressPanelManager : MonoBehaviour
{
    public static CharacterProgressPanelManager Instance { get; private set; }
    public GameObject panelPrefab;
    public Transform panelsAnchor;
    public RectTransform[] anchors;
    public Text localPlacement;
    List<CharacterProgressPanel> panels;
    public Text timerLabel;
    public Text lapsLabel;
    public int laps = 4;

    bool start = false;
    public int timer = 3;
    public enum Mode {None, Challenge, Race}

    public static Mode GameMode;

    Character localCharacter;

    private void Awake()
    {
        Instance = this;
        Character.CharacterCreated += AttachPlacement;
        GameManager.LevelClear += ResetManager;
        GameManager.GameReady += GameManager_GameReady;
        Level.LevelLoaded += Level_LevelLoaded;
    }

    private void OnEnable()
    {
        GetMode();
        Debug.Log("Is Enabled with mode: " + GameMode.ToString());
    }

    void GetMode()
    {
        string mode = GameManager.GameMode;
        if (mode.Contains("Race"))
            GameMode = Mode.Race;
        else if (mode.Contains("Challenge"))
            GameMode = Mode.Challenge;
        else
            GameMode = Mode.None;
    }

    private void Level_LevelLoaded()
    {
        timerLabel.gameObject.SetActive(false);
        if(GameManager.GameMode.Contains("Race"))
        {
            localPlacement.gameObject.SetActive(true);
            Activate();
        }
        else
        {
            localPlacement.gameObject.SetActive(false);
        }
    }

    Coroutine activeCoroutine;
    void Activate()
    {
        if (activeCoroutine != null)
            CoroutineHost.Stop(activeCoroutine);
        activeCoroutine = CoroutineHost.Start(KeepControlsDisabled());
    }

    private void OnDestroy()
    {
        if (activeCoroutine != null)
            CoroutineHost.Stop(activeCoroutine);
        Character.CharacterCreated -= AttachPlacement;
        GameManager.LevelClear -= ResetManager;
        GameManager.GameReady -= GameManager_GameReady;
        Level.LevelLoaded -= Level_LevelLoaded;
        GameMode = Mode.None;
    }

    public void Initialize(List<Character> characters)
    {
        if(panels!=null)
        {
            foreach (var panel in panels)
            {
                if (panel != null)
                    Destroy(panel.gameObject);
            }
            panels.Clear();
        }
        panels = new List<CharacterProgressPanel>();
        int i = 0;
        foreach (var character in characters)
        {
            var panel = Instantiate(panelPrefab, panelsAnchor).GetComponent<CharacterProgressPanel>();
            panel.Initialize(character);
            panel.rectTransform.anchoredPosition = anchors[i].anchoredPosition;
            panels.Add(panel);
            i++;
        }
        if(GameMode == Mode.Race)
        {
            Character.GetLocalPlayer().WaypointVisited += CheckLap;
            localPlacement.gameObject.SetActive(true);
        }
        else
        {
            localPlacement.gameObject.SetActive(false);
        }

    }

    void CheckLap(int waypointIndex)
    {
        lapsLabel.text = Character.GetLocalPlayer().gameProgress.lap.ToString()+"/"+laps;
    }

    void AttachPlacement(Character character)
    {
        if (!character.IsLocalPlayer || !GameManager.GameMode.Contains("Race")) return;
        localCharacter = character;
        localCharacter.PlacementChanged -= LocalCharacter_PlacementChanged;
        localCharacter.PlacementChanged += LocalCharacter_PlacementChanged;
    }

    private void LocalCharacter_PlacementChanged(int placement)
    {
        localPlacement.text = GetPlace(placement);
    }

    string GetPlace(int placement)
    {

        switch(placement)
        {
            case 0:
                return "1st";
            case 1:
                return "2nd";
            case 2:
                return "3rd";
        }
        return (placement+1).ToString() +"th";
    }

    public void RefreshPositions()
    {
        foreach (var panel in panels)
        {
            panel.rectTransform.anchoredPosition = anchors[panel.Character.gameProgress.Placement].anchoredPosition;
        }
    }

    private void GameManager_GameReady()
    {
        if(GameManager.GameMode.Contains("Race"))
            CoroutineHost.Start(EnableTimer());
    }

    void ResetManager()
    {
        start = false;
        timer = 3;
    }

    IEnumerator EnableTimer()
    {
        timerLabel.gameObject.SetActive(true);
        while (timer > 0)
        {
            yield return new WaitForSeconds(1);
            timer--;
            timerLabel.text = timer.ToString();
        }
        timerLabel.text = "GO!";
        start = true;
        yield return new WaitForSeconds(1);
        timerLabel.gameObject.SetActive(false);
    }

    IEnumerator KeepControlsDisabled()
    {
        while (!start)
        {
            foreach (var character in Character.allCharacters)
            {
                if (character.movement.Initialized)
                    character.movement.enabled = false;
            }
            yield return null;
        }
        foreach (var character in Character.allCharacters)
        {
            character.movement.enabled = true;
        }
        activeCoroutine = null;
    }
}