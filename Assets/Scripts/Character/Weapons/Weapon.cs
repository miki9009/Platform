using UnityEngine;

public class Weapon : MonoBehaviour, IRightArmItem
{

    public GameObject collectionPrefab;
    public ParticleSystem attackParticles;
    [HideInInspector] public Character character;
    public CollectionObject collectionObject;

    public CollectionObject CollectionObject { get; set; }

    void OnEnable()
    {
        Apply();
        character.AddItem(this);
    }

    public virtual void Attack()
    {
        if(attackParticles)
            attackParticles.Play();
    }


    public virtual void Remove()
    {
        Engine.PoolingObject.Recycle(gameObject.GetName(), gameObject, () =>
        {
            if (character != null)
            character.movement.MeleeAttack = null;
         });
        collectionObject.BackToCollection(true);
    }

    public virtual void Clear()
    {
        Remove();
    }

    public virtual void Apply()
    {
        character = GetComponentInParent<Character>();
        //character.movement.MeleeAttack = null;
    }

    public void BackToCollection()
    {
        throw new System.NotImplementedException();
    }
}

