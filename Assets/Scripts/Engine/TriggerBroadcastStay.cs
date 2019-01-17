using UnityEngine;

public class TriggerBroadcastStay : MonoBehaviour
{
    public event System.Action<Collider> TriggerEnter;
    private void OnTriggerEnter(Collider other)
    {
        TriggerEnter?.Invoke(other);
    }

    public event System.Action<Collider> TriggerStay;
    void OnTriggerStay(Collider other)
    {
        //       Debug.Log("Collision Registered");
        TriggerStay?.Invoke(other);
    }

    public event System.Action<Collider> TriggerExit;
    void OnTriggerExit(Collider other)
    {
        //       Debug.Log("Collision Registered");
        TriggerExit?.Invoke(other);
    }

}