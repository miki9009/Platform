using UnityEngine;

public class ThrowableObject : MonoBehaviour, IRightArmItem
{
    private IThrowable throwable;
    private Character character;
    public float force;
    Rigidbody rb;
    Vector3 dir;
    public float upFactor;

    public CollectionObject CollectionObject
    {
        get;

        set;
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    public void Apply()
    {
        throwable = (IThrowable)character.movement;
        throwable.ThrowObject = this;
        throwable.Thrown += OnThrow;
    }

    public void BackToCollection()
    {
        CollectionObject.BackToCollection(true);
    }

    public void Initialize(Character character)
    {
        this.character = character;
        character.AddItem(this);
    }

    void OnThrow(IThrowable throwable, Vector3 direction)
    {
        if (character == null) return;
        dir = direction;
        Invoke("PerformThrow", 0.5f);
        throwable.Thrown -= OnThrow;
        character.rightArmItem = null;
    }


    void PerformThrow()
    {
        throwable.ThrowObject = null;
        transform.parent = null;
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.velocity =  dir * force;
    }

    public void Remove()
    {
        Debug.Log("Remove from Throwable Object");
        rb.useGravity = true;
        rb.isKinematic = false;
        transform.parent = null;
        gameObject.SetActive(false);
        throwable.Thrown -= OnThrow;
        BackToCollection();
    }

    private void OnDestroy()
    {
        if (throwable != null)
        {
            throwable.Thrown -= OnThrow;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == Layers.Enemy)
        {
            var affected = other.gameObject.GetComponentInParent<IThrowableAffected>();
            if (affected != null)
            {
                affected.OnHit();
            }
            else
            {
                Debug.Log("Affected not found");
            }
        }

    }

}

/// <summary>
/// Defines if object can be hit by Throwable object
/// </summary>
public interface IThrowableAffected
{
    void OnHit();
}
