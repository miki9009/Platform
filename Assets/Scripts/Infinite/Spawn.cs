using UnityEngine;

public class Spawn : MonoBehaviour
{
    public string SpawnName { get; set; }
    public IPoolObject SpawnInstance { get; set; }

    bool firstDisable = true;

    private void OnDisable()
    {
        if (firstDisable)
        {
            firstDisable = false;
            return;
        }

        Debug.Log("Recycled");
        SpawnManager.Recycle(SpawnInstance);
    }
}