using UnityEngine;

public class DeactivateAfterTime : MonoBehaviour
{
    public float time;

    void OnEnable()
    {
        Invoke("DeactivateMe", time);
    }

    void DeactivateMe()
    {
        gameObject.SetActive(false);
    }
}
