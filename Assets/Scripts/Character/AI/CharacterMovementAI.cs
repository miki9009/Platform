using Engine;
using UnityEngine;
using AI;

public class CharacterMovementAI : CharacterMovement
{
    public PathMovement pathMovement;

    public Vector3[] path;
    protected int pathIndex = 0;
    Vector3 startPos;
    Vector3 Destination
    {
        get
        {
            return aIBehaviour.Destination;
        }
    }
    public Vector3 nextPoint;
    public AIBehaviour aIBehaviour;
    public AIState aiState;
    AIState currentState;
    bool initialized;
    public float nextPointminDistance = 1;
    public float ForwardPower
    {
        get
        {
            return forwardPower;
        }
        set
        {
            forwardPower = value;
        }
    }

    public override bool IsPlayer
    {
        get
        {
            return false;
        }
    }

    public override bool IsBot
    {
        get
        {
            return true;
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

    private void OnDestroy()
    {
        GameManager.LevelClear -= DestroyMe;
        aIBehaviour.Clear();
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
        if (dir == Vector3.zero) return;
        dir.y = 0;
        if (OnGround)
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * character.stats.turningSpeed);
        else
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * character.stats.turningSpeed);
        lastRot = transform.rotation;
    }

    protected override void Inputs()
    {
        //anim.SetBool("onGround", OnGround);
    }

    public virtual float MinDistanceToPoint
    {
        get
        {
            return nextPointminDistance;
        }
    }

    float dis;
    float timeBetweenJumps = 2;
    float curentTimeBetweenJumps;
    Quaternion lastRot;
    protected virtual void DoMovement()
    {
        if (aIBehaviour.Execute())
        {
            if (currentState != aiState)
            {
                ChangeState(AIState.Idle);
            }
            if (path != null && Vector3.Distance(transform.position, nextPoint) > MinDistanceToPoint)
            {
                forwardPower = 1;
                if (!aIBehaviour.Idle && rb.velocity.magnitude < stats.runSpeed / 3 && OnGround && Mathf.Abs(rb.velocity.y) < 5)
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
                if (OnGround && curentTimeBetweenJumps < timeBetweenJumps)
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
                    //Debug.Log("Path created");
                    nextPoint = transform.position;
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
        if(gameObject)
            Destroy(gameObject);
    }

    public void ClearPath()
    {
        path = null;
    }




#if UNITY_EDITOR
    public Color gizmoColor = Color.blue;
    protected virtual void OnDrawGizmos()
    {
        if (path != null && path.Length > 0)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(path[0], 1);
            for (int i = 1; i < path.Length; i++)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(path[i-1], path[i]);
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(path[i], 1);
            }


        }

    }

#endif

}

