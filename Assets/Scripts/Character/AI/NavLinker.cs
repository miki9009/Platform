using UnityEngine;


public class NavLinker : MonoBehaviour
{
    public Transform point1;
    public Transform point2;
    public UnityEngine.AI.OffMeshLink link;

#if UNITY_EDITOR
    public bool draw = false;
    private void OnDrawGizmos()
    {
        if (!draw) return;
        Gizmos.DrawSphere(point1.transform.position, 1);
        Gizmos.DrawSphere(point2.transform.position, 1);
    }
#endif
}