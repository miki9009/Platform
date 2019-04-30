using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Engine;

public class RotatingObject : LevelElement
{
    [SceneryElementSelector]
    public string modelID;

    public Transform rotationAnchor;

    public GameObject model;

    public override void OnLoad()
    {
        base.OnLoad();

        if(data.ContainsKey("Model"))
        {
            GameObject obj = (GameObject)Resources.Load((string)data["Model"]);
            if(obj)
            {
                var go = Instantiate(obj, rotationAnchor);
                SetModel(go);
            }

        }
    }

    public override void OnSave()
    {
        base.OnSave();

        data["Model"] = modelID;
    }

    public void SetModel(GameObject obj)
    {
        var t = obj.GetComponent<Transform>();
        t.SetParent(rotationAnchor);
        t.localPosition = Vector3.zero;
        t.localRotation = Quaternion.identity;
        model = obj;
    }
}
