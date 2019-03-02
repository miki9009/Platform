using UnityEngine;

public class WaterSurface : MonoBehaviour
{
    public ParticleSystem ripples;
    private void OnCollisionEnter(Collision collision)
    {
        var pos = collision.contacts[0].point;
        ripples.transform.position = new Vector3(pos.x, ripples.transform.position.y, pos.z);
        ripples.Emit(1);
    }
}