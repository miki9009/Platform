using Engine;
using UnityEngine;

public class CharacterBoatAI : CharacterMovementAI
{
    public ParticleSystem waterDrops;
    public float scooterSpeed = 30;
    public override bool IsPlayer
    {
        get
        {
            return false;
        }
    }

    float speedModifier;
    protected override void Initialize()
    {
        base.Initialize();
        anim.Play("Scooter");
        character.stats.turningSpeed = character.stats.turningSpeed * 1.25f;
        speedModifier = Random.Range(1.3f, 1.7f);
        character.stats.runSpeed = 30;
    }

    public override void Die()
    {
        // base.Die();
    }

    public override void Attack()
    {
        //base.Attack();
    }

    public override void StateAnimatorInitialized()
    {
        //base.StateAnimatorInitialized();
    }

    public override void SetAnimationHorizontal(Vector3 velo)
    {
        // base.SetAnimationHorizontal(velo);
    }

    public override void Hit(Enemy enemy = null, int hp = 1, bool heavyAttack = false)
    {
        // base.Hit(enemy, hp, heavyAttack);
    }

    float mag;
    public override void Move()
    {
        var velo = rb.velocity;
        float y = velo.y;
        velo.y = 0;
        mag = velo.magnitude;
        float pwr = 0;
        pwr = forwardPower;

        rb.rotation = Quaternion.Lerp(rb.rotation, transform.rotation, Time.deltaTime);
        rb.AddForce(rb.rotation.Vector() * pwr * speedModifier * iceForce, ForceMode.Acceleration);
        if (OnGround)
            rb.AddForce(Vector3.up * pwr * mag / 40, ForceMode.VelocityChange);
        if (OnGround && Engine.Math.Probability(mag / scooterSpeed))
        {
            waterDrops.Emit(4);
        }

    }

    public override float MinDistanceToPoint
    {
        get
        {
            if(path!=null && pathIndex < path.Length - 1)
            {
                return 15;
            }
            return nextPointminDistance;
        }
    }

    public float angleToTarget;
    public float localVelocity;
    public float disToTarget;
    bool braking;
    protected override void DoMovement()
    {
        base.DoMovement();
        localVelocity = rb.velocity.magnitude;
        angleToTarget = Vector3.Angle(transform.forward, Vector.Direction(transform.position, nextPoint));
        disToTarget = Vector3.Distance(transform.position, nextPoint);
        if (disToTarget < 15 && localVelocity > 5 && angleToTarget > 10)
        {
            forwardPower = 0;
            braking = true;
        }
        else
        {
            braking = false;
        }
    }

    protected Vector3 TargetEuler(float angle)
    {
        return new Vector3(0, transform.eulerAngles.y + angle, 0);
    }

//    protected override void Rotation()
//    {
//        var vec = Controller.Instance.gameCamera.transform.forward;
//        vec.y = 0;
//        Quaternion rot = Quaternion.Euler(TargetEuler(angle));
//        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * stats.turningSpeed / 2);
//#if (UNITY_EDITOR)
//        if (angle > 180)
//            angle = -90;
//        angle = Mathf.Clamp(angle, -maxAngle, maxAngle);
//#endif
//        float factor = angle / 90;

//        modelZFactor = Mathf.Clamp(Mathf.Lerp(modelZFactor, factor * 90, Time.deltaTime * stats.turningSpeed / 2), -30, 30);
//        var euler = model.transform.localEulerAngles;
//        model.transform.localEulerAngles = new Vector3(-scooterSpeed * mag / iceForce, 0, -modelZFactor);
//    }

    protected override void FixedUpdate()
    {
        Move();
        Rotation();
    }
#if UNITY_EDITOR
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        if (forwardPower > 0)
            Gizmos.DrawSphere(transform.position, 3);
    }
#endif
}