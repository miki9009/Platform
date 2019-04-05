using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Engine;
using Engine.UI;

public abstract class CharacterMovement : MonoBehaviour, IThrowable, IStateAnimator, IDestructible
{
    bool _onGround;
    public bool OnGround
    {
        get
        {
            return _onGround;
        }
        set
        {
            _onGround = value;
            if(anim)
                anim.SetBool("onGround", value);
        }
    }
    public ParticleSystem smoke2;
    public Transform model;
    public LayerMask enemyLayer;
    public SphereCollider activationCollider;
    public ParticleSystem attackParticles;
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
    public bool shieldUp;
    public bool isRolling;
    public float timeBeforeAnotherRoll = 0;
    float startAttackRadius;
    protected Vector3 forward;
    public Shield Shield
    {
        get;set;
    }

    public Transform powerUpAnchor;
    public bool Initialized
    {
        get; private set;
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
    int hitAnimationHash;
    int rollAnimationHash;
    int shieldAnimationHash;
    float disY;
    float timeLastJump = 0;
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

    public bool MovementEnabled { get; set; } = true;

    public Rigidbody Rigidbody { get { return rb; } }

    private void Awake()
    {
        character = GetComponent<Character>();
        jumpCollider = GetComponent<BoxCollider>();
        smoke = GetComponentInChildren<ParticleSystem>();
        StartPosition = transform.position;
        disY = Screen.height / 8;
        OnGround = true;
    }


    // Use this for initialization
    void Start()
    {
        curPos = transform.position;
        stats = character.stats;
        rb = character.rb;
        anim = character.anim;
        anim.SetBool("onGround", OnGround);
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
        OnGround = false;
        smoke.Stop();
    }

    public virtual void Die()
    {
        Debug.Log("Died");
        anim.Play("Die");
        MovementEnabled = false;
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
        if (!val)
            StopAllCoroutines();
    }

    public void RemoveCharacter()
    {
        if (this == null) return;
        CharacterSetActive(false);

        if (IsLocalPlayer)
            Controller.Instance.OnPlayerDead(character);
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == Layers.Environment)
        {
            OnGround = true;
        }
        if (other.gameObject.layer != 12 && other.gameObject.layer != 13)
        {
            if (!smoke.isPlaying)
            {
                smoke.Play();
            }
            attack = false;
            if (anim)
                anim.SetBool("attackStay", false);
        }
    }

    public void Hit(IDestructible attacker)
    {
        Character character = null;
        if(attacker as CharacterMovement)
        {
            character = (attacker as CharacterMovement).character;
            Hit(character);
        }
        else
        {
            Debug.Log("error");
        }

    }

    public virtual void Hit(Enemy enemy = null, int hp = 1, bool heavyAttack = false)
    {
        if(shieldUp)
        {
            Debug.Log("Blocked by shield");
        }
        if (character.Health <= 0 || Invincible || (shieldUp && !heavyAttack))
        {
            return;
        }

        if (shieldUp)
        {
            if (enemy != null)
            {
                rb.AddForce(enemy.transform.forward * 2, ForceMode.VelocityChange);
                return;
            }
        }

        character.Health = character.Health - hp;

        if (character.Health > 0)
        {
            anim.SetTrigger("hit");
            MovementEnabled = false;
            rb.AddForce(transform.forward * -2, ForceMode.VelocityChange);
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
        vspeed = velocity.y;
        anim.SetFloat("hSpeed", hspeed);
        anim.SetFloat("vSpeed", vspeed);
    }

    // Update is called once per frame
    protected virtual void FixedUpdate()
    {
        if (!MovementEnabled)
        {
            if(shieldUp)
                ShieldMovement();
            return;
        }
        Rotation();
        SetAnimationHorizontal(rb.velocity);
        Move();
        if (OnGround)
            rb.AddForce(Vector3.up * addForce);
        if (jumpInput > 0)
            Jump();
    }



    protected virtual void Update()
    {
        if(timeBeforeAnotherRoll > 0)
            timeBeforeAnotherRoll -= Time.deltaTime;
        if (!MovementEnabled) return;

        forward = transform.forward;
        curPos = transform.position;
        Movement();
        Inputs();
        //Jump();
        attack = false;
    }

    //void LateUpdate()
    //{
    //    if (jumpInput > 0)
    //        Jump();
    //}

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
        if (jumpInput > 0 && OnGround)
        {
            jumpInput = 0;
            rb.AddForce(Vector3.up * stats.jumpForce, ForceMode.VelocityChange);
            //anim.Play("JumpUp");
            OnGround = false;
        }
    }

