using UnityEngine;
using UnityEngine.UI;

public class Analog : MonoBehaviour
{
    public RectTransform rect;
    public RectTransform childRect;
    public Image image;
    public Image childImage;
    public Engine.UI.Button button;
    public CanvasGroup canvasGroup;

    Vector3 childStartPos;
    Vector3 analogStartPos;

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

    private void OnDestroy()
    {
        GameManager.CustomSceneLoaded -= ResetToStartPos;
    }


    private void Start()
    {
        //analogStartPos = transform.position;
        //ResetToStartPos();
        GameManager.CustomSceneLoaded += ResetToStartPos;
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

    public void SetPosition()
    {
        //transform.position = button.TouchPosition;
        canvasGroup.alpha = 1;
    }

    public void ResetToStartPos()
    {
        //transform.position = analogStartPos;
        canvasGroup.alpha = 0.2f;
    }
}