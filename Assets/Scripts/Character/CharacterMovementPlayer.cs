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

    public float angle;

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
        horDistance = 0;
        forwardPower = 0;
        pressedHorizontalCurrent = false;

        if (btnMovement.Pressed || horPressed)
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
                    Engine.Log.Print("Jump Input");
                    verTouched.y = 0;
                    lastAttackTouchPosition.y = 0;
                    verDistance = 0;
                }
                else
                {
                    if (!OnGround)
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
        Debug.Log("Jump tapped");
        jumpInput = 1;
    }

    protected void ButtonsMovement()
    {
        horInput = 0;
        if (buttonsInitialized)
        {
            if (btnRight.Pressed) horInput = 1;
            if (btnLeft.Pressed) horInput = -1;
            if (btnJump.Pressed)
            {
                Engine.Log.Print("Jump input");
            }
        }
    }

    protected float modelZFactor;
    float amplitude = 0;
    float prevY;
    protected override void Rotation()
    {
        var vec = Controller.Instance.gameCamera.transform.forward;
        vec.y = 0;
        modelZFactor = Mathf.Clamp(Mathf.Lerp(modelZFactor, amplitude / 2, Time.deltaTime * stats.turningSpeed), -20, 20);
        Quaternion rot = Quaternion.Euler(targetEuler.x, targetEuler.y, modelZFactor);
        prevY = transform.eulerAngles.y;
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, 0.2f);

        amplitude = (prevY - transform.eulerAngles.y) * rb.velocity.magnitude;

    }

    //void OnGUI()
    //{
        //Draw.TextColor(10, 300, 255, 0, 0, 1, "ModelZ: " + modelZFactor);
        //Draw.TextColor(10, 350, 255, 0, 0, 1, "amplitude: " + amplitude);
        //Draw.TextColor(10, 250, 255, 0, 0, 1, "velo: " + rb.velocity.z);

        //Draw.TextColor(10, 350, 255, 0, 0, 1, "prev Y: " + prevY);
        //Draw.TextColor(10, 400, 255, 0, 0, 1, "cur Y: " + transform.eulerAngles.y);
        //Draw.TextColor(10, 450, 255, 0, 0, 1, "ampli: " + ;
        //Draw.TextColor(10, 400, 255, 0, 0, 1, "Is Touched: " + isTouched);
        //Draw.TextColor(10, 450, 255, 0, 0, 1, "Is Rolling: " + isRolling);
    //}

}