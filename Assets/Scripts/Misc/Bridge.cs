using UnityEngine;
using Engine;

//[ExecuteInEditMode]
public class Bridge : LevelElement
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

    public override void OnLoad()
    {
        base.OnLoad();
        gameObject.SetActive(true);
    }

}