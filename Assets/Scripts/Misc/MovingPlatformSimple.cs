using Engine;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformSimple : LevelElement
{
    public Transform startAnchor;
    public Transform endAnchor;
    public Transform platform;

    public float speed = 2;
    public float waitingTime = 1;
    Quaternion rotation;
    Vector3 anchor1Pos;
    Vector3 anchor2Pos;

    bool forward;
    float time;
    float curWaitTime;
    public TriggerBroadcast triggerBroadcast;
    Dictionary<int, Transform> platformers;

    private void Awake()
    {
        if(triggerBroadcast)
        {
            triggerBroadcast.TriggerEntered += BroadCastTriggerEnter;
            triggerBroadcast.TriggerExit += BroadCastTriggerExit;
        }

        platformers = new Dictionary<int, Transform>();
    }

    void BroadCastTriggerEnter(Collider other)
    {
        if (other.transform.root.gameObject.layer == Layers.Character)
        {
            var transform = other.transform.root;
            transform.parent = platform;
            platformers.Add(other.GetInstanceID(), transform);
        }
    }

    void BroadCastTriggerExit(Collider other)
    {
        //Debug.Log("Moving platform TriggerExit");
        if (platformers.ContainsKey(other.GetInstanceID()))
        {
            int id = other.GetInstanceID();
            var transform = platformers[id];
            transform.parent = null;
            platformers.Remove(id);
        }
    }

    private void FixedUpdate()
    {
        if(time < 1)
        {
            if (forward)
            {
                time += speed * Time.fixedDeltaTime;
                platform.position = Vector3.Lerp(startAnchor.position, endAnchor.position, time);
            }
            else
            {
                time += speed * Time.fixedDeltaTime;
                platform.position = Vector3.Lerp(endAnchor.position, startAnchor.position, time);
            }
        }
        else
        {
            if(curWaitTime < waitingTime)
            {
                curWaitTime += Time.fixedDeltaTime;
            }
            else
            {
                curWaitTime = 0;
                time = 0;
                forward = !forward;
            }
        }


    }

    public override void OnLoad()
    {
        base.OnLoad();
        if(data!=null)
        {
            if (data.ContainsKey("Anchor1"))
                anchor1Pos = (Engine.Float3)data["Anchor1"];
            if (data.ContainsKey("Anchor2"))
                anchor2Pos = (Engine.Float3)data["Anchor2"];
            if (data.ContainsKey("Rotation"))
                rotation = (Engine.Float4)data["Rotation"];
            if (data.ContainsKey("Speed"))
                speed = (float)data["Speed"];
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
        if(data!=null)
        {
            if(startAnchor!=null)
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
            data["Speed"] = speed;
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