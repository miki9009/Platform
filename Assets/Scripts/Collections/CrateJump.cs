using Engine;
using System.Collections;
using UnityEngine;

public class CrateJump : MonoBehaviour
{
    public GameObject collection;
    public int maxCollectionAmount;
    public int minCollectionAmount;
    public bool randomizeAmount;
    public float animationSpeed = 2;
    public float force = 5;
    SkinnedMeshRenderer skinnedMesh;
    float duration = 0f;
    GameCamera gameCam;
    ParticleSystem crateExplosion;
    bool isSquashing = false;
    BoxCollider boxCollider;
    SphereCollider sphereCollider;
    Character character;

    private void Start()
    {
        if (randomizeAmount)
        {
            maxCollectionAmount = Random.Range(minCollectionAmount, maxCollectionAmount);
        }
        skinnedMesh = GetComponent<SkinnedMeshRenderer>();
        gameCam = Controller.Instance.gameCamera.GetComponent<GameCamera>();
        boxCollider = GetComponent<BoxCollider>();
        sphereCollider = GetComponent<SphereCollider>();

        crateExplosion = StaticParticles.Instance.crateExploded;
        for (int i = 0; i < maxCollectionAmount; i++)
        {
            PoolingObject.AddSpawn(collection.name, Instantiate(collection));
        }

    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.transform.position.y > transform.position.y + 1f)
        {
            var character = collision.gameObject.GetComponent<Character>();
            if (character != null && !isSquashing && character.movement.enabled)
            {
                this.character = character;
                gameCam.upFactor = 0;
                isSquashing = true;
                duration = 0;
                character.rb.velocity = Vector3.up * force;
                character.transform.position = new Vector3(transform.position.x, character.transform.position.y, transform.position.z);
                character.movement.SetAnimation("JumpCrate");
                StartCoroutine(Animate());
            }

        }
    }

    private void OnDisable()
    {
        if (maxCollectionAmount <= 0)
        {
            gameCam.upFactor = gameCam.UpFactorAtStart;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var character = other.gameObject.GetComponent<Character>();
        if (character != null)
        {
            gameCam.upFactor = gameCam.UpFactorAtStart;
        }
    }

    IEnumerator Animate()
    {
        if (maxCollectionAmount <= 0)
        {
            crateExplosion.transform.position = transform.position;
            crateExplosion.Play();
            boxCollider.enabled = false;
            sphereCollider.enabled = false;
            gameObject.SetActive(false);
            yield break;
        }
        maxCollectionAmount--;
        var crate = PoolingObject.GetSpawn(collection.name, transform.position, Quaternion.identity);
        while (duration < 1)
        {
            crate.transform.position += Vector3.up * 0.2f;
            duration += Time.deltaTime * animationSpeed;
            skinnedMesh.SetBlendShapeWeight(0, duration * 100);
            gameCam.upFactor = 0;
            yield return null;
        }
        isSquashing = false;
        while (duration > 0)
        {
            duration -= Time.deltaTime * animationSpeed;
            skinnedMesh.SetBlendShapeWeight(0, duration * 100);
            gameCam.upFactor = 0;
            yield return null;
        }
    }
}