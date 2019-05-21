using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Engine;

public class RotatingObject : LevelElement
{
    //public string modelID;

    public Transform rotationAnchor;
    public float rotationSpeed = 1;
    public Vector3 rotationChange;
    public TriggerBroadcast triggerBroadcast;
    Dictionary<int, Transform> platformers;
    Vector3 lastPos;

    float dis;
    Vector3 dir;

    private void Awake()
    {
        platformers = new Dictionary<int, Transform>();
    }

    void TriggerEnter(Collider other)
    {

        if (other.gameObject.layer == Layers.Character && !platformers.ContainsKey(other.gameObject.GetInstanceID()))
        {
            var trans = other.transform;
            trans.SetParent(rotationAnchor);
            platformers.Add(other.gameObject.GetInstanceID(), trans);
            Debug.Log("Trigger Enter");
        }
    }

    void TriggerExit(Collider other)
    {
        if (platformers.ContainsKey(other.gameObject.GetInstanceID()))
        {
            int id = other.gameObject.GetInstanceID();
            var trans = platformers[id];
            trans.parent = null;
            platformers.Remove(id);
            Debug.Log("Trigger Exit");
        }
    }

    public Transform model;

    public override void OnLoad()
    {
        base.OnLoad();

        //if(data.ContainsKey("Model"))
        //{
        //modelID = (string)data["Model"];
        //GameObject obj = (GameObject)Resources.Load(modelID);
        //if(obj)
        //{
        //var go = Instantiate(obj, rotationAnchor);
        //SetModel(go);
        //var collider = go.GetComponentInChildren<Collider>();
        //if(collider)
        //{
        //    var triggerObj = Instantiate(collider.gameObject);
        //    triggerObj.transform.SetParent(model.transform);
        //    var scale = model.transform.localScale * 1.1f;
        //    triggerObj.transform.localScale = scale;
        //    var meshRenderer = triggerObj.GetComponent<MeshRenderer>();
        //    if (meshRenderer)
        //        meshRenderer.enabled = false;
        //    triggerObj.transform.localPosition = Vector3.zero;

        //    var triggerCollider = triggerObj.GetComponent<Collider>();
        //    if (triggerCollider)
        //    {
        //        if (triggerCollider.GetType() == typeof(MeshCollider))
        //            ((MeshCollider)triggerCollider).convex = true;
        //        triggerCollider.isTrigger = true;
        //    }

        //triggerBroadcast = triggerObj.AddComponent<TriggerBroadcast>();
        if (triggerBroadcast)
        {
            triggerBroadcast.TriggerEntered += TriggerEnter;
            triggerBroadcast.TriggerExit += TriggerExit;
        }

        //}
        if (data.ContainsKey("ModelRotation"))
        {
            model.transform.localRotation = (Float4)data["ModelRotation"];
        }
        if (data.ContainsKey("ModelScale"))
        {
            model.transform.localScale = (Float3)data["ModelScale"];
        }
        if (data.ContainsKey("ModelPosition"))
        {
            model.transform.localPosition = (Float3)data["ModelPosition"];
        }
        if (data.ContainsKey("RotationChange"))
        {
            rotationChange = (Float3)data["RotationChange"];
        }
        if (data.ContainsKey("RotationSpeed"))
        {
            rotationSpeed = (float)data["RotationSpeed"];
        }
        //}

        //}
    }

    private void FixedUpdate()
    {
        rotationAnchor.Rotate(new Vector3(rotationChange.x, rotationChange.y, rotationChange.z)*rotationSpeed);
    }

    public override void OnSave()
    {
        base.OnSave();

        //data["Model"] = modelID;
        if(model)
        {
            data["ModelRotation"] = (Float4)model.transform.localRotation;
            data["ModelScale"] = (Float3)model.transform.localScale;
            data["ModelPosition"] = (Float3)model.transform.localPosition;
            data["RotationChange"] = (Float3)rotationChange;
            data["RotationSpeed"] = (float)rotationSpeed;
        }
    }

    //public void SetModel(GameObject obj)
    //{
    //    var t = obj.GetComponent<Transform>();
    //    t.SetParent(rotationAnchor);
    //    t.localPosition = Vector3.zero;
    //    t.localRotation = Quaternion.identity;
    //    model = obj;
    //}
}
