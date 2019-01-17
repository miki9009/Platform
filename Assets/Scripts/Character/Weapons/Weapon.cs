using UnityEngine;

public class Weapon : MonoBehaviour, IRightArmItem
{

    public GameObject collectionPrefab;
    public ParticleSystem attackParticles;
    [HideInInspector] public Character character;
    public CollectionObject collectionObject;

    public CollectionObject CollectionObject { get; set; }

    private void Start()
    {
        character.AddItem(this);
    }

    void Attack()
    {
        attackParticles.Play();
    }


    public void Remove()
    {
        Engine.PoolingObject.Recycle(gameObject.GetName(), gameObject, () =>
        {
            if (character != null)
            character.movement.MeleeAttack -= Attack;
         });
        Debug.Log("Remove");
        collectionObject.BackToCollection(true);
    }

    public void Clear()
    {
        Remove();
    }

    public void Apply()
    {
        character = transform.root.GetComponent<Character>();
        character.movement.MeleeAttack += Attack;
    }

    public void BackToCollection()
    {
        throw new System.NotImplementedException();
    }
}

