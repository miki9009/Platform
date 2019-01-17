using UnityEngine;

public class Bridge : MonoBehaviour
{
    public LineRenderer lineRenderer;

    public Transform[] anchors;

    private void Awake()
    {
        lineRenderer.enabled = true;
    }


    private void Update()
    {
        for (int i = 0; i < anchors.Length; i++)
        {
            lineRenderer.SetPosition(i, anchors[i].position);
        }
    }
}