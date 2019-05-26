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


    IEnumerator MoveTo()
    {
        float animation = 0;
        vignette.enabled = true;
        while(animation < 1)
        {
            animation += Time.deltaTime * animationSpeed;
            vignette.intensity = animation;
            yield return null;
        }
        transform.position = currentTarget.position;
        transform.rotation = currentTarget.rotation;

        while (animation > 0)
        {
            animation -= Time.deltaTime * animationSpeed;
            vignette.intensity = animation;
            yield return null;
        }

        vignette.intensity = 0;
        vignette.enabled = false;
    }

    [EventMethod]
    public static void SetViewToWorld()
    {
        instance.currentTarget = instance.worldAnchor;
        CoroutineHost.Start(instance.MoveTo());
    }

    [EventMethod]
    public static void SetViewToStartView()
    {
        instance.currentTarget = instance.startAnchor;
        CoroutineHost.Start(instance.MoveTo());
    }


}
