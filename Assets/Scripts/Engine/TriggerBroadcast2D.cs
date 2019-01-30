using UnityEngine;


public class TriggerBroadcast2D : MonoBehaviour
{
    public event System.Action<Collider2D> TriggerEntered;
    public event System.Action<Collider2D> TriggerExit;
    void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log("OnTriggerEnter: "+ collider.name);
        TriggerEntered?.Invoke(collider);
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        Debug.Log("OnTriggerExit: " + collider.name);
        TriggerExit?.Invoke(collider);
    }
}