using UnityEngine;

public class PowerUpThrowable : PowerUp
{
    public GameObject prefab;
    [HideInInspector] public ThrowableObject throwableObject;
    CollectionObject collectionObject;

    protected override void Start()
    {
        base.Start();
        col = GetComponent<SphereCollider>();
        collectionObject = GetComponent<CollectionObject>();
        collectionObject.OnCollected += (GameObject obj) =>
        {
            col.enabled = false;
            character = obj.GetComponentInParent<Character>();
            ApplyPowerUp();
        };
    }

    protected override void ApplyPowerUp()
    {
        var throwable = (IThrowable)character.movement;
        base.ApplyPowerUp();
        var obj = Instantiate(prefab, character.rightLowerArm);
        throwableObject = obj.GetComponent<ThrowableObject>();
        throwableObject.transform.localPosition = new Vector3(-0.3f, 0.1934f, -0.0378f);
        throwableObject.CollectionObject = collectionObject;
        throwableObject.Initialize(character);

        Disable();
    }

    public override void Disable()
    {
        base.Disable();
    }

}
/// <summary>
/// Defines objects that can throw ThrowableObject
/// </summary>
public interface IThrowable
{
    ThrowableObject ThrowObject { get; set; }
    event System.Action<IThrowable, Vector3> Thrown;
}