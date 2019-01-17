using UnityEngine;

public class TargetPointer : MonoBehaviour
{
    public Transform target;
    public Transform origin;

    public static event System.Action Dectivate;

    private void Update()
    {
        if (target == null || origin == null || !target.gameObject.activeInHierarchy)
        {
            gameObject.SetActive(false);
            return;
        }
        transform.position = origin.position + Vector3.up;
        transform.rotation = Quaternion.LookRotation(Engine.Vector.Direction(transform.position, target.position));
    }
}

public interface ITargetPointer
{
    void Activate(Transform target);
    void Deactivate();
    TargetPointer Pointer {get;set;}

}