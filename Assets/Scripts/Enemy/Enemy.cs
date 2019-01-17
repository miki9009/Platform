using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Engine;

public class Enemy : MonoBehaviour, IDestructible, IThrowableAffected, IStateAnimator, IPoolObject
{
    public LayerMask collisionLayer;
    public float offset = 10;
    public bool randomStartRotation;
    public float speed = 3;
    public PathMovement pathMovement;
    public float looseTargetDistance = 10;
    public float patrolDistance = 10;
    public float attackDistance = 3;

    [Label("Wait time before next path")]
    public float waitTime = 3;

    public int action = 0;

    protected Rigidbody rb;
    protected Vector3 startPos;
    public bool isAttacking;

    public Transform Transform { get { return transform; } set { Transform = value; } }

    public AnimatorBehaviour AnimatorBehaviour
    {
        get;set;
    }

    public Rigidbody Rigidbody
    {
        get
        {
            return rb;
        }
    }

    public GameObject GameObject
    {
        get { return gameObject; }
    }

    Collider[] colliders;

    /*[System.NonSerialized] */public Transform target;

    [System.NonSerialized] public Animator anim;
    public float attackTime = 1;

    public GameObject stars;

    ParticleSystem starsExplosion;
    private EnemyDeath enemyDeath;
    Vector3 startPosition;
    public bool dead = false;
    protected bool canAttack = false;
    protected int pathIndex = 0;
    protected Vector3[] path;

    void Awake()
    {
        colliders = GetComponents<Collider>();
    }

	protected virtual void Start ()
    {
        canAttack = true;
        startPosition = transform.position;
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        starsExplosion = StaticParticles.Instance.starsExplosion;
        enemyDeath = GetComponent<EnemyDeath>();
        startPos = transform.position;
        Vector3 destination = startPos;
        if(pathMovement!=null)
        {
            if (pathMovement.RandomPoint(startPos, patrolDistance, out destination))
            {
                path = pathMovement.GetPath(destination);
            }
            path = pathMovement.GetPath(pathMovement.GetRandomPointOnNavMesh());
            pathIndex = 0;
        }

 //       Debug.Log("Path points: " + path.Length);
    }

    protected virtual void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == Layers.Environment && other.contacts[0].point.y > transform.position.y + 0.1f)
        {
            path = pathMovement.GetPath(startPos);
            pathIndex = 0;
        }
    }


    protected float waitTimeCur;
    protected float pathUpdater = 1;
    protected float curTimeUpdater = 0;
    protected Vector3 prevPos;
    protected bool isColliding = true;
    protected float collidingTime;
    protected virtual void Update()
    {
        anim.SetFloat("hSpeed", rb.velocity.magnitude);
        if (target != null)
        {
            if (pathUpdater > curTimeUpdater)
            {
                curTimeUpdater += Time.deltaTime;
            }
            else
            {
                path = pathMovement.GetPath(target.position);
                pathIndex = 0;
                curTimeUpdater = 0;
            }
            var dis = Vector3.Distance(transform.position, target.position);
            if (dis > looseTargetDistance)
            {
                target = null;
            }
            else if(dis < attackDistance)
            {
                var dir = Vector.Direction(transform.position, target.position);
                dir.y = 0;
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), 0.1f);
                if (canAttack)
                {
                    Attack();
                }
            }
        }
        if (!isAttacking)
        {
            if (target == null && waitTimeCur > 0)
            {
                waitTimeCur -= Time.deltaTime;
                if(collidingTime > 0)
                {
                    collidingTime -= Time.deltaTime;
                }
                else
                {
                    isColliding = Physics.Raycast(new Ray(transform.position + Vector3.up, Vector3.down), 3, collisionLayer.value);
                    collidingTime = 1;
                }

                return;
            }
            if (path != null && Vector3.Distance(transform.position, path[pathIndex]) > 2)
            {
                //transform.rotation = Math.RotateTowardsTopDown(transform, path[pathIndex], Time.deltaTime * 5);
                var dir = Vector.Direction(transform.position, path[pathIndex]);
                dir.y = 0;
                if(dir != Vector3.zero)
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), 0.1f);
                float y = rb.velocity.y;
                Vector3 velo = transform.forward * speed;
                velo.y = y;
                rb.velocity = velo;
            }
            else
            {
                if (path!= null && pathIndex + 1 < path.Length)
                {
                    pathIndex++;
                }
                else
                {
                    Vector3 destination;
                    if (pathMovement.RandomPoint(startPos, patrolDistance, out destination))
                    {
                        //Debug.Log("Next path");
                        rb.velocity = Vector3.zero;
                        path = pathMovement.GetPath(destination);
                        waitTimeCur = waitTime;
                        pathIndex = 0;
                    }
                }
            }
        }
    }


    protected virtual void Attack()
    {
        //Debug.Log("Attack");
        canAttack = false;
        if (target == null || dead) return;
        anim.Play("Attack");
        Invoke("CanAttack", attackTime);
    }

    protected void CanAttack()
    {
        canAttack = true;
        if(!dead)
            anim.Play("Idle");
    }


    public void Hit(CharacterMovement character)
    {
        anim.SetTrigger("hit");
        dead = true;
        enemyDeath.StartCoroutine(enemyDeath.DestroyMe());
        enabled = false;
        if (colliders == null)
        {
            colliders = GetComponentsInChildren<Collider>();
            Debug.Log("Colliders were null could be a problem in Infinite mode.");
        }
        foreach (var col in colliders)
        {
            col.enabled = false;
        }
        rb.useGravity = false;
        //stars.SetActive(true);
        rb.velocity = Vector3.zero;
        starsExplosion.transform.position = transform.position;
        starsExplosion.Play();
        CollectionManager.Instance.SetCollection(character.character.ID, CollectionType.KillEnemy, 1);
    }

    public virtual void Recycle()
    {
        dead = false;
        enemyDeath.StopAllCoroutines();
        anim.Play("Idle");
        enabled = true;
        if(colliders == null)
        {
            colliders = GetComponentsInChildren<Collider>();
            Debug.Log("Colliders were null could be a problem in Infinite mode.");
        }
        if (colliders != null)
        {
            foreach (var col in colliders)
            {
                col.enabled = true;
            }
        }
        rb.useGravity = true;
        stars.SetActive(false);
        rb.velocity = Vector3.zero;
        starsExplosion.transform.position = transform.position;
        starsExplosion.Stop();
    }

    public void OnHit()
    {
        starsExplosion.Play();
        Hit(null);
    }


    protected int attackHashName = Animator.StringToHash("Attack");

    public virtual void StateAnimatorInitialized()
    {
        AnimatorBehaviour.StateEnter += (animatorStateInfo) =>
        {
            if (animatorStateInfo.shortNameHash == attackHashName)
            {
                isAttacking = true;
            }
        };

        AnimatorBehaviour.StateExit += (animatorStateInfo) =>
        {
            if (animatorStateInfo.shortNameHash == attackHashName)
            {
                isAttacking = false;
            }
        };
    }

#if UNITY_EDITOR
    //void OnGUI()
    //{
    //    Draw.TextColor(10, 300, 255, 255, 255, 1, "Velocity: " + rb.velocity);
    //}

    public Color gizmoColor = Color.blue;
    private void OnDrawGizmos()
    {
        if (path != null && path.Length > 0)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transform.position, path[pathIndex]);
        }
    }


#endif
}
