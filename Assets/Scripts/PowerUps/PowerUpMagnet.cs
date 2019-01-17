using UnityEngine;

public class PowerUpMagnet : PowerUp
{
    public GameObject prefab;
    [HideInInspector]public Magnet magnet;
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
        var obj = Instantiate(prefab, character.leftLowerArm);
        magnet = obj.GetComponent<Magnet>();
        magnet.Character = character;
        magnet.upperArm = character.leftUpperArm;
        magnet.lowerArm = character.leftLowerArm;
    }

    public override void Disable()
    {

        base.Disable();
        if (magnet != null)
        {
            Destroy(magnet.gameObject);
        }
    }

    public void Remove()
    {
        Disable();
    }
}