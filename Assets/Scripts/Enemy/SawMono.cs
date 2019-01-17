using UnityEngine;
using Engine;

public class SawMono : MonoBehaviour
{
    public float sawSpeed = 5;
    public Transform saw;
    Vector3 localEuler;
    public TriggerBroadcast broadcast;

    private void Awake()
    {
        localEuler = saw.localEulerAngles;
        broadcast.TriggerEntered += Hit;
    }

    private void Update()
    {
        saw.localEulerAngles = new Vector3(localEuler.x, saw.localEulerAngles.y + sawSpeed, localEuler.z);
    }

    public void Hit(Collider col)
    {
        var mov = col.GetComponent<CharacterMovement>();
        if (mov != null)
        {
            if (!mov.character.IsDead)
                mov.Die();
        }

    }
}