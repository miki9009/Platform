﻿using Engine;
using System.Collections;
using UnityEngine;

public class MissionWaypoint : LevelElement
{
    [MissionSelector]
    public string missionID;
    public float height = 1;
    public bool active = true;

    Vector3 startScale;
    MeshRenderer meshRenderer;
    Mission mission;


    private void Awake()
    {
        startScale = transform.localScale;
        meshRenderer = GetComponentInChildren<MeshRenderer>();
    }

    public event System.Action<CharacterMovement> Visited;

    private void Start()
    {
        if (arrowTarget)
        {
            ArrowActivator.Enable(false);
        }
        if (!mission.unlocked)
            Deactivate();
    }

    void OnTriggerEnter(Collider other)
    {
        var character = other.GetComponentInParent<Character>();
        Visited?.Invoke(character.movement);
        if(active)
        {
            active = false;
            GoToLevelAdditive();
        }

    }

    void GoToLevelAdditive()
    {
        LevelManager.ChangeLevel(LevelsConfig.GetSceneName(missionID), LevelsConfig.GetLevelName(missionID));
    }

    private void Initialize()
    {
        mission = MissionsConfig.Instance.GetMission(missionID);
    }

    public override void OnLoad()
    {
        base.OnLoad();
        if (data != null)
        {
            object obj;
            if (data.TryGetValue("LevelName", out obj))
            {
                missionID = (string)obj;
            }
            if (data.TryGetValue("Height", out obj))
            {
                height = (float)obj;
            }

            Initialize();
        }
    }

    public override void OnSave()
    {
        base.OnSave();
        if (data != null)
        {
            data["Height"] = height;
            data["LevelName"] = missionID;
        }
    }

    private void OnDestroy()
    {

    }

    public void Deactivate()
    {
         gameObject.SetActive(false);
    }
}