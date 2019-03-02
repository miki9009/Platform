using LPWAsset;
using UnityEngine;

public class LowPolyWaterRenderer : MonoBehaviour
{
    public Renderer rend;
    LowPolyWaterScript lowPolyWater;

    private void Awake()
    {
        lowPolyWater = GetComponentInParent<LowPolyWaterScript>();
    }

    private void OnBecameVisible()
    {
        lowPolyWater.IsVisible = true;
    }

    private void OnBecameInvisible()
    {
        lowPolyWater.IsVisible = false;
    }
}