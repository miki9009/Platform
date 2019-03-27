using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Engine;
using Engine.UI;

public abstract class CharacterMovement : MonoBehaviour, IThrowable, IStateAnimator, IDestructible
{
    public bool onGround = true;
    public ParticleSystem smoke2;
    public Transform model;
    public LayerMask enemyLayer;
    public SphereCollider activationCollider;
    public ParticleSystem attackParticles;
    [NonSerialized]
    public bool movementEnabled = true;
    [HideInInspector]
    public Character character;
    [HideInInspector] public CharacterHealth characterHealth;
    public Action MeleeAttack;
    public Action StopAttack;
    public event Action AttackBroadcast;
    public event Action DieBroadcast;
    public event Action<IThrowable, Vector3> Thrown;
    public float addForce = 1;
    public bool attack;
    public bool isAttacking = false;
    [NonSerialized]
    public Rigidbody rb;
    public Vector3 velocity;
    public float pipeFactor;
    [NonSerialized]
    public Animator anim;
    public float meleeAttackRadius = 2;

    float startAttackRadius;

    public Transform powerUpAnchor;
    public bool Initialized
    {
        get;private set;
    }

    public float airForce = 5;

    public bool Invincible { get; set; }
    public ThrowableObject ThrowObject { get; set; }
    public Vector3 StartPosition { get; private set; }
    public bool ButtonsInput { get; set; }
    public virtual bool IsBot
    {
        get
        {
            return false;
        }
    }

    public AnimatorBehaviour AnimatorBehaviour
    {
        get; set;
    }


    public bool IsLocalPlayer
    {
        get
        {
            return character.IsLocalPlayer;
        }
    }

    protected CharacterStatistics stats;
    protected float verInput = 0;
    protected float horInput = 0;
    protected float jumpInput = 0;
    protected float forwardPower;
    protected Action Movement;

    ParticleSystem smokeExplosion;
    ParticleSystem starsExplosion;
    protected Vector3 curPos;


    //ANIMATIONS
    int throwAnimationHash;
    int attackAnimationHash;
    float disY;
    float timeLastJump = 0;
    bool jumpReleased = true;
    float modelZ = 0;
    float hspeed;
    float vspeed;

    private BoxCollider jumpCollider;
    [NonSerialized]
    public ParticleSystem smoke;

    protected Vector3 targetEuler;

    protected abstract void Initialize();
    protected abstract void Inputs();
    protected abstract void Rotation();

    public bool isRemoteControl { get; set; }
    public abstract bool IsPlayer { get; }

    public Transform Transform
    {
        get { return transform; }
    }

    public Rigidbody Rigidbody { get { return rb; } }

    private void Awake()
    {
        character = GetComponent<Character>();
        jumpCollider = GetComponent<BoxCollider>();
        smoke = GetComponentInChildren<ParticleSystem>();
        StartPosition = transform.position;
        disY = Screen.height / 8;
    }

    // Use this for initialization
    void Start ()
    {
        curPos = transform.position;
        stats = character.stats;
        rb = character.rb;
        anim = character.anim;
        smokeExplosion = StaticParticles.Instance.smokeExplosion;
        starsExplosion = StaticParticles.Instance.starsExplosion;

        startAttackRadius = meleeAttackRadius;

        if (isRemoteControl)
            enabled = false;
        else
            Initialize();

        Initialized = true;
    }

    public void OnTriggerExit(Collider other)
    {
        onGround = false;
        smoke.Stop();
    }

    public virtual void Die()
    {
        anim.Play("Die");
        DieBroadcast?.Invoke();
        if (PhotonManager.IsMultiplayer)
        {
            character.characterPhoton.RestartCharacter();
        }
        else
        {
            RemoveCharacter();
        }
    }

    public void CharacterSetActive(bool val)
    {
        character.IsDead = !val;
        enabled = val;
        activationCollider.enabled = val;
        enabled = val;
        if(!val)
            StopAllCoroutines();
    }

