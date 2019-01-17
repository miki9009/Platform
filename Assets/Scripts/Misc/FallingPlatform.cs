using Engine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : LevelElement
{
    public Transform startAnchor;
    public Transform endAnchor;
    public Transform platform;
    public float shakeXFactor = 0.1f;
    public float shakeYFactor = 0.1f;
    public float shakeZFactor = 0.1f;
    public Collider col;

    public float fallingSpeed = 2;
    public float waitingTime = 1;
    Quaternion rotation;
    Vector3 anchor1Pos;
    Vector3 anchor2Pos;

    float time;
    public TriggerBroadcast triggerBroadcast;
    bool triggered;
    Vector3 startPos;

    private void Awake()
    {

        if (triggerBroadcast)
        {
            triggerBroadcast.TriggerEntered += BroadCastTriggerEnter;
            triggerBroadcast.TriggerExit += BroadCastTriggerExit;
        }
    }

    public override void ElementStart()
    {
        base.ElementStart();
        startPos = transform.position;
    }

    void BroadCastTriggerEnter(Collider other)
    {
        triggered = true;
        StartCoroutine(Fall());
        StartCoroutine(Shake());
    }

    void BroadCastTriggerExit(Collider other)
    {
        triggered = false;
    }

    IEnumerator Fall()
    {
        yield return new WaitForSeconds(waitingTime);
        if (!triggered) yield break;
        triggered = false;
        float progress = 0;
        while(progress < 1)
        {
            progress += Time.deltaTime * fallingSpeed;
            platform.position = Vector3.Slerp(startPos, endAnchor.position, progress);
            yield return null;
        }
        progress = 0;
        col.enabled = false;
        yield return new WaitForSeconds(2);
        col.enabled = true;
        while (progress < 1)
        {
            progress += Time.deltaTime;
            platform.position = Vector3.Slerp(endAnchor.position, startPos, progress);
            yield return null;
        }
    }

    IEnumerator Shake()
    {
        while(triggered)
        {
            transform.position = new Vector3(startPos.x + Random.Range(-shakeXFactor, shakeXFactor), startPos.y + 
                Random.Range(-shakeYFactor, shakeYFactor), startPos.z + Random.Range(-shakeZFactor, shakeZFactor));
            yield return null;
        }

    }



    public override void OnLoad()
    {
        base.OnLoad();
        if (data != null)
        {
            if (data.ContainsKey("Anchor1"))
                anchor1Pos = (Engine.Float3)data["Anchor1"];
            if (data.ContainsKey("Anchor2"))
                anchor2Pos = (Engine.Float3)data["Anchor2"];
            if (data.ContainsKey("Rotation"))
                rotation = (Engine.Float4)data["Rotation"];
            if (data.ContainsKey("Speed"))
                fallingSpeed = (float)data["Speed"];
            if (data.ContainsKey("WaitingTime"))
                waitingTime = (float)data["WaitingTime"];

            startAnchor.position = anchor1Pos;
            endAnchor.position = anchor2Pos;
            platform.rotation = rotation;
        }
    }

    public override void OnSave()
    {
        base.OnSave();
        if (data != null)
        {
            if (startAnchor != null)
            {
                anchor1Pos = startAnchor.position;
            }
            if (endAnchor != null)
            {
                anchor2Pos = endAnchor.position;
            }
            rotation = platform.rotation;
            data["Anchor1"] = (Engine.Float3)anchor1Pos;
            data["Anchor2"] = (Engine.Float3)anchor2Pos;
            data["Rotation"] = (Engine.Float4)rotation;
            data["Speed"] = fallingSpeed;
            data["WaitingTime"] = waitingTime;
        }
    }

#if UNITY_EDITOR
    public Vector3 gizmoPlatformSize;
    private void OnDrawGizmos()
    {

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(startAnchor.position, 1);
        Gizmos.color = Color.red;
        //Gizmos.DrawSphere(endAnchor.position, 1);
        Gizmos.DrawLine(startAnchor.position, endAnchor.position);
        Gizmos.DrawCube(endAnchor.position, gizmoPlatformSize);
    }
#endif
}