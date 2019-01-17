using Engine;
using UnityEngine;


public class CharacterWagon : CharacterMovementPlayer, ILocalPlayer
{
    public Track track;
    public Track nextTrack;
    public Track previousTrack;
    public float curTrackPos = 0;
    public ParticleSystem[] sparks;
    public Transform[] wheels;
    public Transform neck;
    public Transform chest;
    public float wagonSpeed = 1;
    public float distance;

    public int trackIndex;

    float speedTimer;

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
        var cam = Controller.Instance.gameCamera;
        cam.SetTarget(transform);
        cam.ResetView();
        cam.regularUpdate = true;
        cam.ChangeToWagonView();
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
         
            }

        }
        catch
        {
            Debug.Log("Buttons are not initialized, you can still use keyboard for movement");
            buttonsInitialized = false;
        }
    }

    protected override void Inputs()
    {

    }

    protected override void Rotation()
    {

    }

    public float eulerAngle = 0;
    public float wagonAngleZ = 0;
    public Vector3 endPointForward;
    float lean;
    Vector3 prevPos;
    protected override void Update()
    {
        wheels[0].Rotate(0, 0, -8 * wagonSpeed);
        wheels[1].Rotate(0, 0, -8 * wagonSpeed);
        curTrackPos += Time.deltaTime * wagonSpeed;
        if (curTrackPos >= 1 && nextTrack != null)
        {
            previousTrack = track;
            if(nextTrack != track)
                trackIndex++;
            track = nextTrack;
            curTrackPos = curTrackPos % 1;
        }
        if (track != null)
        {
            Vector3 pos = track.GetPosition(curTrackPos);         
            eulerAngle = Vector3.SignedAngle(transform.forward, endPointForward, Vector3.up);
            transform.rotation = Quaternion.LookRotation(Vector.Direction(prevPos, pos));
            prevPos = transform.position;
            transform.position = pos;
        }
        else
        {
            Debug.Log("track was null");
        }
        WagonMovement();
        WagonRotation(eulerAngle);

        if (finalAngle > 20)
        {
            sparks[0].Emit(5);
        }
        else if (finalAngle < -20)
        {
            sparks[1].Emit(5);
        }
        if(speedTimer > 1)
        {
            speedTimer = 0;
            wagonSpeed += Time.deltaTime / 5;
        }
        else
        {
            speedTimer += Time.deltaTime;
        }

        if (!character.IsDead)
            distance += wagonSpeed * Time.deltaTime * 10;
    }

    void WagonMovement()
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
        if (horDistance > 1)
        {
            pointingDir = Vector.Direction(horTouched, curHorTouched);
            angle = -Vector2.SignedAngle(Vector2.up, pointingDir);
        }
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        if (Input.GetKey(KeyCode.LeftArrow))
            angle = -90;
        if (Input.GetKey(KeyCode.RightArrow))
            angle = 90;
#endif

        if (!pressedHorizontalCurrent && horPressed)
        {
            horPressed = false;
        }


         lean = Mathf.Lerp(lean, angle, Time.deltaTime * 10);
    }

    public float maxAngle = 30;
    float finalAngle;
    public float critical = 0;
    public float maxCritical = 800;
    public float criticalFactor = 25;
    void WagonRotation(float a)
    {
        if (a > 0)
        {
            if (wagonAngleZ < 90 && a > wagonAngleZ)
                wagonAngleZ += wagonSpeed * 4;
            else
                wagonAngleZ = Mathf.Lerp(wagonAngleZ, 0, Time.deltaTime * 8);
        }
        else if(a <= 0)
        {
            if (wagonAngleZ > -90 && a < wagonAngleZ)
                wagonAngleZ -= wagonSpeed * 4;
            else
                wagonAngleZ = Mathf.Lerp(wagonAngleZ, 0, Time.deltaTime * 8);
        }
        finalAngle = -wagonAngleZ + lean;
        var toRot = new Vector3(0, 180, (finalAngle) / 3);
        model.transform.localEulerAngles = toRot;
        var neckRot = neck.transform.localEulerAngles;
        neckRot.z = (wagonAngleZ + lean) / 3;
        neck.transform.localEulerAngles = neckRot;
        var chestRot = chest.localEulerAngles;
        chestRot.z = (wagonAngleZ) / 4;
        chest.localEulerAngles = chestRot;
        if(CheckCollision())
        {
            Controller.Instance.gameCamera.SetTarget(null);
            rb.useGravity = true;
            rb.velocity = transform.forward * wagonSpeed * 50;
            character.IsDead = true;
            enabled = false;
            Invoke("Restart",1);
        }

    }

    protected override void FixedUpdate()
    {
        
    }

    void Restart()
    {
        DieNonAnimation();
    }

    public int criticalDir;
    bool CheckCollision()
    {
        criticalDir = finalAngle > 0 ? 1 : -1;
        if (Mathf.Abs(finalAngle) > 30)
        {
            if (critical < maxCritical * 2)
                critical += Mathf.Abs(finalAngle) - Mathf.Abs(lean);
        }
        else
        {
            if (critical > criticalFactor)
                critical -= criticalFactor;
            else
                critical = 0;
        }
        if (critical > maxCritical)
            return true;
        return false;
    }

    void OnDrawGizmos()
    {
        if(nextTrack!=null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawCube(nextTrack.transform.position, new Vector3(20, 20, 20));
        }
        if (track != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawCube(track.transform.position, new Vector3(20, 20, 20));
        }
        if (previousTrack != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawCube(previousTrack.transform.position, new Vector3(20, 20, 20));
        }
    }
}