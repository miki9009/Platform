using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Engine;

public class Blob : Enemy
{
    public float attackForce = 100;
    public float chargeTime = 5;
    float chTime;

    protected override void Start()
    {
        base.Start();
        chTime = chargeTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == Layers.Environment)
        {
            isAttacking = false;
            waitTimeCur = waitTime;
        }

    }

    protected override void Update()
    {
        if (waitTimeCur > 0)
        {
            anim.SetFloat("hSpeed", 0);
            waitTimeCur -= Time.deltaTime;
            var dir = Vector.Direction(transform.position, path[pathIndex]);
            dir.y = 0;
            if(dir != Vector3.zero)
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), 0.1f);
            return;
        }
        if (isAttacking)
        {
            float y = rb.velocity.y;
            anim.SetFloat("hSpeed", speed * 2);
            Vector3 velo = transform.forward * speed * 2;
            velo.y = y;
            rb.velocity = velo;
            float dis = 0;
            if (target != null)
            {
                dis = Vector3.Distance(transform.position, target.position);
            }

            chTime -= Time.deltaTime;
            if (dis < 1.5f || chTime <= 0)
            {
                isAttacking = false;
                waitTimeCur = waitTime;
                anim.Play("Idle");
            }
        }
        else
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
                else if (dis < attackDistance)
                {
                    isAttacking = true;
                    anim.SetTrigger("attack");
                    var dir = Vector.Direction(transform.position, target.position);
                    dir.y = 0;
                    transform.rotation = Quaternion.LookRotation(dir);
                    chTime = chargeTime;
                }
            }
            if (!isAttacking)
            {
                if (target == null && waitTimeCur > 0)
                {
                    waitTimeCur -= Time.deltaTime;
                    if (collidingTime > 0)
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
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), 0.1f);
                    float y = rb.velocity.y;
                    Vector3 velo = transform.forward * speed;
                    velo.y = y;
                    rb.velocity = velo;
                }
                else
                {
                    if (path != null && pathIndex + 1 < path.Length)
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

    }

    protected override void Attack()
    {
        canAttack = false;
        if (target == null || dead) return;
    }

}
