using UnityEngine;

public class JetpackCollection : MonoBehaviour
{
    public GameObject jetpackPrefab;

    private void Awake()
    {
        var collection = GetComponent<Collection>();
        collection.Collected += Collection_Collected;
    }

    private void Collection_Collected(GameObject collector)
    {
        Debug.Log(collector.name);
        var character = collector.GetComponentInParent<Character>();
        var jetpack = Instantiate(jetpackPrefab).GetComponent<Jetpack>();
        jetpack.SetCharacter(character);
    }
}