    public void RemoveCharacter()
    {
        if (this == null) return;
        CharacterSetActive(false);

        if(IsLocalPlayer)
            Controller.Instance.OnPlayerDead(character);
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == Layers.Environment)
        {
            onGround = true;
        }
        if (other.gameObject.layer != 12 && other.gameObject.layer != 13)
        {
            if (!smoke.isPlaying)
            {
                smoke.Play();
            }
            attack = false;
            if(anim)
                anim.SetBool("attackStay", false);
        }
    }

    //public void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.layer == 11)
    //    {
    //        var enemy = other.transform.root.gameObject.GetComponent<Enemy>();
    //        if (enemy == null) return;
    //        if (!enemy.dead && enemy.isAttacking)
    //        {
    //            Hit(enemy);
    //        }
    //    }
    //    else if(other.gameObject.layer == Layers.Environment || other.gameObject.layer == Layers.Destructible)
    //    {
    //        onGround = true;
    //        smoke2.Play();
    //    }
    //}

    public void Hit(IDestructible attacker)
    {
        Debug.Log("Not Implemented.");
    }

    public virtual void Hit(Enemy enemy = null, int hp = 1, bool heavyAttack = false)
    {
        if (character.Health <= 0 || Invincible || (isAttacking && !heavyAttack)) return;
        //hp = Mathf.Clamp(hp, 1, character.Health);

        //if (IsLocalPlayer)
        //{
        //    {
        //        for (int i = 0; i < hp; i++)
        //        {
        //            characterHealth.RemoveHealth(stats.health - i - 1);
        //        }
        //    }
        //}

        character.Health = character.Health - hp;

        if (character.Health > 0)
        {
            anim.SetTrigger("hit");
            if(rb.velocity.y < 10)
                rb.AddForce(Vector3.up * 10, ForceMode.VelocityChange);
        }
        else
        {
            if (enemy != null)
            {
                enemy.target = null;
            }
            Die();
        }
        starsExplosion.transform.position = transform.position;
        starsExplosion.Play();
    }

    public virtual void SetAnimationHorizontal(Vector3 velo)
    {
        if (anim == null) return;
        velocity = velo;
        hspeed = new Vector2(velo.x, velo.z).magnitude;
        vspeed = velocity.y; //Mathf.Lerp(vspeed,velocity.y, 0.05f);
        anim.SetFloat("hSpeed", hspeed);
        anim.SetFloat("vSpeed", vspeed);
        anim.SetBool("onGround", onGround);
    }

    // Update is called once per frame
    protected virtual void FixedUpdate ()
    {
        Rotation();
        SetAnimationHorizontal(rb.velocity);
        Move();
        if (onGround)
            rb.AddForce(Vector3.up * addForce);
    }

    protected virtual void Update()
    {
        curPos = transform.position;
        Movement();
        Inputs();
        Jump();
        //if (attack)
        //{
            attack = false;
        //    AttackCollision();
        //}
    }


    void WeaponAttack()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            Attack();
        }
