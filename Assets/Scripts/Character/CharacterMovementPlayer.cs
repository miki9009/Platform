using Engine;
using Engine.UI;
using System;
using UnityEngine;

public class CharacterMovementPlayer : CharacterMovement, ILocalPlayer
{
    protected Button btnLeft;
    protected Button btnRight;
    [HideInInspector]
    public Button btnJump;
    protected Button btnAttack;
    protected Button btnForward;
    protected Button btnMovement;
    protected bool buttonsInitialized = true;
    protected Vector2 horTouched;
    protected Vector2 verTouched;
    protected bool horPressed;
    protected bool verPressed;
    protected float verDistance;
    protected float horDistance;
    protected Vector3 lastAttackTouchPosition;
    protected Vector3 curHorTouched;
    protected Vector3 pointingDir;
    public float maxAngle = 180;

    protected float angle;

    public bool Touched
    {
        get
        { return horPressed; }
    }
    public Vector2 StartTouchedPosition
    {
        get
        {
            return horTouched;
        }
    }
    public Vector2 CurrentTouchedPosition
    {
        get
        {
            return curHorTouched;
        }
    }

    public override bool IsPlayer
    {
        get
        {
            return true;
        }
    }

    public override bool IsBot
    {
        get
        {
            return false;
        }
    }

    protected override void Initialize()
    {
        Controller.Instance.gameCamera.regularUpdate = false;
        Controller.Instance.gameCamera.ChangeToRegularCharacterView();
        try
        {
            btnMovement = GameGUI.GetButtonByName("ButtonMovement");
            ButtonsInput = Controller.Instance.ButtonMovement;
            btnAttack = GameGUI.GetButtonByName("ButtonAttack");
            btnJump = GameGUI.GetButtonByName("ButtonForward");
            btnAttack.OnTapPressed.AddListener(Attack);

            if (ButtonsInput)
            {
                btnAttack.gameObject.SetActive(true);
                btnJump.gameObject.SetActive(true);
                var rect = GameGUI.GetButtonByName("Action").GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(700, rect.anchoredPosition.y);
                Movement = ButtonsMovement;

            }
            else
            {
                Movement = GestureMovement;
                btnJump.OnTapRelesed.AddListener(OnJumpTapped);
            }

        }
        catch (Exception ex)
        {
            //Movement = ButtonsMovement;
            Debug.Log("Buttons are not initialized, you can still use keyboard for movement. " + ex );
            buttonsInitialized = false;
        }
    }

    void OnDestroy()
    {
        btnJump.OnTapRelesed.RemoveListener(OnJumpTapped);
    }

    Transform cam;
    public Transform Camera
    {
        get
        {
            if (cam == null)
            {
                cam = Controller.Instance.gameCamera.transform;
            }
            return cam;
        }
    }

    protected override void Inputs()
    {

    }

    protected virtual void GestureMovement()
    {
        bool pressedHorizontalCurrent = false;
        int touchCount = Input.touchCount;
        var touches = Input.touches;
        //jumpInput = 0;
        horDistance = 0;
        forwardPower = 0;
        pressedHorizontalCurrent = false;
        //angle = 0;

        if (btnMovement.isTouched || horPressed)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                if (touches[i].position.x < Screen.width / 2)
                {
                    pressedHorizontalCurrent = true;
                    if (!horPressed)
                    {
                        horPressed = true;
                        horTouched = btnMovement.transform.position;
                    }
                    else
                    {
                        curHorTouched = touches[i].position;
                        horDistance = Vector3.Distance(horTouched, curHorTouched);
                    }
                }
            }

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            if (Input.GetMouseButton(0))
            {
                if (Input.mousePosition.x < Screen.width / 2)
                {
                    pressedHorizontalCurrent = true;
                    if (!horPressed)
                    {
                        horPressed = true;
                        horTouched = btnMovement.transform.position;
                    }
                    else
                    {
                        curHorTouched = Input.mousePosition;
                        horDistance = Vector3.Distance(horTouched, curHorTouched);
                    }
                }
            }
#endif
        }

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        float hor = Input.GetAxisRaw("Horizontal");
        float ver = Input.GetAxisRaw("Vertical");
        if(hor != 0 || ver != 0)
        {


        if (hor < 0) angle = 270;
        if (hor > 0) angle = 90;
        if (ver > 0) angle = 0;
        if (ver < 0) angle = 180;
        if (ver > 0 && hor > 0) angle = 45;
        if (ver > 0 && hor < 0) angle = 315;
        if (ver < 0 && hor > 0) angle = 135;
        if (ver < 0 && hor < 0) angle = 225;
        if (hor != 0 || ver != 0)
        {
            forwardPower = Mathf.Lerp(forwardPower,1,Time.deltaTime * 50);
        }
        else
            {
                forwardPower = Mathf.Lerp(forwardPower, 0, Time.deltaTime * 10);
            }
        targetEuler = new Vector3(0, Camera.eulerAngles.y + angle, 0);
        }