    public bool onIce;
    public float iceForce = 30;
    public virtual void Move()
    {
        var velo = rb.velocity;
        float y = velo.y;
        velo.y = 0;
        float mag = velo.magnitude;
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
        }
    }


    public virtual void Attack()
    {
        if (character.IsDead || attack) return;

            if (Thrown != null)
            {
                RaycastHit hit;
                if (Physics.SphereCast(transform.position, 5, transform.forward, out hit, 50, enemyLayer.value, QueryTriggerInteraction.Ignore))
                {
                    transform.rotation = Engine.Math.RotateTowards(transform.position, hit.point);
                }
                anim.Play("Throw");
                Thrown(this, transform.forward);
            }
            else
            {
            AttackBroadcast?.Invoke();
            if(MeleeAttack!=null)
            {
                MeleeAttack.Invoke();
            }
        }
    }

    public void PerformMeleeAttack()
    {
        isAttacking = true;
        attack = true;
        anim.Play("Attack");
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
        hitAnimationHash = Animator.StringToHash("hit");
        rollAnimationHash = Animator.StringToHash("Roll");
        shieldAnimationHash = Animator.StringToHash("Shield");

        AnimatorBehaviour.StateEnter += (animatorStateInfo)=>
        {
            //if(animatorStateInfo.shortNameHash == attackAnimationHash)
            //{
            //    var velo = rb.velocity;
            //    hspeed = new Vector2(velo.x, velo.z).magnitude;
            //    anim.SetFloat("hSpeed", hspeed);
            //}
        };

        AnimatorBehaviour.StateExit += (animatorStateInfo) =>
        {
            //anim.ResetTrigger("ShieldDown");
            if (animatorStateInfo.shortNameHash == throwAnimationHash) //THROW
            {
                //CanMove = true;
            }
            else if (animatorStateInfo.shortNameHash == attackAnimationHash) //ATTACK
            {
               // ResetVelocity();
                isAttacking = false;
                StopAttack?.Invoke();
            }
            else if (animatorStateInfo.shortNameHash == hitAnimationHash) //GETS HIT
            {
                MovementEnabled = true;
            }
            else if (animatorStateInfo.shortNameHash == rollAnimationHash) //ROLLING
            {
                isRolling = false;
                ResetVelocity();
                MovementEnabled = true;
                //anim.SetTrigger("ShieldUp");
                anim.SetTrigger("ShieldDown");
                timeBeforeAnotherRoll = 0.25f;
            }
            if(animatorStateInfo.shortNameHash == shieldAnimationHash) //SHIELD
            {
                shieldUp = false;
            }
        };
    }

    public void ResetVelocity()
    {
        var velo = rb.velocity;
        velo.x = 0;
        velo.z = 0;
        rb.velocity = velo;
        anim.SetFloat("hSpeed", 0);
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
        Hit(null, 2);
    }

    public void CallShake()
    {
        if(!character.IsLocalPlayer)
            Controller.Instance.gameCamera.Shake(0.15f, 2, 0.05f);
    }

    protected virtual void ShieldMovement()
    {
        if(shieldUp)
        {
            Console.WriteLine("Not implemented");
        }
    }

    public virtual void Roll(bool roll)
    {
        if (!isRolling)
        {
            if (roll)
            {
                anim.SetTrigger("roll");
                isRolling = true;
            }
        }
        else
        {
            forwardPower = 1;
            Move();
        }

        if (roll)
        {
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.LookRotation(-forward), character.stats.turningSpeed);
        }
    }

    //void OnGUI()
    //{
    //    Draw.TextColor(10, 300, 255, 255, 255, 1, "Forward factor: " + forwardPower);
    //}
}