#endif

    }

    public void Jump()
    {
        if (jumpInput > 0 && onGround && timeLastJump < 0.1f)
        {
            timeLastJump = 1;
            rb.AddForce(Vector3.up * stats.jumpForce, ForceMode.VelocityChange);
            jumpInput = 0; 
            onGround = false;
        }
        else
        {
            timeLastJump -= Time.deltaTime;
        }
    }
    //public float force = 10;
    public bool onIce;
    public float iceForce = 30;
    public virtual void Move()
    {
        var velo = rb.velocity;
        float y = velo.y;
        velo.y = 0;
        float mag = velo.magnitude;
        //rb.rotation = Quaternion.Lerp(rb.rotation, transform.rotation, Time.deltaTime);
        if (velo.magnitude < stats.runSpeed)
        {
            if(onIce)
            {
                rb.AddForce(rb.rotation.Vector() * forwardPower * iceForce, ForceMode.Acceleration);
            }
            else
            {
                velo = rb.rotation.Vector() * (mag + forwardPower);
                velo.y = y;
                rb.velocity = velo;
            }
            //rb.AddForce(rb.rotation.Vector() * forwardPower * force, ForceMode.Acceleration);

        }
    }


    public virtual void Attack()
    {
        //Debug.Log("Attack");
        if (character.IsDead || attack) return;

            if (Thrown != null)
            {
                RaycastHit hit;
                if (Physics.SphereCast(transform.position, 5, transform.forward, out hit, 50, enemyLayer.value, QueryTriggerInteraction.Ignore))
                {
                    //Debug.Log(hit.transform.name);
                    transform.rotation = Engine.Math.RotateTowards(transform.position, hit.point);
                }
                anim.Play("Throw");
                Thrown(this, transform.forward);
            }
            else
            {
            //if(!onGround && rb.velocity.y > -2)
            //{
            //    rb.AddForce(Vector3.down * airForce, ForceMode.);
            //}
            //else
            //{
            //Debug.Log("Ground Attack");

            AttackBroadcast?.Invoke();
            if(MeleeAttack!=null)
            {
                MeleeAttack.Invoke();
            }
            //else
            //{
            //    PerformMeleeAttack();
            //}
            //}

        }
        //}
    }

    public void PerformMeleeAttack()
    {
        isAttacking = true;
        attack = true;
        anim.Play("Attack");
        //attackParticles.Play();
    }

    public void ResetAttackRadius()
    {
        meleeAttackRadius = startAttackRadius;
    }

    public LayerMask collisionLayer;
    HashSet<IDestructible> scripts = new HashSet<IDestructible>();


    public virtual void StateAnimatorInitialized()
    {
        throwAnimationHash = Animator.StringToHash("Throw");
        attackAnimationHash = Animator.StringToHash("Attack");
        AnimatorBehaviour.StateExit += (animatorStateInfo) =>
        {
            if (animatorStateInfo.shortNameHash == throwAnimationHash)
            {
                //CanMove = true;
            }
            else if (animatorStateInfo.shortNameHash == attackAnimationHash)
            {
                isAttacking = false;
                StopAttack?.Invoke();
            }
        };
    }

    //protected void AttackCollision()
    //{
    //    Ray ray = new Ray(curPos, Vector3.down);
    //    RaycastHit[] hits = Physics.SphereCastAll(curPos, meleeAttackRadius, Vector3.down, 10, collisionLayer.value,QueryTriggerInteraction.Ignore);
    //    for (int i = 0; i < hits.Length; i++)
    //    {
    //        //Debug.Log("Hit: " + hits[i].transform.name);
    //        //Debug.Log(hits[i].collider.GetType());
    //        scripts.Add(hits[i].collider.GetComponentInParent<IDestructible>());

    //    }
    //    foreach (var script in scripts)
    //    {
    //        if (script != null)
    //        {
    //            script.Hit(character);
    //            //script.Rigidbody.velocity = Vector3.zero;
    //           // script.Rigidbody.AddForce((Vector.Direction(transform.position, script.Transform.position) + Vector3.up * 2) * character.stats.attackForce, ForceMode.VelocityChange);
    //            StaticParticles.PlayHitParticles(script.Transform.position + Vector3.up);
    //            smokeExplosion.transform.position = script.Transform.position;
    //            smokeExplosion.Play();
    //            //attack = false;
    //        }
    //    }
    //    scripts.Clear();
    //}

    public void MovementEnable(bool enable)
    {
        movementEnabled = enable;
    }

    public void SetAnimation(string animationName)
    {
        anim.Play(animationName);
    }

    public void AnimationSetTrigger(string triggerName)
    {
        anim.SetTrigger(triggerName);
    }

    public void Hit(Character character)
    {
        //throw new NotImplementedException();
    }

    public void CallShake()
    {
        if(!character.IsLocalPlayer)
            Controller.Instance.gameCamera.Shake(0.15f, 2, 0.05f);
    }
}
