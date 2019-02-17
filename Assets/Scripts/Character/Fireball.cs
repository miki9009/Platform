using Engine;
using UnityEngine;

public class Fireball : MonoBehaviour, IPoolObject
{
    public LayerMask collisionLayer;
    public float force = 10;
    public Rigidbody rb;
    public float poolTime;
    public Transform caster;
    [HideInInspector]
    public Character character;
    [HideInInspector]
    public Enemy enemy;
    ParticleSystem parts;
    public enum CasterType { Character, Enemy}
    public CasterType casterType = CasterType.Character;
    public float autoAimSpeed = 2;
    public GameObject GameObject
    {
        get
        {
            return gameObject;
        }
    }

    void Awake()
    {
        parts = GetComponent<ParticleSystem>();
        var main = parts.main;
        main.loop = true;   // prewarm only works on looping systems

        Restart();
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.transform == caster) return;
        StaticParticles.CreateExplosion(transform.position);
        Pool();
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.transform == caster) return;
        //Console.WriteLine("Projectile hit: " + collision.transform.name, Console.LogColor.Lime);
        
        if(casterType == CasterType.Character && collision.gameObject.layer == Layers.Enemy)
        {
            var enemy= collision.GetComponent<IThrowableAffected>();
            if (enemy != null)
                enemy.OnHit(character);
            else
                Debug.LogError("Didn't find Enemy Component on: " + collision.name);
            StaticParticles.CreateExplosion(transform.position);
            Pool();
        }
        else
        {
            return;
        }

    }

    Transform target;
    public void Shoot(Vector3 startPos, Vector3 dir, Character character)
    {
        var colliders = Physics.SphereCastAll(startPos, 20, dir, 1, collisionLayer.value, QueryTriggerInteraction.Ignore);
        float angle = Mathf.Infinity;
        foreach (var collider in colliders)
        {
            float angle2 = Vector3.Angle(dir, Vector.Direction(startPos, collider.point));
            if(angle2 < angle)
            {
                angle = angle2;
                target = collider.transform;
                Debug.Log("Found: " + target.name);
            }
        }
        dir.y = 0;
        this.character = character;
        this.caster = character.transform;
        ParticleSystem.MainModule psmain = parts.main;
        psmain.prewarm = true;
        transform.position = startPos;
        rb.velocity = dir * force;
        Invoke("Pool", poolTime);
    }

    //void FixedUpdate()
    //{
    //    if (target != null)
    //    {
    //        rb.velocity = Vector3.Lerp(rb.velocity.normalized, Vector.Direction(transform.position, target.position + Vector3.up*1.5f), Time.deltaTime * autoAimSpeed) * force;
    //    }
    //}

    public void Shoot(Vector3 startPos, Vector3 dir, Enemy enemy)
    {
        dir.y = 0;
        this.caster = enemy.transform;
        this.enemy = enemy;
        ParticleSystem.MainModule psmain = parts.main;
        psmain.prewarm = true;
        transform.position = startPos;
        rb.velocity = dir * force;
        Invoke("Pool", poolTime);
    }

    void Pool()
    {
        target = null;
        CancelInvoke();
        SpawnManager.Recycle(this);
    }

    public void AdditionalRecycle()
    {
        rb.velocity = Vector3.zero;
    }

    void Restart()
    {
        parts.Stop();
        parts.Clear();

        var main = parts.main;
        main.prewarm = true;

        parts.Play();
    }
}