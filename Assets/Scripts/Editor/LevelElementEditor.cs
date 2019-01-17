using Engine;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelElement), true)]
[CanEditMultipleObjects]
public class LevelElementZEditor : Editor
{
    public void OnEnable()
    {
        var instance = (LevelElement)target;
        if(instance.elementID == -1)
        {
            instance.elementID =Level.GetID();
        }
    }
}