using UnityEngine;

using Engine;

public class Buoy : LevelElement
{

    public BoxCollider boxCollider;


    public override void OnLoad()
    {
        base.OnLoad();
        if(data.ContainsKey("Center"))
        {
            boxCollider.center = (Float3)data["Center"];
            boxCollider.size = (Float3)data["Size"];
        }

    }

    public override void OnSave()
    {
        base.OnSave();
        data["Center"] = (Float3)boxCollider.center;
        data["Size"] = (Float3)boxCollider.size;
    }

#if UNITY_EDITOR
    public Color color = Color.red;
    public bool draw = true;

    public void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = Color.red;
        Gizmos.DrawCube(Vector3.zero, boxCollider.size);
    }
#endif
}