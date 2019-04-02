using UnityEngine;
using Engine;
using System;

public class CharacterOstrichPlayer : CharacterMovementPlayer, ILocalPlayer
{

    protected override void Initialize()
    {
        Controller.Instance.gameCamera.regularUpdate = false;
        Controller.Instance.gameCamera.ChangeToRegularCharacterView();
        character.stats.runSpeed *= 1.5f;
        try
        {
            btnMovement = GameGUI.GetButtonByName("ButtonMovement");
            ButtonsInput = Controller.Instance.ButtonMovement;
            btnAttack = GameGUI.GetButtonByName("ButtonAttack");
            btnJump = GameGUI.GetButtonByName("ButtonForward");
            btnAttack.OnTapPressed.AddListener(Attack);
            Movement = GestureMovement;

        }
        catch (Exception ex)
        {
            Movement = GestureMovement;
            Debug.Log("Buttons are not initialized, you can still use keyboard for movement. " + ex);
            buttonsInitialized = false;
        }
    }

    protected override void Update()
    {
        //base.Update();
        curPos = transform.position;
        Movement();
        Jump();
        //if (attack)
        //{
            attack = false;
            //AttackCollision();
        //}
    }

    protected override void FixedUpdate()
    {
        //base.FixedUpdate();
        Rotation();
        OstrichMove();
        if (onGround)
            rb.AddForce(Vector3.up * addForce);
    }

    public void OstrichMove()
    {
        var velo = rb.velocity;
        float y = velo.y;
        velo.y = 0;
        float mag = velo.magnitude;
        anim.SetFloat("Hspeed", mag);
        //rb.rotation = Quaternion.Lerp(rb.rotation, transform.rotation, Time.deltaTime);
        if (velo.magnitude < stats.runSpeed)
        {
            //rb.AddForce(rb.rotation.Vector() * forwardPower * force, ForceMode.Acceleration);
            velo = rb.rotation.Vector() * (mag + forwardPower);
            velo.y = y;
            rb.velocity = velo;
        }
    }

    protected override void GestureMovement()
    {
        bool pressedHorizontalCurrent = false;
        int touchCount = Input.touchCount;
        var touches = Input.touches;
        jumpInput = 0;
        horDistance = 0;
        forwardPower = 0;
        pressedHorizontalCurrent = false;
        angle = 0;

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
            forwardPower = 1;
        }
        targetEuler = new Vector3(0, Camera.eulerAngles.y + angle, 0);
#endif

        if (horDistance > 1)
        {
            pointingDir = Vector.Direction(horTouched, curHorTouched);
            angle = -Vector2.SignedAngle(Vector2.up, pointingDir);
            forwardPower = Mathf.Clamp(horDistance, 0, 100) / 100;
            targetEuler = new Vector3(0, Camera.eulerAngles.y + angle, 0);
        }

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

        if (btnJump.isTouched)
        {
            jumpInput = 1;
        }
    }


}