using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PathMovement : MonoBehaviour
{

    Transform targetPrev;

    public Vector3[] pathPoints;
    Vector3 nextPathPoint;

    int pathPointIndex;

    public float followDistance;
    public bool isClose;

 
    float lookDot;

    [Header("Debug Values")]
    public Vector3 localVelocity;
    public float localVelocityMag;

    private NavMeshQueryFilter queryFilter;
    private NavMeshPath path;
    public Vector3 direction;

    private System.Random rndGenerator;

    Vector3 startPos;
    Vector3 curPos;

    public Vector3 Direction
    {
        get
        {
            return direction;
        }
    }

    public delegate void TargetPoint();
    public event TargetPoint OnReachDestination;
    public bool noPath = false;

    void Awake()
    {
        startPos = transform.position;
        curPos = startPos;
        rndGenerator = new System.Random(Mathf.Abs(System.Environment.TickCount + gameObject.GetHashCode()));

        queryFilter = new NavMeshQueryFilter() { agentTypeID = 0, areaMask = 1 };
        path = new NavMeshPath();
        GetPath();
    }

    public void Inputs(out float horInput, out float verInput)
    {
        horInput = 0;
        verInput = 0;
        curPos = transform.position;

        if (pathPoints != null && pathPoints.Length > 0)
        {
            float followDistanceSqr = Mathf.Max(Mathf.Pow(followDistance, 2), 1f);
            if ((curPos - nextPathPoint).sqrMagnitude <= followDistanceSqr)
            {
                pathPointIndex++;
                if (pathPointIndex < pathPoints.Length)
                {
                    nextPathPoint = pathPoints[pathPointIndex];
                }
            }

            isClose = (curPos - nextPathPoint).sqrMagnitude <= followDistanceSqr;
            direction = (nextPathPoint - curPos).normalized;
            if (!isClose)
            {
                verInput = 1;
            }
            else
            {
                verInput = 0f;
            }
        }
        var right = transform.right;
        horInput = -(Vector3.Angle(right, direction) > 180 ? Vector3.Dot(right, direction) : -Vector3.Dot(right, direction));
    }

    private void SpawnOnNavMesh()
    {
        transform.position = GetRandomPointOnNavMesh() + Vector3.up * 1.5f;
    }

    public Vector3[] GetPath(Vector3 targetPoint)
    {
        Vector3 startPoint = transform.position;
        if (NavMesh.CalculatePath(GetNavMeshPosition(startPoint), GetNavMeshPosition(targetPoint), queryFilter, path))
        {
            if (path.status == NavMeshPathStatus.PathComplete)
            {
                //Debug.Log("FOUND PATH");
                return SetupPath(path.corners);
            }
            else
            {
                noPath = true;
            }
        }
        //Debug.Log("DIDN'T FIND PATH");
        return new Vector3[] { transform.position };
    }



    public void GetPath()
    {
        var newTargetPosition = GetRandomPointOnNavMesh();
        GetPath(newTargetPosition);
    }

    public Vector3 GetRandomPointOnNavMesh()
    {
        var data = NavMesh.CalculateTriangulation();
        int index = 0;
        if(data.indices.Length >= 3)
        {
            index = rndGenerator.Next(0, data.indices.Length - 3);
        }
        
        index = index - (index % 3);
        Vector3 point = Vector3.Lerp(data.vertices[data.indices[index]], data.vertices[data.indices[index + 1]], (float)rndGenerator.NextDouble());
        point = Vector3.Lerp(point, data.vertices[data.indices[index + 2]], (float)rndGenerator.NextDouble());
        return point;
    }

    public bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * range + Vector3.up;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 5.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }
        result = Vector3.zero;
        return false;
    }

    private Vector3 GetNavMeshPosition(Vector3 position)
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(position, out hit, 20f, queryFilter))
        {
            return hit.position;
        }
        else
        {
            return Vector3.zero;
        }
    }

    private Vector3[] SetupPath(Vector3[] pathPoints)
    {
        List<Vector3> points = new List<Vector3>();
        float sqrDistance = Mathf.Pow(followDistance * 1.2f, 2f);
        foreach (var p in pathPoints)
        {
            if ((p - transform.position).sqrMagnitude > sqrDistance)
            {
                points.Add(p);
            }
        }
        this.pathPoints = points.ToArray();
        pathPointIndex = 0;
        if (this.pathPoints.Length > 0)
        {
            nextPathPoint = this.pathPoints[pathPointIndex];
        }
        return pathPoints;
    }


//#if UNITY_EDITOR
//	public float gizmoSize = 0.1f;
//	public Color pathColor;



//    void OnDrawGizmos ()
//	{
//		if (pathPoints != null && pathPoints.Length > 0) 
//		{
//			Gizmos.color = pathColor;
//			Vector3 prev = pathPoints [0];
//			Gizmos.DrawSphere (prev, gizmoSize * UnityEditor.HandleUtility.GetHandleSize (prev));
//			for (int i = 1; i < pathPoints.Length; i++) 
//			{
//				Vector3 next = pathPoints [i];
//				Gizmos.DrawSphere (next, gizmoSize * UnityEditor.HandleUtility.GetHandleSize (next));
//				prev = next;
//			}
//			Gizmos.color = Color.red;
//			Gizmos.DrawLine (transform.position, nextPathPoint);
//		}
//		if (localVelocityMag > 0) 
//		{
//			Gizmos.color = Color.yellow;
//			Gizmos.DrawLine (transform.position, transform.position + transform.TransformDirection (localVelocity));
//		}
			
//	}
//#endif
}
