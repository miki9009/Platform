using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public int id = -1;
    public bool collected = false;
    public int time;

    protected SphereCollider col;
    protected Character character;
    public CollectionType Type { get; private set; }

    public static HashSet<PowerUp> activePowerUps = new HashSet<PowerUp>();

    protected virtual void Start()
    {
        Type = GetComponent<CollectionObject>().type;
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    col.enabled = false;
    //    if (OnCollected != null)
    //    {
    //        character = other.GetComponentInParent<Character>();
    //        OnCollected(character);
    //        Apply();
    //    }
    //}

    protected virtual void ApplyPowerUp()
    {
        if (!RemoveDuplicate(Type))
        {
            PowerUpDisplayManager.Instance.AddDisplay(this);
        }
        else
        {
            PowerUpDisplayManager.Instance.ResetDisplay(this);
        }


        activePowerUps.Add(this);
    }

    protected virtual void Use()
    {
        //character.gameObject.AddComponent<Component>(powerUp);
    }

    public virtual void Disable()
    {
        activePowerUps.Remove(this);
    }

    static bool RemoveDuplicate(CollectionType type)
    {
        PowerUp pwr = activePowerUps.SingleOrDefault(x => x.Type == type);
        if (pwr != null)
        {
            pwr.Disable();
            return true;
        }
        return false;
    }

}
