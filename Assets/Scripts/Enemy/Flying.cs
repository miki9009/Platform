
using Engine;
using UnityEngine;

public class Flying : Enemy
{
    protected override void Start()
    {
        base.Start();
        destination = transform.position;
        aimedPos = transform.position;
    }

    public override void Recycle()
    {
        
    }

    protected override void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == collisionLayer.value)
        {
            waitTimeCur = 0;
            GetDestination();
            Debug.Log("Change Pos");
        }
    }


    int frame = 0;
    float curSpeed;
    Vector3 destination;
    Vector3 aimedPos;
    protected override void Update()
    {

    }

    private void FixedUpdate()
    {
        anim.SetFloat("hSpeed", speed);
        if (target != null)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(Vector.Direction(transform.position, new Vector3(target.position.x, transform.position.y, target.position.z))), 0.1f);
            aimedPos += transform.forward * speed * Time.deltaTime;
            if (Vector3.Distance(transform.position, target.position) < attackDistance)
            {
                if (!isAttacking)
                    Attack();
            }
        }
        else
        {
            if (Vector3.Distance(transform.position, destination) > 3)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(Vector.Direction(transform.position, new Vector3(destination.x, transform.position.y, destination.z))), 0.1f);
                aimedPos += transform.forward * speed * Time.deltaTime;
            }
            else
            {
                GetDestination();
            }
        }
        if (frame % 10 == 0)
        {
            RaycastHit hit;
            if (Physics.Raycast(new Ray(aimedPos + Vector3.up * 4, Vector3.down), out hit, 100, collisionLayer.value))
            {
                var pos = aimedPos;
                pos.y = hit.point.y;
                aimedPos = pos;
            }
        }
        transform.position = Vector3.Lerp(transform.position, aimedPos, Time.deltaTime);
        prevPos = transform.position;
    }

    void GetDestination()
    {
        RaycastHit hit;
        if (Physics.Raycast(startPos + Vector3.up * 5 + new Vector3(Random.Range(-patrolDistance, patrolDistance), 0, Random.Range(-patrolDistance, patrolDistance)),
            Vector3.down, out hit, 100, collisionLayer.value, QueryTriggerInteraction.Ignore))
        {
            destination = hit.point;
            waitTimeCur = waitTime;
        }
    }

}