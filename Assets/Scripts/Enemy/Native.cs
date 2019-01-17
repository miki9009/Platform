using UnityEngine;

public class Native : Enemy
{
    public TriggerBroadcast triggerBroadcast;

    //private void Awake()
    //{
    //    triggerBroadcast.TriggerEntered += TriggerEnter;
    //}

    //private void TriggerEnter(Collider other)
    //{
    //    var character = other.GetComponentInParent<Character>();
    //    if(character != null)
    //    {
    //        target = character.transform;
    //        action = 1;
    //    }
    //}

    //private void TriggerExit(Collider other)
    //{
    //    if (target != null)
    //    {
    //        var character = other.GetComponentInParent<Character>();
    //        if (character != null && character.transform == target)
    //        {
    //            target = null;
    //            action = 0;
    //        }
    //    }
    //}

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if(waitTimeCur <= 0)
    //        rb.velocity = Vector3.up * 3;
    //}


}