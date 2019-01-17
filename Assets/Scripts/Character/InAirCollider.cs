using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InAirCollider : MonoBehaviour
{
    public int collisions = 0;
    public bool collision;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 10)
        {
            collisions++;
            collision = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 10)
        {
            collision = false;
        }
    }

}
