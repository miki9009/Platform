using Engine;
using System.Collections.Generic;
using UnityEngine;

public class Cog : LevelElement
{
    public Transform cog;
    public float speed = 1;
    public TriggerBroadcast triggerBroadcast;

    Dictionary<int, Transform> platformers;
    Vector3 euler;
    public override void ElementStart()
    {
        base.ElementStart();
        euler = cog.eulerAngles;
        triggerBroadcast.TriggerEntered += BroadCastTriggerEnter;
        triggerBroadcast.TriggerExit += BroadCastTriggerExit;
        platformers = new Dictionary<int, Transform>();
    }
    void BroadCastTriggerEnter(Collider other)
    {
        if (other.transform.root.gameObject.layer == Layers.Character)
        {
            var transform = other.transform.root;
            transform.parent = cog;
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

    private void Update()
    {
        cog.localRotation = Quaternion.Lerp(cog.localRotation, Quaternion.Euler(euler.x, cog.localEulerAngles.y +359, euler.z), Time.deltaTime * speed);
    }
}