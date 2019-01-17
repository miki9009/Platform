using Engine;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public bool triggered = false;
    public ParticleSystem parts;
    public float force = 10;
    public float explosionForce = 100;
    public GameObject model;
    public float deactivateAfterTimer = 3f;

    void EnableParticles()
    {
        parts.transform.position = transform.position + Vector3.up;
        parts.Play();
        model.SetActive(false);
        Invoke("DestroyMe", deactivateAfterTimer);
    }
    void DestroyMe()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        transform.rotation = CollectionObject.rotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (other.attachedRigidbody != null)
        {
            Debug.Log(other.name);
            other.attachedRigidbody.AddForceAtPosition((Vector3.up + Vector.Direction(transform.position, other.transform.position)) * explosionForce, other.transform.position, ForceMode.VelocityChange);
            triggered = true;
            EnableParticles();
            var movement = other.gameObject.GetComponentInParent<CharacterMovement>();
            if (movement != null)
            {
                movement.Hit(null, 5, true);
            }
            else
            {
                Debug.LogError("No movement attached to current object");
            }

        }
    }
}