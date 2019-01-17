using UnityEngine;

public class SeaTrigger : MonoBehaviour
{
    public GameObject sea;

    private void OnTriggerEnter(Collider other)
    {
        sea.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        sea.SetActive(false);
    }
}