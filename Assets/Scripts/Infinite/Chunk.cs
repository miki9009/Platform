using UnityEngine;

public class Chunk : MonoBehaviour
{
    public Vector3 size;
    public Vector3 center;

    public float sizeX;

    private MeshRenderer mainMeshRenderer;

    public Vector3 currentPosition;

    public SpawningPlace spawningPlace;

    private void Awake()
    {
        currentPosition = transform.position;
    }

    private void Reset()
    {
        SetSize();
    }

    public void PlaceNextTo(Chunk chunk)
    {
        transform.position = chunk.currentPosition + Vector3.right * chunk.sizeX/2 + sizeX / 2 * Vector3.right;
        currentPosition = transform.position;
    }

    public void PlaceBefore(Chunk chunk)
    {
        transform.position = chunk.currentPosition - Vector3.right * chunk.sizeX / 2 - sizeX / 2 * Vector3.right;
        currentPosition = transform.position;
    }

    public void SetSize()
    {
        mainMeshRenderer = GetComponentInChildren<MeshRenderer>();
        if(mainMeshRenderer != null)
        {
            size = GetComponentInChildren<MeshRenderer>().bounds.size;
            sizeX = size.x;
            currentPosition = transform.position;
            center = currentPosition;
        }
        else
        {
            Debug.Log("Mesh not found");
        }

    }

#if UNITY_EDITOR
    public bool drawBouds = false;
    public Color gizmosColor = Color.gray;

    private void OnDrawGizmos()
    {
        if (drawBouds)
        {
            Gizmos.color = gizmosColor;
            Gizmos.DrawCube(center, size);
        }
    }

#endif

}