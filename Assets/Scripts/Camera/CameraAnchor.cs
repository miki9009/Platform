using UnityEngine;

public class CameraAnchor : MonoBehaviour
{
    public int index;
    public Transform anchorTransform;

    private void Awake()
    {
        GetComponentInChildren<Camera>().enabled = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position,1);
    }
}