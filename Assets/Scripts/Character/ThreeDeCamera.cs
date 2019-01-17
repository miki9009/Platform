using UnityEngine;

public class ThreeDeCamera : MonoBehaviour
{
    public bool move = true;
    public float speed = 5;
    public Transform target;
    public float y;
    public float z;

    private void OnTriggerEnter(Collider other)
    {
        move = false;
    }

    private void OnTriggerExit(Collider other)
    {
        move = true;
    }

    private void Update()
    {
        if(move)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position - target.forward * z + Vector3.up * y, speed * Time.deltaTime);
        }
        transform.LookAt(target);

    }
}