#endif

        if (horDistance > 1)
        {
            pointingDir = Vector.Direction(horTouched, curHorTouched);
            angle = -Vector2.SignedAngle(Vector2.up, pointingDir);
            angle = Mathf.Clamp(angle, -maxAngle, maxAngle);
            forwardPower = Mathf.Clamp(horDistance, 0, 100) / 100;
            targetEuler = new Vector3(0, Camera.eulerAngles.y + angle, 0);
        }

#if UNITY_EDITOR


        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpInput = 1;
            Jump();
        }
  

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            //Debug.Log("Attack Invoked");
            btnAttack.OnTapPressedInvoke();
        }
#endif

        if (!pressedHorizontalCurrent && horPressed)
        {
            horPressed = false;
        }

        if (!verPressed)
        {
            if (verDistance > 30)
            {
                if (verTouched.y < lastAttackTouchPosition.y)
                {
                    Console.WriteLine("Jump Input");
                    jumpInput = 1;
                    verTouched.y = 0;
                    lastAttackTouchPosition.y = 0;
                    verDistance = 0;
                }
                else
                {
                    if (!onGround)
                    {
                        lastAttackTouchPosition.y = 0;
                        rb.velocity = Vector3.down * stats.attackForce;
                        anim.SetTrigger("attack");
                        anim.SetBool("attackStay", true);
                    }
                }
            }
        }
    }

    void OnJumpTapped()
    {
        if(btnJump.PressedTime < 0.25f)
        {
            jumpInput = 1;
            Jump();
        }
    }

    protected void ButtonsMovement()
    {
        horInput = 0;
        if (buttonsInitialized)
        {
            if (btnRight.isTouched) horInput = 1;
            if (btnLeft.isTouched) horInput = -1;
            if (btnJump.isTouched)
            {
                Console.WriteLine("Jump input");
                jumpInput = 1;
            }
            else
                jumpInput = 0;
        }
    }

    protected float modelZFactor;
    protected override void Rotation()
    {
        var vec = Controller.Instance.gameCamera.transform.forward;
        vec.y = 0;
        Quaternion rot = Quaternion.Euler(targetEuler);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, 0.2f);
    }

    bool isTouched;
    protected override void ShieldMovement()
    {
        if (timeBeforeAnotherRoll > 0) return;
        isTouched = false;
        isTouched = btnMovement.isTouched;

#if UNITY_EDITOR
        if (!isTouched)
            isTouched = (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) && Input.GetKey(KeyCode.LeftShift);
#endif

        if (!isRolling)
        {
            if (isTouched)
            {
                anim.SetTrigger("roll");
                isRolling = true;
            }
        }
        else
        {
            forwardPower = 1;
            Move();
        }

        if(isTouched)
        {
            GestureMovement();
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(0, angle + 90, 0), character.stats.turningSpeed);
        }
    }

    //void OnGUI()
    //{
    //    Draw.TextColor(10, 300, 255, 0, 0, 1, "Movement Enabled: " + MovementEnabled);
    //    Draw.TextColor(10, 350, 255, 0, 0, 1, "Shield up: " + shieldUp);
    //    Draw.TextColor(10, 400, 255, 0, 0, 1, "Is Touched: " + isTouched);
    //    Draw.TextColor(10, 450, 255, 0, 0, 1, "Is Rolling: " + isRolling);
    //}

}