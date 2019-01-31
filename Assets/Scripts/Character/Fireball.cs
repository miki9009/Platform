using UnityEngine;

public class Fireball : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        StaticParticles.CreateExplosion(collision.contacts[0].point);
        gameObject.SetActive(false);
    }
}