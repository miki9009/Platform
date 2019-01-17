using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MovingTransform : MonoBehaviour
{
    
    public float speed = 5;
    public float turningSpeed = 15;
    [HideInInspector] public List<Vector3> points;
    public bool rotate;
    public Vector3 rotationAngle;
    public Transform anchor;
    

    [HideInInspector]
    public Transform point;
    [HideInInspector]
    public List<Transform> anchors;
    [HideInInspector]
    public float distance = 1;
    [HideInInspector]
    public float prevScale = 1;
    [HideInInspector]
    public int numberOfPoints = 1;


    private void Start()
    {
        if (Application.isPlaying)
        {
            StartCoroutine(Move());
        }
    }

    public IEnumerator Move()
    {
        while (true)
        {
            if (points != null && anchor != null)
            {
                if (points.Count > 0)
                {
                    for (int i = 0; i < points.Count; i++)
                    {
                        while (Vector3.Distance(anchor.position, points[i]) > turningSpeed * Time.deltaTime)
                        {
                            //Engine.Instance.MoveTowardsPointRotation(anchor, anchor.position, points[i], speed * Time.deltaTime, turningSpeed * Time.deltaTime);
                            anchor.position = Vector3.MoveTowards(anchor.position, points[i], speed * Time.deltaTime);
                            yield return null;
                        }
                        yield return null;
                    }
                }
                else
                {
                    while (points.Count == 0)
                    {
                        if (rotate)
                        {
                            anchor.Rotate(rotationAngle * Time.deltaTime * speed);
                        }
                        yield return null;
                    }
                }
            }
            yield return null;
        }
    }
        

    public void EndLoop()
    {
        StopAllCoroutines();
    }

#if UNITY_EDITOR
    [HideInInspector]
    public bool isSimulating;
    void OnDrawGizmos()
    {

        if (anchors != null)
        {
            Gizmos.color = Color.red;
            for (int i = 0; i < anchors.Count; i++)
            {
                if (anchors[i] != null)
                {
                    Gizmos.DrawSphere(anchors[i].position, 0.25f);
                }
            }
        }
        if (anchor != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(anchor.position, 0.5f);
        }
    }
#endif

}
