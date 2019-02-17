using Engine;
using UnityEngine;


public class CharacterBoat : CharacterMovementPlayer, ILocalPlayer
{
    public ParticleSystem waterDrops;

    public override bool IsPlayer
    {
        get
        {
            return true;
        }
    }

    //Transform sensor;
    protected override void Initialize()
    {
        anim.Play("Scooter");
        Debug.Log("Sit Scooter");
        var cam = Controller.Instance.gameCamera;
        cam.SetTarget(transform);
        cam.ResetView();
        cam.regularUpdate = false;
        cam.ChangeToScooterView();
        cam.minDistance = 10;
        cam.maxDistance = 10;
        iceForce = 30;
        character.stats.runSpeed = 30;
        cam.speed = 2;
        cam.mainCamera.fieldOfView = 40;
        cam.mainCamera.farClipPlane = 250;
        // sensor = new GameObject("WagonSensor").transform;
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

                btnLeft = GameGUI.GetButtonByName("ButtonLeft");
                btnLeft.gameObject.SetActive(true);
                btnRight = GameGUI.GetButtonByName("ButtonRight");
                btnRight.gameObject.SetActive(true);
                btnJump.gameObject.SetActive(true);
                var rect = GameGUI.GetButtonByName("Action").GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(700, rect.anchoredPosition.y);
                Movement = ButtonsMovement;
            }
            else
            {
                Movement = GestureMovement;
            }
        }
        catch
        {
            Debug.Log("Buttons are not initialized, you can still use keyboard for movement");
            buttonsInitialized = false;
        }
    }

    public override void Die()
    {
       // base.Die();
    }

    public override void Attack()
    {
        //base.Attack();
    }

    public override void StateAnimatorInitialized()
    {
        //base.StateAnimatorInitialized();
    }

    public override void SetAnimationHorizontal(Vector3 velo)
    {
       // base.SetAnimationHorizontal(velo);
    }

    public override void Hit(Enemy enemy = null, int hp = 1, bool heavyAttack = false)
    {
       // base.Hit(enemy, hp, heavyAttack);
    }

    float mag;
    public override void Move()
    {
        var velo = rb.velocity;
        float y = velo.y;
        velo.y = 0;
        mag = velo.magnitude;
        rb.rotation = Quaternion.Lerp(rb.rotation, transform.rotation, Time.deltaTime);
        rb.AddForce(rb.rotation.Vector() * forwardPower * iceForce, ForceMode.Acceleration);
        if (onGround)
           rb.AddForce(Vector3.up * forwardPower * mag / 40, ForceMode.VelocityChange);
        if (onGround && Engine.Math.Probability(mag/stats.runSpeed))
        {
            waterDrops.Emit(4);
        }

    }

    protected override void Rotation()
    {
        var vec = Controller.Instance.gameCamera.transform.forward;
        vec.y = 0;
        Quaternion rot = Quaternion.Euler(targetEuler);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * stats.turningSpeed / 2);
#if (UNITY_EDITOR)
        if (angle > 180)
            angle = -90;
        angle = Mathf.Clamp(angle, -maxAngle, maxAngle);
#endif
        float factor = angle / 90;
        modelZFactor = Mathf.Clamp(Mathf.Lerp(modelZFactor, factor * 90, Time.deltaTime * stats.turningSpeed / 2),-30,30);
        var euler = model.transform.localEulerAngles;
        model.transform.localEulerAngles = new Vector3(-30 * mag / stats.runSpeed, 0, -modelZFactor);
    }

    protected override void FixedUpdate()
    {
        Move();
        Rotation();
    }

}