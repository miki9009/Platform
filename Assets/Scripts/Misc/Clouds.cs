using UnityEngine;


public class Clouds : MonoBehaviour
{
    public float offset = 10;
    public float speed = 1;

    public bool increase;

    Vector3 startPos;
    Vector3 endPos;
    float duration;

    private void Start()
    {
        startPos = transform.position;
        endPos = transform.position + Vector3.right * offset;
    }


    private void Update()
    {
        if (increase)
        {
            if (duration < 1)
            {
                duration += Time.deltaTime * speed * 0.01f;
                transform.position = Vector3.Slerp(startPos, endPos, duration);
            }
            else
            {
                increase = false;
            }
        }
        else
        {
            if (duration > 0)
            {
                duration -= Time.deltaTime * speed * 0.01f;
                transform.position = Vector3.Slerp(startPos, endPos, duration);
            }
            else
            {
                increase = true;
            }
        }
    }
}