using Engine;
using UnityEngine;
using AI;

public class AISpawner : LevelElement
{
    public GameObject aiPrefab;
    public AIState state;

    GameObject ai;

    private void Start()
    {
        ai = Instantiate(aiPrefab, transform.position, transform.rotation);
        var script = ai.GetComponent<CharacterMovementAI>();
        script.ChangeState(state);
    }

    public override void OnSave()
    {
        base.OnSave();
        if(data!=null)
        {
            data["State"] = state;
        }
    }

    public override void OnLoad()
    {
        base.OnLoad();
        if(data!=null)
        {
            if(data.ContainsKey("State"))
            {
                state = (AIState)data["State"];
            }
        }
    }

    protected override void OnLevelClear()
    {

        if(ai != null)
        {
            Destroy(ai);
        }
        base.OnLevelClear();
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, 1);
    }
#endif
}