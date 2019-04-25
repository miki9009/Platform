using UnityEngine;


public class Mist : MonoBehaviour
{
    public ParticleSystem mistParts;
    public Transform anchor;
    public float forwardFactor = 5;
    Transform target;

    private void Awake()
    {
        Character.CharacterCreated += SetTarget;
        GameManager.GameReady += ResetSystem;
    }

    void ResetSystem()
    {
        mistParts.Play();
    }

    private void OnDestroy()
    {
        Character.CharacterCreated -= SetTarget;
        GameManager.GameReady -= ResetSystem;
    }

    void SetTarget(Character character)
    {
        if(character.IsLocalPlayer)
        {
            target = character.transform;
        }
    }

    private void Update()
    {
        if (!target) return;

        anchor.position = target.position + target.forward * forwardFactor;

    }
}