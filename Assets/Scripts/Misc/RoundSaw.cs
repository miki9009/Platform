using Engine;
using UnityEngine;

public class RoundSaw : LevelElement
{
    public float rotationSpeed = 1;
    public Transform rotator;
    public Transform[] saws;
    public int damage = 20;
    public TriggerBroadcast[] triggers;

    private void Start()
    {
        for (int i = 0; i < triggers.Length; i++)
        {
            triggers[i].TriggerEntered += OnTriggerBroadcast;
        }
    }

    private void Update()
    {
        rotator.Rotate(0, rotationSpeed, 0);
        for (int i = 0; i < saws.Length; i++)
        {
            saws[i].Rotate(0,0 , 10);
        }
    }

    void OnTriggerBroadcast(Collider col)
    {
        if (col.gameObject.layer == Layers.Character)
        {
            var movement = col.GetComponent<CharacterMovement>();
            if (movement)
            {
                movement.Hit(null, damage, true);
            }
        }
    }

    public override void OnLoad()
    {
        base.OnLoad();
        if (data.ContainsKey("RotationSpeed"))
            rotationSpeed = (float)data["RotationSpeed"];
        if (data.ContainsKey("Damage"))
            damage = (int)data["Damage"];
    }

    public override void OnSave()
    {
        base.OnSave();
        data["Damage"] = damage;
        data["RotationSpeed"] = rotationSpeed;
    }


}