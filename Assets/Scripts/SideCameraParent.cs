using UnityEngine;

public class SideCameraParent : MonoBehaviour
{
    public Transform target;
    public float distanceFromTarget;
    public Transform cameraAnchor;

    private void Start()
    {
        SetCamera();
    }

    private void Update()
    {
        if (target!=null)
        transform.position = target.position - transform.forward * distanceFromTarget;
    }

    public void SetCamera()
    {
        var cam = Controller.Instance.gameCamera.transform;
        cam.parent = cameraAnchor;
        cam.localRotation = Quaternion.identity;
        cam.localPosition = Vector3.zero;
        cam.GetComponent<GameCamera>().enabled = false;
    }
}