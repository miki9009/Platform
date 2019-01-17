using UnityEngine;

[ExecuteInEditMode]
public class FastWater : MonoBehaviour
{
    MeshRenderer meshRenderer;
    Material mat;
    [Range(1f,2f)]
    public float tiling;
    [Range(0.01f, 1f)]
    public float speed = 0.1f;
    
    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        tiling += 0.01f * speed;
        meshRenderer.sharedMaterial.SetTextureOffset("_BumpMap", new Vector2(tiling, tiling));
    }
}