using UnityEngine;
using Engine;
using UnityEngine.EventSystems;

public class WorldPointer : MonoBehaviour, IPointerClickHandler
{
    public static event System.Action<Transform> Click;

    public LayerMask colliisonLayer;

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
            Debug.Log("Touched: " + groundHit.transform.name);
            Click?.Invoke(groundHit.transform);
            return groundHit.point;
        }
        else
        {
            Debug.Log("Touched: None");
            return new Vector3(0, 0, 0);
        }
    }

    
}