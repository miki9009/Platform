using System.Collections;
using UnityEngine;

public class WaterCollisionSplash : MonoBehaviour
{
    public GameObject waterSplashPrefab;

    ParticleSystem waterSplash;
    bool enabledCollision = false;

    private void Awake()
    {
        waterSplash = Instantiate(waterSplashPrefab).GetComponent<ParticleSystem>();
    }

    private void Start()
    {
        enabledCollision = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!enabledCollision) return;
        waterSplash.transform.position = other.transform.position;
        waterSplash.Play();
        if (other.gameObject.layer == Layers.Character)
        {
            var character = other.gameObject.GetComponent<Character>();
            if (character == null)
            {
                Debug.LogError("No character on object");
                return;
            }
            StartCoroutine(KillPlayer(character));
        }
        else
        {
            other.gameObject.SetActive(false);
        }
    }

    IEnumerator KillPlayer(Character character)
    {
        Controller.Instance.gameCamera.SetTarget(null);
        yield return new WaitForSeconds(1);
        character.gameObject.SetActive(false);
        yield return new WaitForSeconds(2);
        if(character != null && character.movement != null)
            character.movement.DieNonAnimation();
    }
}
