using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour, IRightArmItem
{
    public float attackTime = 1.5f;
    public GameObject collectionPrefab;
    public ParticleSystem attackParticles;
    [HideInInspector] public Character character;
    public float attackRadius = 3;
    public TriggerBroadcastStay triggerBroadcast;

    bool canInflictDamage;

    ParticleSystem smokeExplosion;

    public CollectionObject CollectionObject { get; set; }

    protected virtual void OnEnable()
    {
        Apply();
        character.AddItem(this);
    }

    protected virtual void Awake()
    {
        if (triggerBroadcast)
        {
            triggerBroadcast.TriggerStay += x =>
            {
                OnTriggerWithEnemeny(x);
            };
        }
    }

    void Start()
    {
        smokeExplosion = StaticParticles.Instance.smokeExplosion;

    }

    bool isInRange;
    void OnTriggerWithEnemeny(Collider col)
    {
        if (!canInflictDamage || !character || character.IsDead || !character.movement.isAttacking) return;
        isInRange = true;
    }

    void InflictDamage(Transform enemy)
    {
        if (!enemy || !canInflictDamage || !character || character.IsDead || !character.movement.isAttacking) return;
        //Debug.Log("IsInRange: " + isInRange);
        var destructible = enemy.GetComponent<IDestructible>();
        if (destructible != null)
        {
            destructible.Hit(character.movement);
            StaticParticles.PlayHitParticles(enemy.position + Vector3.up);
            smokeExplosion.transform.position = enemy.position;
            smokeExplosion.Play();
            canInflictDamage = false;
        }
    }

    IEnumerator attackCoroutine;

    public virtual void Attack()
    {
        if(attackCoroutine==null)
        {
            attackCoroutine = AttackCoroutine();
            StartCoroutine(attackCoroutine);
        }

    }

    public virtual void StopAttack()
    {
        canInflictDamage = false;
    }

    IEnumerator AttackCoroutine()
    {
        if (!character)
            yield break;
        var pos = character.transform.position;
        Ray ray = new Ray(pos, Vector3.down);
        RaycastHit[] hits = Physics.SphereCastAll(pos, attackRadius * 3, Vector3.down, 10, character.movement.collisionLayer.value,QueryTriggerInteraction.Ignore);
        float dot = -100;
        float dot2;
        Transform enemy = null;
        foreach (var hit in hits)
        {
            //Debug.Log(hit.transform.name);
            if (hit.transform == character.transform) continue;
            //angle2 = Vector3.Angle(character.transform.position, hit.point);
            dot2 = Vector3.Dot(character.transform.forward, Engine.Vector.Direction(character.transform.position, hit.transform.position)) * 100;
            if (dot2 > 0)
            {
                dot = dot2;
                var enemyComponent = hit.transform.GetComponent<IDestructible>();
                if (enemyComponent != null)
                {
                    if (enemyComponent.Destroyed) continue;

                    if(enemy)
                    {
                        if (Vector3.Distance(hit.transform.position, character.transform.position) < Vector3.Distance(enemy.position, character.transform.position))
                            enemy = hit.transform;
                    }
                    else
                    {
                        enemy = hit.transform;
                    }
                }
                else
                {
                    enemyComponent = hit.transform.GetComponentInParent<IDestructible>();
                    if (enemyComponent != null)
                    {
                        if (enemyComponent.Destroyed) continue;
                        if (enemy)
                        {
                            if (Vector3.Distance(hit.transform.position, character.transform.position) < Vector3.Distance(enemy.position, character.transform.position))
                                enemy = enemyComponent.Transform;
                        }
                        else
                        {
                            enemy = enemyComponent.Transform;
                        }
                    }
                }
            }
        }
        if (!enemy)
            Debug.Log("No enemy");

        float halfAttack = attackTime/2;

        character.movement.MovementEnabled = false;
        var velo = character.rb.velocity;
        velo.x = 0;
        velo.z = 0;
        character.rb.velocity = velo;

        if (attackParticles)
            attackParticles.Play();

        character.movement.PerformMeleeAttack();
        yield return new WaitForSeconds(halfAttack);
        canInflictDamage = true;
        float time = 0;
        while(time < halfAttack)
        {
            if(enemy!=null && dot > 0)
            {
                character.transform.rotation = Engine.Math.RotateTowardsTopDown(character.transform, enemy.position, character.stats.turningSpeed * Time.deltaTime);

                if (time > (halfAttack / 2) && canInflictDamage)
                {
                    if (Vector3.Distance(character.transform.position, enemy.position) < attackRadius || isInRange)
                        InflictDamage(enemy);
                    canInflictDamage = false;
                }
            }
            time += Time.deltaTime;
            yield return null;
        }

        isInRange = false;
        if (character && !character.IsDead)
        {
            character.movement.MovementEnabled = true;
        }
        attackCoroutine = null;
    }

    public virtual void Remove()
    {
        Engine.PoolingObject.Recycle(gameObject.GetName(), gameObject, () =>
        {
            if (character != null)
            {
                character.movement.MeleeAttack = null;
                character.movement.StopAttack = null;
                character.movement.ResetAttackRadius();
            }

         });
        CollectionObject.BackToCollection(true);
    }

    public virtual void Clear()
    {
        Remove();
    }

    public virtual void Apply()
    {
        character = GetComponentInParent<Character>();
        character.movement.MeleeAttack = Attack;
        character.movement.StopAttack = StopAttack;
        character.movement.meleeAttackRadius = attackRadius;
    }

    public void BackToCollection()
    {
        Engine.Log.Print("Not implemented.");
    }
}

