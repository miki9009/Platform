using Engine;
using UnityEngine;

public class VisibilityHandler : MonoBehaviour
{
    public LevelElement levelElement;

    private void OnBecameInvisible()
    {
        if(levelElement)
        {
            levelElement.IsInvisible();
        }
        else
        {
            Debug.LogError("No LevelElement Assigned on: " + transform.name);
        }
    }

    private void OnBecameVisible()
    {
        if (levelElement)
        {
            levelElement.IsVisible();
        }
        else
        {
            Debug.LogError("No LevelElement Assigned on: " + transform.name);
        }
    }
}