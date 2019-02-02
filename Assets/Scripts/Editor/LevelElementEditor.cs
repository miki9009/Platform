using Engine;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelElement), true)]
[CanEditMultipleObjects]
public class LevelElementEditor : Editor
{
    public void OnEnable()
    {
        if (Application.isPlaying) return;
        var instance = (LevelElement)target;
        if(instance.elementID == -1)
        {
            instance.elementID =Level.GetID();
        }
    }
}