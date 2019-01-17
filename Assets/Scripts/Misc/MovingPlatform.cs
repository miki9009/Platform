using Engine;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : LevelElement
{
    BezierCurve curve;
    public Transform platform;
    public float speed;
    public bool rotate;
    public float rotationSpeed;
    public TriggerBroadcast triggerBroadcast;
    public Transform[] points;

    [Range(0, 1f)]
    public float pos;
    Vector3 prevPos;
    float calculatedSpeed;
    float calculatedRotationSpeed;

    Dictionary<int, Transform> platformers;
    Vector3 lastPos;

    float dis;
    Vector3 dir;

    private void Awake()
    {
        platformers = new Dictionary<int, Transform>();
        curve = GetComponentInChildren<BezierCurve>();
        triggerBroadcast.TriggerEntered += BroadCastTriggerEnter;
        triggerBroadcast.TriggerExit += BroadCastTriggerExit;
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
        if (pos + calculatedSpeed >= 1) pos -= 1f;
            pos += calculatedSpeed;
        platform.position = Vector3.Lerp(platform.position, curve.GetPointAt(pos), Time.deltaTime * speed);
        if (rotate)
        {
            Quaternion q = Quaternion.Lerp(platform.rotation, Quaternion.LookRotation(Engine.Vector.Direction(platform.position, curve.GetPointAt(Mathf.Clamp01(pos)))), calculatedRotationSpeed);
            platform.eulerAngles = new Vector3(0, q.eulerAngles.y, q.eulerAngles.z);
        }
    }

    Float3[] pointsPos;
    Float4[] pointsRot;
    Float3[] handle1Pos;
    Float3[] handle2Pos;

    public override void OnSave()
    {
        base.OnSave();
        int length = points.Length;
        pointsPos = new Float3[length];
        pointsRot = new Float4[length];
        handle1Pos = new Float3[length];
        handle2Pos = new Float3[length]; 
        for (int i = 0; i < length; i++)
        {
            pointsPos[i] = points[i].localPosition;
            pointsRot[i] = points[i].localRotation;
            var handle = points[i].GetComponent<BezierPoint>();
            handle1Pos[i] = handle.handle1;
            handle2Pos[i] = handle.handle2;
        }
        if(data!=null)
        {
            data["Points"] = pointsPos;
            data["Rotations"] = pointsRot;
            data["Handle1"] = handle1Pos;
            data["Handle2"] = handle2Pos;
            data["PlatformPos"] = pos;
            data["Rotate"] = rotate;
            data["Speed"] = speed;
        }
    }

    public override void OnLoad()
    {
        base.OnLoad();
        if (data != null)
        {
            if (data.ContainsKey("Points"))
                pointsPos = (Float3[])data["Points"];
            if (data.ContainsKey("Rotations"))
                pointsRot = (Float4[])data["Rotations"];
            if (data.ContainsKey("Handle1"))
                handle1Pos = (Float3[])data["Handle1"];
            if (data.ContainsKey("Handle2"))
                handle2Pos = (Float3[])data["Handle2"];
            if (data.ContainsKey("PlatformPos"))
                pos = (float)data["PlatformPos"];
            if (data.ContainsKey("Rotate"))
                rotate = (bool)data["Rotate"];
            if (data.ContainsKey("Speed"))
                speed = (float)data["Speed"];
            for (int i = 0; i < pointsPos.Length; i++)
            {
                points[i].transform.localPosition = pointsPos[i];
                points[i].transform.localRotation = pointsRot[i];
                var handle = points[i].GetComponent<BezierPoint>();
                handle.handle1 = handle1Pos[i];
                handle.handle2 = handle2Pos[i];
            }
        }
        calculatedSpeed = speed * 0.0016f;
        prevPos = platform.position;
        calculatedRotationSpeed = rotationSpeed * 0.016f;
        lastPos = platform.position;
    }

}