using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    public bool destroyOnStart = true;
    public float timeToDestroy = 5;

    void Start()
    {
        if (destroyOnStart)
        {
            Invoke("DestoyMe", timeToDestroy);
        }
    }

    void DestroyMe()
    {
        Destroy(gameObject);
    }

    public void DestroyAfter(float time)
    {
        Invoke("DestroyMe", time);
    }
}