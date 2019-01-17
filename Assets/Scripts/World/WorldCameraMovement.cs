using Engine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class WorldCameraMovement : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public event Action<Vector3> PointerUp;

    public float swipeSpeed = 0.2f;
    bool wasPressed;
    Vector2 startPosition;
    Vector2 currentTouchPosition;

    public Vector3 worldExtentsMin;
    public Vector3 worldExtentsMax;

    Vector3 lastWorldPos;
    Quaternion lastWorldRot;

    bool _locked;
    public bool Locked
    {
        get
        {
            return _locked;
        }
        set
        {
            _locked = value;
            Debug.Log("Locked value: " + value);
        }
    }

    public static WorldGroup SelectedWorldGroup
    {
        get;set;
    }

    void Awake()
    {
        Locked = false;
    }

    public void AttachToMap(Transform map)
    {
        camTransform.SetParent(map);
        lastWorldPos = camTransform.position;
        lastWorldRot = camTransform.rotation;
        World.Instance.BackToFullViewButtonEnable(true);
    }

    [EventMethod]
    public static void BackToWorld()
    {
        if (World.Instance!=null)
        {
            World.Instance.StartCoroutine(World.Instance.Window.movement.DetachFromMap());
            if (SelectedWorldGroup != null)
                SelectedWorldGroup.groupName.enabled = true;

            World.Instance.BackToFullViewButtonEnable(false);
        }
    }

    float time = 0;
    IEnumerator DetachFromMap()
    {
        Locked = false;
        camTransform.SetParent(null);
        time = 0;
        float speed = 2f;
        Vector3 startPos = camTransform.position;
        Quaternion startRot = camTransform.rotation;
        while (time < 1)
        {
            time += Time.deltaTime * speed;
            camTransform.position = Vector3.Lerp(startPos, lastWorldPos, time);
            camTransform.rotation = Quaternion.Lerp(startRot, lastWorldRot, time);
            yield return null;
        }
    }

    Vector2 dragDelta;

    float releastTime;
    float currentRotationSpeed;

    float distance;
    bool isPressed;
    Vector3 startEulers;
    float sign;
    float curAutomaticRotationTime;
    Vector3 direction;
    bool isDraging;
    int dir;
    Transform camTransform;
    Vector3 pos;
    public float MaxDistance
    {
        get;set;
    }

    void Start()
    {
        World.Initialized += Initialize;
    }

    void OnDestroy()
    {
        World.Initialized -= Initialize;
    }

    void Initialize()
    {
        camTransform = World.Instance.worldCamera.transform;
    }

    void Update()
    {
        if (camTransform == null) return;
        if (!isPressed)
        {
            dragDelta *= 0.95f;

            if (dragDelta.magnitude < 0.01f)
                dragDelta = Vector2.zero;
        }

            distance = dragDelta.magnitude * swipeSpeed;
        if(distance > 1)
        {
            if(Locked)
            {
                if(SelectedWorldGroup == null)
                {
                    Locked = false;
                    return;
                }
                var eulers = SelectedWorldGroup.cameraRotator.localEulerAngles;
                SelectedWorldGroup.cameraRotator.localEulerAngles = new Vector3(eulers.x, eulers.y + distance * swipeSpeed * (dragDelta.x > 0 ? 1 : -1), eulers.z);

            }
            else
            {
                pos = camTransform.position + direction * distance * swipeSpeed;
                pos.x = Mathf.Clamp(pos.x, worldExtentsMin.x, worldExtentsMax.x);
                pos.z = Mathf.Clamp(pos.z, worldExtentsMin.z, worldExtentsMax.z);
                camTransform.position = pos;
            }

        }
    }

    public void OnPointerDown(PointerEventData data)
    {
        isPressed = true;
        dragDelta = Vector2.zero;
        startPosition = data.position;
        currentTouchPosition = startPosition;
        wasPressed = true;
    }

    public void OnPointerUp(PointerEventData data)
    {
        if (!isDraging)
            PointerUp?.Invoke(data.position);
        isPressed = false;
        isDraging = false;
        currentRotationSpeed = 0f;
    }

    public void OnDrag(PointerEventData data)
    {
        isDraging = true;
        dragDelta = currentTouchPosition - data.position;
        direction = Engine.Vector.Direction(currentTouchPosition, data.position);
        direction.z = direction.y;
        direction.y = 0;
        currentTouchPosition = data.position;
        
    }

}
