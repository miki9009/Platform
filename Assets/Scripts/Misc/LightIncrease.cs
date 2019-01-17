using UnityEngine;

public class LightIncrease : MonoBehaviour
{
    public float increaseAmmont = 1;
    public float strength = 1;
    private void Start()
    {
        var light = GetComponent<Light>();
        light.range *= increaseAmmont;
        light.intensity *= strength;
    }
}