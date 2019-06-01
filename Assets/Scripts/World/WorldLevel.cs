using Engine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldLevel : MonoBehaviour
{
    [MissionSelector]
    public string mission;
    public LineRenderer lineRenderer;
    public SphereCollider sphereCollider;
    public Color reachedColor;
    public Color notReachedColor;
    public int index;
    public Material mat_ON;
    public Material mat_OFF;
    public MeshRenderer meshRenderer;
    public bool active;

    public WorldLevel nextLevel;

    private void Awake()
    {
        WorldPointer.Click += WorldPointer_Click;
        World.AddWorldLevel(this);
    }

    private void Start()
    {

        SetValues();
    }

    private void OnDestroy()
    {
        WorldPointer.Click -= WorldPointer_Click;
    }

    private void WorldPointer_Click(Transform t)
    {
        if(active &&  t == transform)
        {
            Engine.UI.UIWindow.GetWindow("WorldWindowNew").Hide();
            GoToLevelAdditive();
        }
    }

    void GoToLevelAdditive()
    {
        GameManager.MissionIndex = index;
        LevelManager.BeginCustomLevelLoadSequenceAdditive(mission);
    }

    void SetValues()
    {
        if(index < World.CurrentIndex+1)
        {
            lineRenderer.startColor = reachedColor;
            lineRenderer.endColor = reachedColor;
            meshRenderer.material = mat_ON;
            active = true;
        }
        else
        {
            lineRenderer.startColor = notReachedColor;
            lineRenderer.endColor = notReachedColor;
            meshRenderer.material = mat_OFF;
            active = false;
        }
        if (nextLevel)
        {
            lineRenderer.SetPosition(0, transform.position + lineRenderer.transform.localPosition);
            lineRenderer.SetPosition(1, nextLevel.transform.position + lineRenderer.transform.localPosition);
        }
        else
        {
            lineRenderer.enabled = false;
        }

    }


}

