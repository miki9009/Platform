using UnityEngine;
using Engine;

public class DirectionTest : MonoBehaviour
{
    public Transform cam;
    private void OnGUI()
    {
        Engine.Draw.TextColor(10, 10, 255, 0, 0, 1, "Direction: " + transform.rotation);
        Engine.Draw.TextColor(10, 50, 255, 0, 0, 1, "PointingDir: " + Quaternion.LookRotation(pointingDir));
        Engine.Draw.TextColor(10, 90, 255, 0, 0, 1, "Angle: " + angle);
    }

    private void Update()
    {
        cam.LookAt(transform);
        GestureMovement();

    }

    Vector2 horTouched;
    Vector2 verTouched;
    bool horPressed;
    bool verPressed;
    float verDistance;
    float horDistance;
    Vector3 lastAttackTouchPosition;
    Vector3 curHorTouched;
    Vector3 pointingDir;
    float forwardPower;
    float angle;
    float jumpInput;

    void GestureMovement()
    {
        bool pressedHorizontalCurrent = false;
        bool pressedVerticalCurrent = false;
        int touchCount = Input.touchCount;
        var touches = Input.touches;
        jumpInput = 0;
        horDistance = 0;
        forwardPower = 0;
        pressedHorizontalCurrent = false;
        angle = 0;

        for (int i = 0; i < Input.touchCount; i++)
        {
            if (touches[i].position.x < Screen.width / 2)
            {
                pressedHorizontalCurrent = true;
                if (!horPressed)
                {
                    horPressed = true;
                    horTouched = touches[i].position;
                }
                else
                {
                    curHorTouched = touches[i].position;
                    horDistance = Vector3.Distance(horTouched, curHorTouched);
                }
            }
            else
            {
                pressedVerticalCurrent = true;
                if (!verPressed)
                {
                    verPressed = true;
                    verTouched = touches[i].position;
                }
                else
                {
                    lastAttackTouchPosition = touches[i].position;
                    verDistance = Vector3.Distance(verTouched, lastAttackTouchPosition);
                }
            }
        }
#if UNITY_EDITOR
        if (Input.GetMouseButton(0))
        {
            if (Input.mousePosition.x < Screen.width / 2)
            {
                pressedHorizontalCurrent = true;
                if (!horPressed)
                {
                    horPressed = true;
                    horTouched = Input.mousePosition;
                }
                else
                {
                    curHorTouched = Input.mousePosition;
                    horDistance = Vector3.Distance(horTouched, curHorTouched);
                }
            }
            else
            {
                pressedVerticalCurrent = true;
                if (!verPressed)
                {
                    verPressed = true;
                    verTouched = Input.mousePosition;
                }
                else
                {
                    lastAttackTouchPosition = Input.mousePosition;
                    verDistance = Vector3.Distance(verTouched, lastAttackTouchPosition);
                }
            }
        }

#endif
        if (horDistance > 1)
        {
            pointingDir = Vector.Direction(horTouched, curHorTouched);
            angle = -Vector2.SignedAngle(Vector2.up, pointingDir);
            forwardPower = Mathf.Clamp(horDistance, 0, 50) / 50;
            transform.eulerAngles = new Vector3(0, cam.eulerAngles.y + angle, 0);
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
            }
        }
        if (verPressed && !pressedVerticalCurrent)
        {
            verPressed = false;
            if (verDistance < 50)
            {
                jumpInput = 0;
            }
        }
        verDistance = Mathf.Clamp(verDistance - 1, 0, 50);
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpInput = 1;
        }

        float hor = Input.GetAxis("Horizontal");
        float ver = Input.GetAxis("Vertical");

        if (hor != 0 || ver != 0)
        {
            //pointingDir = new Vector3(hor, ver);
            angle = -Vector2.SignedAngle(Vector2.up, pointingDir);

            forwardPower = 1;
        }

#endif
    }
}