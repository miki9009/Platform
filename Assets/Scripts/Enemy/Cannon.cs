using Engine;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : LevelElement, IShooter
{
    public Collider col;
    public Transform cannon;
    public Transform barrel;
    protected bool triggered;
    public float rotationSpeed = 1;
    protected List<CharacterMovement> characters;
    protected Transform target;
    public GameObject projectile;
    public float timeBeforeShooting = 2;
    public float force = 10;
    protected float curTimeBeforeShooting = 0;
    public float forceDivider = 5;
    Stack<Projectile> projectiles;
    public ParticleSystem parts;

    public ParticleSystem Trails
    {
        get
        {
            return parts;
        }
    }

    protected void Awake()
    {
        characters = new List<CharacterMovement>();
        projectiles = new Stack<Projectile>();
        for (int i = 0; i < 10; i++)
        {
            var p = Instantiate(projectile);
            projectiles.Push(p.GetComponent<Projectile>());
        }
    }

    protected void OnTriggerEnter(Collider other)
    {
        triggered = true;
        var character = other.GetComponentInParent<CharacterMovement>();
        if(character)
        {
            characters.Add(character);
        }
    }

    protected void OnTriggerExit(Collider other)
    {
        var character = other.GetComponentInParent<CharacterMovement>();
        if (character)
        {
            if(characters.Contains(character))
                characters.Remove(character);
        }

        if(characters.Count == 0)
        {
            triggered = false;
            target = null;
        }

    }

    protected virtual void Update()
    {
        if(triggered && characters.Count > 0)
        {
            if(target == null)
            {
                float dis = Mathf.Infinity;
               
                for (int i = 0; i < characters.Count; i++)
                {
                    float dis2 = Vector3.Distance(transform.position, characters[i].transform.position);
                    if(dis2 < dis)
                    {
                        target = characters[i].transform;
                        dis = dis2;
                    }
                }
            }
            else
            {
                transform.rotation = Math.RotateTowardsTopDown(cannon, target.position + Vector3.up, rotationSpeed * Time.deltaTime);
                if(curTimeBeforeShooting >= timeBeforeShooting)
                {
                    Shoot(target.position);
                    curTimeBeforeShooting = 0;
                }
                else
                {
                    curTimeBeforeShooting += Time.deltaTime;
                }
            }

        }
    }

    public void ResetProjectile(Projectile projectile)
    {
        projectiles.Push(projectile);
        projectile.rb.velocity = Vector3.zero;
        projectile.gameObject.SetActive(false);
    }

    public void Shoot(Vector3 position, bool useGravity = true)
    {
        Projectile p = null;
        if(projectiles.Count > 0)
        {
            p = projectiles.Pop();
        }
        else
        {
            p = Instantiate(projectile, transform).GetComponent<Projectile>();
        }
        if(p!= null)
        {
            p.transform.position = barrel.position;
            p.Shoot(this,barrel.forward, force * forceDivider * Vector3.Distance(transform.position, position), useGravity);
        }
    }
}