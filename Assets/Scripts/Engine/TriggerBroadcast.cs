using UnityEngine;


public class TriggerBroadcast : MonoBehaviour
{
    public event System.Action<Collider> TriggerEntered;
    public event System.Action<Collider> TriggerExit;
    void OnTriggerEnter(Collider collider)
    {
        TriggerEntered?.Invoke(collider);
    }

    void OnTriggerExit(Collider collider)
    {
        TriggerExit?.Invoke(collider);
    }
}