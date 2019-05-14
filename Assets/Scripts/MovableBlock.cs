using UnityEngine;
using Engine;

public class MovableBlock : LevelElement, IMovable
{
    public Rigidbody rb;

    RigidbodyConstraints constraints;

    public Rigidbody Rigidbody
    {
        get
        {
            return rb;
        }
    }

    public bool ActiveAndEnabled
    {
        get
        {
            return enabled && gameObject.activeSelf;
        }
    }

    public RigidbodyConstraints PushConstraints
    {
        get
        {
            return constraints;
        }
    }


    public RigidbodyConstraints NonPushConstraints
    {
        get
        {
            return constraints;
        }
    }

    public override void OnLoad()
    {
        base.OnLoad();
        if(data.ContainsKey("Constraints"))
        {
            constraints = (RigidbodyConstraints)data["Constraints"];
            rb.constraints = constraints;
        }
    }

    public override void OnSave()
    {
        base.OnSave();
        data["Constraints"] = rb.constraints;
    }
}