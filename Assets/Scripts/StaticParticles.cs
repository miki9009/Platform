using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticParticles : MonoBehaviour
{
    public ParticleSystem crateExploded;
    public ParticleSystem smokeExplosion;
    public ParticleSystem starsExplosion;
    public ParticleSystem hitParticles;
    public GameObject explosionPrefab;
    public float explosionPullTime = 5;

    static Stack<ParticleSystem> explosions;

    public static StaticParticles Instance
    {
        get;private set;
    }
    private void Awake()
    {
        Instance = this;
        CreateExplosions();
    }

    void CreateExplosions()
    {
        explosions = new Stack<ParticleSystem>();
        for (int i = 0; i < 10; i++)
        {
            var go = Instantiate(explosionPrefab);
            go.SetActive(false);
            explosions.Push(go.GetComponent<ParticleSystem>());
        }
    }

    public static void PlayHitParticles(Vector3 position)
    {
        if (Instance == null) return;
        Instance.hitParticles.transform.position = position;
        Instance.hitParticles.Play();
    }

    public static void CreateExplosion(Vector3 position)
    {
        if(explosions != null && explosions.Count > 0)
        {
            var explosion = explosions.Pop();
            explosion.gameObject.SetActive(true);
            explosion.transform.position = position;
            explosion.Play();
            Instance.StartCoroutine(ExplosionPull(explosion));
        }
    }

    static IEnumerator ExplosionPull(ParticleSystem explosion)
    {
        yield return new WaitForSeconds(Instance.explosionPullTime);
        explosion.gameObject.SetActive(false);
        explosions.Push(explosion);
        explosion.Stop();
    }

}
