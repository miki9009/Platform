using UnityEngine;
using UnityEngine.UI;

public class Analog : MonoBehaviour
{
    public RectTransform rect;
    public RectTransform childRect;
    public Image image;
    public Image childImage;

    Vector3 childStartPos;

    CharacterMovementPlayer characterMovement;
    CharacterMovementPlayer CharacterMovement
    {
        get
        {
            if (characterMovement == null)
            {
                if (Controller.Instance != null)
                {
                    var character = Controller.Instance.character;
                    if (character != null)
                    {
                        characterMovement = (CharacterMovementPlayer)character.movement;
                    }
                }
            }
            return characterMovement;
        }
    }

    private void Awake()
    {
        childStartPos = childRect.position;
    }

    private void Update()
    {
        if(CharacterMovement!= null && CharacterMovement.Touched)
        {
            if(!childImage.enabled)
            {
                childImage.enabled = true;
            }
            rect.position = CharacterMovement.StartTouchedPosition;
            childRect.position = CharacterMovement.CurrentTouchedPosition;
        }
        else
        {
            if(childImage.enabled)
            {
                childImage.enabled = false;
            }
        }
        
    }
}