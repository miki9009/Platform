using UnityEngine;

public class MovingStone : MonoBehaviour
{
    public BezierCurve curve;
    public float speed = 0.25f;

    float movement = 0;
    bool forward;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == Layers.Character)
        {
            collision.gameObject.GetComponent<CharacterMovement>().Hit(null,100, true);
        }
    }

    private void Update()
    {
        if (forward)
        {
            if (movement < 0.5f)
            {
                speed += 0.015f;
            }
            else
            {
                speed -= 0.01f;
            }
            movement += Time.deltaTime * speed;
            if (movement >= 1)
            {
                movement = 0.99f;
                forward = false;
                speed = 0;
            }
        }
        else
        {
            if (movement > 0.5f)
            {
                speed += 0.015f;
            }
            else
            {
                speed -= 0.01f;
            }
            movement -= Time.deltaTime * speed;
            if (movement < 0)
            {
                movement = 0.01f;
                forward = true;
                speed = 0;
            }
        }

        transform.position = curve.GetPointAt(movement);
        transform.Rotate(0, 0, (forward ? speed * 5 : -speed * 5));
    }
}