using UnityEngine;

public class Fireball : MonoBehaviour, IPoolObject
{
    public float force = 10;
    public Rigidbody rb;
    public float poolTime;
    public Transform caster;
    ParticleSystem parts;

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

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.transform == caster) return;
        Console.WriteLine("Projectile hit: " + collision.transform.name, Console.LogColor.Lime);
        StaticParticles.CreateExplosion(transform.position);
        Pool();
    }

    public void Shoot(Vector3 startPos, Vector3 dir, Transform caster)
    {
        dir.y = 0;
        this.caster = caster;
        ParticleSystem.MainModule psmain = parts.main;
        psmain.prewarm = true;
        transform.position = startPos;
        rb.velocity = dir * force;
        Invoke("Pool", poolTime);
    }

    void Pool()
    {
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