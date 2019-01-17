using System.Collections.Generic;
using UnityEngine;

public class SpawningPlace : MonoBehaviour
{
    public Transform[] spawns;

    public Transform[] rowLeft;
    public Transform[] rowCenter;
    public Transform[] rowRight;

    public Stack<IPoolObject> activeSpawns;

    private void Awake()
    {
        activeSpawns = new Stack<IPoolObject>();
    }

    public void RecycleActiveSpawns()
    {
        foreach (var obj in activeSpawns)
        {
            if (obj == null) continue;
            obj.Recycle();
        }

        activeSpawns.Clear();
    }


#if UNITY_EDITOR
    public Color gizmosColor = Color.green;
    private void OnDrawGizmos()
    {
        if (spawns != null)
        {
            Gizmos.color = gizmosColor;
            for (int i = 0; i < spawns.Length; i++)
            {
                if (spawns[i] != null)
                    Gizmos.DrawCube(spawns[i].position, new Vector3(2, 5, 2));
            }
        }
    }
#endif
}//4 1 -2