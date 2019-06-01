using UnityEngine;
using Engine;
using UnityEngine.EventSystems;

public class WorldPointer : MonoBehaviour, IPointerClickHandler, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public static event System.Action<Transform> Click;

    public LayerMask colliisonLayer;
    public Transform camTransform;
    private bool isPressed;
    private Vector2 dragDelta;
    private Vector2 startPosition;
    private Vector2 currentTouchPosition;
    private bool wasPressed;
    public float swipeSpeed = 1;

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        Vector2 hitPoint = GetTouchPosition(eventData.position);
    }

    Vector3 GetTouchPosition(Vector2 pos)
    {
        var cam = WorldCameraMovement.CurrentCamera;
        if(!cam)
        {
            Debug.LogError("Current cam is null");
            return Vector3.zero;
        }
        Ray ray = cam.ScreenPointToRay(pos);
        RaycastHit groundHit;

        if (Physics.Raycast(ray, out groundHit, Mathf.Infinity, colliisonLayer.value, QueryTriggerInteraction.Ignore))
        {
            //Debug.Log("Touched: " + groundHit.transform.name);
            Click?.Invoke(groundHit.transform);
            return groundHit.point;
        }
        else
        {
            //Debug.Log("Touched: None");
            return new Vector3(0, 0, 0);
        }
    }

    void Update()
    {
        if(isPressed)
        {
            var distance = dragDelta.x * swipeSpeed;
            var distanceY = dragDelta.y * swipeSpeed;
            if (WorldCameraMovement.CurrentCamera)
            {
                WorldCameraMovement.CurrentCamera.transform.position += Vector3.right * distance;
                WorldCameraMovement.CurrentCamera.transform.position += Vector3.forward * distanceY;
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
        isPressed = false;
    }

    public void OnDrag(PointerEventData data)
    {
        dragDelta = currentTouchPosition - data.position;
        currentTouchPosition = data.position;
    }
}