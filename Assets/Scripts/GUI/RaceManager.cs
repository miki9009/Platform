using Engine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RaceManager : MonoBehaviour
{
    Character[] characters;
    private void Start()
    {
        GameManager.LevelClear += Stop;
    }

    bool activated = false;
    public void Activate()
    {
        totalWaypoints = WaypointManager.Instance.waypoints.Count;
        characters = new Character[Character.allCharacters.Count];
        for (int i = 0; i < Character.allCharacters.Count; i++)
        {
            characters[i] = Character.allCharacters[i];
        }

        if (!activated)
        {
            if (runCoroutine != null)
                StopCoroutine(runCoroutine);
            runCoroutine = StartCoroutine(Run());
        }

        CharacterProgressPanelManager.Instance?.Initialize(characters.ToList());

    }

    int totalWaypoints;

    Coroutine runCoroutine;
    IEnumerator Run()
    {
        while(true)
        {
            for (int i = 0; i < characters.Length; i++)
            {
                var gameProgress = characters[i].gameProgress;
                float waypointProgress = gameProgress.CurrentWaypoint / (float)totalWaypoints;
                int prevWaypoint = gameProgress.CurrentWaypoint > 0 ? gameProgress.CurrentWaypoint - 1 : totalWaypoints - 1;
                float totalDis = Vector3.Distance(WaypointManager.Instance.waypoints[gameProgress.CurrentWaypoint].transform.position, WaypointManager.Instance.waypoints[prevWaypoint].transform.position);
                float curDis = Vector3.Distance(WaypointManager.Instance.waypoints[gameProgress.CurrentWaypoint].transform.position, characters[i].transform.position);
                float disProgress = (1 - (curDis / totalDis)) / totalWaypoints;
                characters[i].gameProgress.raceProgress = (waypointProgress + disProgress) * characters[i].gameProgress.lap;
                yield return null;
            }
            yield return null;
            characters = characters.OrderByDescending(x => x.gameProgress.raceProgress).ToArray();
            for (int i = 0; i < characters.Length; i++)
            {
                characters[i].gameProgress.Placement = i;
            }
        }
    }



    void Stop()
    {
        if(activated)
        {
            if(runCoroutine != null)
                StopCoroutine(runCoroutine);
            runCoroutine = null;
            activated = false;
        }
    }

    
}