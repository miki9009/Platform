using Engine;
using UnityEngine;
using AI;

public class CharacterMovementAI : CharacterMovement
{
    public PathMovement pathMovement;

    public Vector3[] path;
    int pathIndex = 0;
    Vector3 startPos;
    Vector3 Destination
    {
        get
        {
            return aIBehaviour.Destination;
        }
    }
    Vector3 nextPoint;
    public AIBehaviour aIBehaviour;
    public AIState aiState;
    AIState currentState;
    bool initialized;

    public override bool IsPlayer
    {
        get
        {
            return false;
        }
    }

    protected override void Initialize()
    {
        if (initialized) return;
        GameManager.LevelClear += DestroyMe;
        initialized = true;
        startPos = transform.position;
        Movement = DoMovement;
        nextPoint = transform.position+transform.forward;
        aIBehaviour = new AIBehaviour(this);
        aIBehaviour.AssignState(aiState);
        currentState = aiState;
        path = pathMovement.GetPath(Destination);
    }

    public void ChangeState(AIState state)
    {
        currentState = state;
        Initialize();
        aIBehaviour.AssignState(state);
    }

    protected override void Rotation()
    {
        if (currentState == AIState.Idle) return;
        var dir = Vector.Direction(transform.position, nextPoint);
        dir.y = 0;
        if (onGround)
            transform.rotation = Quaternion.LookRotation(dir);
        else
            transform.rotation = lastRot;
        lastRot = transform.rotation;
    }

    protected override void Inputs()
    {
        anim.SetBool("onGround", onGround);
    }
    float dis;
    float timeBetweenJumps = 2;
    float curentTimeBetweenJumps;
    Quaternion lastRot;
    void DoMovement()
    {
        if (aIBehaviour.Execute())
        {
            if (currentState != aiState)
            {
                ChangeState(AIState.Idle);
            }
            if (path != null && Vector3.Distance(transform.position, nextPoint) > 1f)
            {
                //transform.rotation = Math.RotateTowardsTopDown(transform, path[pathIndex], Time.deltaTime * 5);
                forwardPower = 1;
                if (!aIBehaviour.Idle && rb.velocity.magnitude < stats.runSpeed / 3 && onGround && Mathf.Abs(rb.velocity.y) < 5)
                {
                    if (curentTimeBetweenJumps >= timeBetweenJumps)
                    {
                        jumpInput = 1;
                        curentTimeBetweenJumps = 0;
                        //Debug.Log("Jumped");
                    }
                    else
                    {
                        jumpInput = 0;
                    }
                }
                if (onGround && curentTimeBetweenJumps < timeBetweenJumps)
                {
                    curentTimeBetweenJumps += Time.deltaTime;
                }
            }
            else
            {
                forwardPower = 0;
                if (path != null && pathIndex + 1 < path.Length)
                {
                    pathIndex++;
                    nextPoint = path[pathIndex];
                }
                else
                {
                    path = pathMovement.GetPath(Destination);
                    pathIndex = 0;
                }
            }
        }
        else if(currentState != AIState.Idle)
        {
            ChangeState(AIState.Idle);
            forwardPower = 0;
        }
    }

    void DestroyMe()
    {
        Destroy(gameObject);
    }

    public void ClearPath()
    {
        path = null;
    }

#if UNITY_EDITOR
    public Color gizmoColor = Color.blue;
    private void OnDrawGizmos()
    {
        if (path != null && path.Length > 0)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, nextPoint);
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(Destination, 1);

        }

    }

#endif

}

