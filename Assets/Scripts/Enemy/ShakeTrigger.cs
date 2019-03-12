using Engine;
using UnityEngine;

public class ShakeTrigger : LevelElement
{
    public bool isShaking;
    public BoxCollider boxCollider;
    public float shakeForce = 0.5f;
    public float amplitude = 0.2f;

    private void OnTriggerEnter(Collider other)
    {
            var character = other.gameObject.GetComponentInParent<Character>();
            if(character!=null && character.IsLocalPlayer)
            {
                isShaking = true;
            }
    }

    private void OnTriggerExit(Collider other)
    {
            var character = other.gameObject.GetComponentInParent<Character>();
            if (character != null && character.IsLocalPlayer)
            {
                isShaking = false;
            }
    }

    private void Update()
    {
        if(isShaking)
        {
            Controller.Instance.gameCamera.Shake(1, shakeForce, amplitude);
        }
    }

    public override void OnLoad()
    {
        base.OnLoad();
        if(data.ContainsKey("Size"))
        {
            boxCollider.size = (Float3)data["Size"];
            boxCollider.center = (Float3)data["Center"];
        }
    }

    public override void OnSave()
    {
        base.OnSave();
        data["Size"] = (Float3)boxCollider.size;
        data["Center"] = (Float3)boxCollider.center;
    }
}