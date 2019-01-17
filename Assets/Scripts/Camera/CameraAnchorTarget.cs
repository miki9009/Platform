using UnityEngine;

public class CameraAnchorTarget : MonoBehaviour
{
    public CharacterMovement characterMovement;

    private void Start()
    {
        if (characterMovement == null)
        {
            characterMovement = Controller.Instance.character.GetComponent<CharacterMovement>();
        }
    }

    private void Update()
    {
        if (characterMovement.onGround)
        {
            transform.position = characterMovement.transform.position;
        }
    }
}