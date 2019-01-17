using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : CollectionObject
{
    public ParticleSystem heartExplosion;
    public ParticleSystem heartInside;
    protected override void Start()
    {
        base.Start();
        OnCollected += Event_OnCollected;
        //OnCollected += 
    }

    void Event_OnCollected(GameObject obj)
    {
        heartInside.gameObject.SetActive(false);
        heartExplosion.Play();
        var character = obj.GetComponentInParent<Character>();
        if (character != null)
        {
            character.stats.health++;
            character.movement.characterHealth.AddHealth(character.stats.health);
        }
    }



}
