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

    WorldLevel nextLevel;

    private void Awake()
    {
        WorldPointer.Click += WorldPointer_Click;
        World.AddWorldLevel(this);
    }

    private void Start()
    {
        nextLevel = World.FindTargetLevel(this);
        if(nextLevel)
        {
            SetLineRendererOnTarget();
        }
    }

    private void OnDestroy()
    {
        WorldPointer.Click -= WorldPointer_Click;
    }

    private void WorldPointer_Click(Transform t)
    {
        if(t == transform)
        {

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody != null)
        {
            var character = other.attachedRigidbody.GetComponentInParent<Character>();
            if (character != null && character.IsLocalPlayer)
                GoToLevelAdditive();
        }
    }

    void GoToLevelAdditive()
    {
        LevelManager.BeginCustomLevelLoadSequenceAdditive(mission);
    }

    void SetLineRendererOnTarget()
    {
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, nextLevel.transform.position);
        lineRenderer.startColor
    }


}

