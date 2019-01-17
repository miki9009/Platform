using UnityEngine;

public class TwoDimensionTrigger : MonoBehaviour
{
    CharacterMovement movement;
    private void Start()
    {
        movement = Character.GetLocalPlayer().movement;
    }

    private void OnTriggerStay(Collider other)
    {
        if (movement.movementEnabled)
        {
            var trans = movement.transform;
            trans.position = Vector3.Lerp(trans.position, new Vector3(trans.position.x, trans.position.y, transform.position.z), Time.deltaTime);
        }
    }

}