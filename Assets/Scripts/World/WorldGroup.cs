using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Engine.UI;

public class WorldGroup : MonoBehaviour
{
    public MeshRenderer modelRenderer;
    public Text groupName;
    bool pressed;
    public Transform camAnchor;
    public Transform cameraRotator;

    WorldCameraMovement camMovement;


    private Transform cam;
    private void Start()
    {
        World.Instance.Window.movement.PointerUp += WorldPointerUp;
        cam = World.Instance.worldCamera.transform;
        camMovement = World.Instance.Window.movement;
    }

    private void OnDestroy()
    {
        World.Instance.Window.movement.PointerUp -= WorldPointerUp;
    }

    void WorldPointerUp(Vector3 position)
    {
        if (camMovement.Locked) return;
        if(modelRenderer.bounds.Intersects(World.Instance.PointerBounds))
        {
            Debug.Log("Tapped on " + name);  
            Tapped();
            pressed = true;
        }
        else
        {
            pressed = false;
        }
    }

    IEnumerator MoveToAnchor()
    {
        camMovement.Locked = true;
        groupName.enabled = false;
        float speed = 2f;
        float time = 0;
        Vector3 startPos = cam.position;
        camMovement.MaxDistance = Vector3.Distance(cameraRotator.position, camAnchor.position);
        while(time < 1)
        {
            time += Time.deltaTime * speed;
            cam.position = Vector3.Lerp(startPos, camAnchor.position, time);
            cam.rotation = Quaternion.Lerp(cam.rotation, Quaternion.LookRotation(Engine.Vector.Direction(cam.position, cameraRotator.transform.position)), time);
            yield return null;
        }
        while(camMovement.Locked)
        {
            cam.LookAt(cameraRotator);
            yield return null;
        }

    }

    void Tapped()
    {
        WorldCameraMovement.SelectedWorldGroup = this;
        camMovement.AttachToMap(cameraRotator);
        StartCoroutine(MoveToAnchor());
    }





    //private void OnDrawGizmos()
    //{
    //    Gizmos.DrawCube(transform.position, modelRenderer.bounds.extents * 2);

    //    if(World.Instance != null)
    //     Gizmos.DrawSphere(World.Instance.PointerPosition, 50);
    //}
}