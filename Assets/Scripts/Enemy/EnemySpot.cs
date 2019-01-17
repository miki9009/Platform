using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpot : MonoBehaviour
{
    private Transform target;
    private Enemy enemy;

    void Start()
    {
        enemy = GetComponentInParent<Enemy>();
    }

    private void OnTriggerEnter(Collider other)
    {
        target = other.attachedRigidbody.GetComponentInParent<Character>().transform;
        if(enemy.target!=null)
        {
            if(Vector3.Distance(enemy.target.position, transform.position) >
                Vector3.Distance(target.position, transform.position))
            {
                enemy.target = null;
            }
        }
        if(enemy.target==null)
            enemy.target = target;
        Debug.Log("Target: " + other.attachedRigidbody.GetComponentInParent<Character>());
    }

    //private void OnTriggerExit(Collider other)
    //{
    //    if (target == null) return;
    //    if (other.attachedRigidbody.transform == target)
    //    {
    //        target = null;
    //        enemy.target = null;
    //    }
    //}
}
