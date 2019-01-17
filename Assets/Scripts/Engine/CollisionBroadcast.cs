using UnityEngine;


public class CollisionBroadcast : MonoBehaviour
{
    public event System.Action<Collision> CollisionEntered;
    void OnCollisionEnter(Collision collision)
    {
        //       Debug.Log("Collision Registered");
        CollisionEntered?.Invoke(collision);
    }

    public event System.Action<Collision> CollisionExit;
    void OnCollisionExit(Collision collision)
    {
        //       Debug.Log("Collision Registered");
        CollisionExit?.Invoke(collision);
    }
}