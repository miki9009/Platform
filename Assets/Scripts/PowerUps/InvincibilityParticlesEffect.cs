using UnityEngine;

public class InvincibilityParticlesEffect : MonoBehaviour
{
    public Transform lightAnchor;
    public float maxY = 3;
    public float minY = 0;
    public float minX = 1;
    public float maxX = 3;
    public float rotationSpeed = 1;
    public float changeHorizontalFactor = 0.1f;
    public float changeVerticalFactor = 0.1f;

    float x = 0;
    float y = 0;

    bool up = false;
    bool right = false;

    Vector3 rotationVector = new Vector3(0, 1, 0);

    private void Update()
    {
        transform.Rotate(rotationVector * rotationSpeed);
        if (up)
        {
            if (y < maxY)
                y += changeVerticalFactor;
            else
                up = false;
        }
        else
        {
            if (y > minY)
                y -= changeVerticalFactor;
            else
                up = true;
        }
        if (right)
        {
            if (x < maxX)
                x += changeHorizontalFactor;
            else
                right = false;
        }
        else
        {
            if (x > minX)
                x -= changeHorizontalFactor;
            else
                right = true;
        }
        lightAnchor.localPosition = new Vector3(x, y, 0);
    }
}