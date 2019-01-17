using UnityEngine;

public class MonkeyThrowable : Enemy
{
    public TriggerBroadcast triggerBroadcast;
    float curAttackTimer = 0;
    public GameObject coconutPrefab;

    public int coconuts = 4;

    GameObject[] poolCoconuts;
    int coconutIndex = 0;
    public Transform hand;
    public float coconutSpeed;
    public float throwAfterTime = 1.5f;

    void Awake()
    {
        triggerBroadcast.TriggerEntered += (Collider col) =>
        {
            curAttackTimer = attackTime - 0.5f;
            target = col.transform;
        };
        triggerBroadcast.TriggerExit += (Collider col) =>
        {
            target = null;
        };

        poolCoconuts = new GameObject[coconuts];
        for (int i = 0; i < coconuts; i++)
        {
            var coco = Instantiate(coconutPrefab);
            poolCoconuts[i] = coco;
            coco.GetComponent<MonkeyCoconut>().Parent = gameObject;
            coco.SetActive(false);
        }
    }

    protected int dieHashName = Animator.StringToHash("Die");
    public override void StateAnimatorInitialized()
    {
        AnimatorBehaviour.StateEnter += (animatorStateInfo) =>
        {
            if (animatorStateInfo.shortNameHash == dieHashName)
            {
                var rigid = GetComponent<Rigidbody>();
                rigid.isKinematic = false;
                rigid.constraints = RigidbodyConstraints.None;
                rigid.useGravity = true;
                var coll = GetComponent<Collider>();
                coll.enabled = true;
            }
        };

        AnimatorBehaviour.StateEnter += (animatorStateInfo) =>
        {
            if (animatorStateInfo.shortNameHash == attackHashName)
            {
                isAttacking = true;
                Invoke("Throw", throwAfterTime);
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

    Vector3 dirToTarget;
    protected override void Update()
    {
        if(target != null)
        {
            dirToTarget = Engine.Vector.Direction(startPos, new Vector3(target.position.x, target.position.y + Random.Range(1f,4f), target.position.z));
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dirToTarget), Time.deltaTime);
            if (curAttackTimer < attackTime)
            {
                curAttackTimer += Time.deltaTime;
            }
            else
            {
                curAttackTimer = 0;
                Attack();
            }
        }
    }

    public void Throw()
    {
        var c = poolCoconuts[coconutIndex];
        c.SetActive(true);
        c.transform.position = hand.position;
        c.GetComponent<Rigidbody>().velocity = dirToTarget * coconutSpeed;
        c.GetComponent<MonkeyCoconut>().Parent = gameObject;
        if (coconutIndex < poolCoconuts.Length-1)
        {
            coconutIndex++;
        }
        else
        {
            coconutIndex = 0;
        }
    }

    //protected override void OnCollisionEnter(Collision other)
    //{
        
    //}


}