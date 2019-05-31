using UnityEngine;
using Engine;

public class WallCollider : Scenery
{
    public BoxCollider boxCollider;

    public override void OnSave()
    {
        base.OnSave();
        data["ColliderSize"] = (Float3)boxCollider.size;
        data["ColliderCenter"] = (Float3)boxCollider.center;
    }

    public override void OnLoad()
    {
        base.OnLoad();
        if(data.ContainsKey("ColliderSize"))
            boxCollider.size = (Float3)data["ColliderSize"];
        if(data.ContainsKey("ColliderCenter"))
            boxCollider.center = (Float3)data["ColliderCenter"];
    }


#if UNITY_EDITOR
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