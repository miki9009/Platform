using UnityEngine;
using UnityEngine.SceneManagement;

[ExecuteInEditMode]
public class Significant : MonoBehaviour
{
    private static int Index
    {
        get
        {
            Significant[] significants = FindObjectsOfType<Significant>();
            return significants.Length;
        }
    }
    public int keyLength = 15;
    public string propertyKey;

    private void Reset()
    {
        propertyKey = SceneManager.GetActiveScene().name + "_"+Index;       
    }
}

