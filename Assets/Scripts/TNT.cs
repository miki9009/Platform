using Engine;
using System.Collections;
using UnityEngine;

public class TNT:MonoBehaviour
{
    public bool triggered = false;
    public float timeToExplosion = 4;
    public ParticleSystem parts;
    public float force = 10;
    public float timeForce = 1;
    public float explosionForce = 100;
    float blendSwitch = 1;
    bool expand;

    Rigidbody rb;
    SkinnedMeshRenderer mesh;
    float timeout = 0;
    public bool canTrigger = true;
    SphereCollider sphere;
    


    private void Awake()
    {
        parts.transform.parent = null;
        parts.gameObject.SetActive(false);
        rb = GetComponent<Rigidbody>();
        mesh = GetComponent<SkinnedMeshRenderer>();
        sphere = GetComponent<SphereCollider>();
        blendSwitch = Random.Range(-1f, 1f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == Layers.Character)
        {
            if (canTrigger)
            triggered = true;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody != null && canTrigger == false)
        {
            Debug.Log(other.name);
            other.attachedRigidbody.AddForceAtPosition((Vector3.up + Vector.Direction(transform.position, other.transform.position)) * explosionForce, other.transform.position, ForceMode.VelocityChange);
            if (other.gameObject.layer == Layers.Character)
            {
                var movement = other.gameObject.GetComponent<CharacterMovement>();
                if (movement != null)
                {
                    movement.Hit(null,5, true);
                }
                else
                {
                    Debug.LogError("No movement attached to current object");
                }
            }
        }
    }


    void EnableParticles()
    {
        sphere.enabled = true;
        parts.transform.position = transform.position + Vector3.up;
        parts.gameObject.SetActive(true);
        parts.Play();
        canTrigger = false;
        Invoke("DestroyMe", 0.1f);
        triggered = false;
    }


    void DestroyMe()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (triggered)
        {
            if (!canTrigger)
            {
                triggered = false;
                return;
            }

            if (timeout < timeForce)
            {
                timeout += Time.deltaTime;
            }
            else
            {
                var pos = transform.position + Vector3.up + Vector3.right * Random.Range(-2f, 2f) + Vector3.forward * Random.Range(-2f, 2f);
                rb.AddForceAtPosition((Vector.Direction(transform.position, pos)) * force, pos, ForceMode.VelocityChange);
                timeout = 0;
            }

            if (expand)
            {
                if (blendSwitch > 0)
                {
                    blendSwitch -= Time.deltaTime;
                    float lastShape1 = mesh.GetBlendShapeWeight(0);
                    mesh.SetBlendShapeWeight(0, Mathf.Clamp(lastShape1 + Random.Range(-30,30),0,100));
                }
                else
                {
                    blendSwitch = Random.Range(0.1f, 1);
                    expand = false;
                }
            }
            else
            {
                if (blendSwitch > 0)
                {
                    blendSwitch -= Time.deltaTime;
                    float lastShape2 = mesh.GetBlendShapeWeight(1);
                    mesh.SetBlendShapeWeight(1, Mathf.Clamp(lastShape2 + Random.Range(-30, 30), 0, 100));
                }
                else
                {
                    blendSwitch = Random.Range(0.1f, 1);
                    expand = true;
                }
            }



            if (timeToExplosion > 0)
            {
                timeToExplosion -= Time.deltaTime;
            }
            else
            {
                canTrigger = false;
                EnableParticles();
            }
        }
    }

}