using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public float speed;
    public Vector3 angle;

    public GameObject prefab;

    public Points points;


    private void Start()
    {
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        angle = angle.normalized;
        if (points == null)
        {
            points = new Points();
            points.points = new List<Vector3>();
        }
        foreach (var point in points.points)
        {
            var obj = Instantiate(prefab, point, Quaternion.identity, transform);
            var rigid = obj.AddComponent<Rigidbody>();
            rigid.useGravity = false;
        }
    }

    // Update is called once per frame
    void FixedUpdate ()
    {
        transform.Rotate(angle * speed * Time.deltaTime);
	}

#if UNITY_EDITOR
    Color color = new Color(0, 0, 1, 0.3f);
    Color color2 = new Color(1, 0, 1, 0.3f);
    private void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawSphere(transform.position, 3);
        Gizmos.color = color2;
        if (points != null)
        {
            if (points.points != null)
            {
                foreach (var point in points.points)
                {
                    Gizmos.DrawSphere(point, 1);
                }
            }
        }
    }
#endif
    [Serializable]
    public class Points
    {
        public List<Vector3> points;
        public int pointsNum;
        public float distance;
    }
}
