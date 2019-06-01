using Engine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityStandardAssets.ImageEffects;
using Engine;
using Engine.Threads;

public class WorldCameraMovement : MonoBehaviour
{

    public static Camera CurrentCamera
    {
        get
        {
            if (instance)
                return instance.cam;
            return null;
        }
    }
    public Camera cam;
    public Transform worldAnchor;
    public Transform startAnchor;

    Transform currentTarget;
    public VignetteAndChromaticAberration vignette;
    public float animationSpeed = 2;
    static WorldCameraMovement instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        SetViewToStartView();
    }


    void MoveTo()
    {
        if (transform == null || currentTarget == null) return;
        transform.position = currentTarget.position;
        transform.rotation = currentTarget.rotation;
    }

    [EventMethod]
    public static void SetViewToWorld()
    {
        instance.currentTarget = instance.worldAnchor;
        instance.MoveTo();
    }

    [EventMethod]
    public static void SetViewToStartView()
    {
        instance.currentTarget = instance.startAnchor;
        instance.MoveTo();
    }


}
