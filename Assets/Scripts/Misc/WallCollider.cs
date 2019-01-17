using UnityEngine;

public class WallCollider : MonoBehaviour
{

#if UNITY_EDITOR
    BoxCollider boxCollider;
    public Color color = Color.red;
    public bool draw = true;

    private void OnDrawGizmos()
    {
        if (!draw) return;
        if(boxCollider == null)
        {
            boxCollider = GetComponent<BoxCollider>();
        }
        else
        {
            Gizmos.color = color;
            Gizmos.DrawCube(transform.position + boxCollider.center, boxCollider.size);
        }
    }
#endif
}