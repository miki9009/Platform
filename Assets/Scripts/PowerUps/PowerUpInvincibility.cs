using UnityEngine;

public class PowerUpInvincibility : PowerUp
{
    public GameObject prefab;
    [HideInInspector] public Transform shield;
    public CollectionObject CollectionObject { get; set; }

    public void BackToCollection()
    {

    }

    protected override void Start()
    {
        base.Start();
        col = GetComponent<SphereCollider>();
        GetComponent<CollectionObject>().OnCollected += (GameObject obj) =>
        {
            col.enabled = false;
            character = obj.GetComponentInParent<Character>();
            ApplyPowerUp();
        };
    }

    protected override void ApplyPowerUp()
    {
        base.ApplyPowerUp();
        shield = Instantiate(prefab, character.transform).transform;
        character.movement.Invincible = true;
    }

    public override void Disable()
    {

        base.Disable();
        if (shield != null)
        {
            Destroy(shield.gameObject);
        }
        character.movement.Invincible = false;
    }

    public void Remove()
    {
        Disable();
    }
}