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

    void OnEnable()
    {
        Apply();
        character.AddItem(this);
    }

    void Awake()
    {
        if(triggerBroadcast)
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


    void OnTriggerWithEnemeny(Collider col)
    {
        if (!canInflictDamage || !character || character.IsDead || !character.movement.isAttacking) return;

        var destructible = col.GetComponent<IDestructible>();
        if(destructible!=null)
        {
            destructible.Hit(character.movement);
            StaticParticles.PlayHitParticles(col.transform.position + Vector3.up);
            smokeExplosion.transform.position = col.transform.position;
            smokeExplosion.Play();
            canInflictDamage = false;
        }
    }

    public virtual void Attack()
    {
        StartCoroutine(AttackCoroutine());
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
        RaycastHit[] hits = Physics.SphereCastAll(pos, 5, Vector3.down, 10, character.movement.collisionLayer.value,QueryTriggerInteraction.Ignore);
        float angle = Mathf.Infinity;
        float angle2;
        Transform enemy = null;
        foreach (var hit in hits)
        {
            if (hit.transform == character.transform) continue;
            angle2 = Vector3.Angle(character.transform.forward, hit.transform.position);
            if(angle2 < angle)
            {
                angle = angle2;
                var enemyComponent = hit.transform.GetComponent<IDestructible>();
                if (enemyComponent != null)
                    enemy = hit.transform;
            }
        }
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
        while(time <halfAttack)
        {
            if(enemy!=null && angle < 135)
            {
                character.transform.rotation = Engine.Math.RotateTowardsTopDown(character.transform, enemy.position, character.stats.turningSpeed * Time.deltaTime);
            }
            time += Time.deltaTime;
            yield return null;
        }

        canInflictDamage = false;
        if (character && !character.IsDead)
        {
            character.movement.MovementEnabled = true;
        }
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

