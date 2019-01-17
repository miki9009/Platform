﻿using Engine;
using UnityEngine;

public class CharacterInstantiator : LevelElement
{
    public GameObject prefab;

    public override void OnSave()
    {
        base.OnSave();
        if (data != null)
        {
#if UNITY_EDITOR
            string path = UnityEditor.AssetDatabase.GetAssetPath(prefab);
            if (!string.IsNullOrEmpty(path))
            {
                //Debug.Log(path.Length);
                path = path.Substring(17, path.Length - 24);
                Debug.Log(path);
                data.Add("Path", path);
            }
#endif
        }
    }

    public override void OnLoad()
    {
        base.OnLoad();
        if (data != null)
        {
            if (data.ContainsKey("Path"))
            {
                string path = (string)data["Path"];
                prefab = (GameObject)Resources.Load(path);
                if (Application.isPlaying)
                {
                    InstantiateCharacter();
                }
            }
        }
    }

#if UNITY_EDITOR
    public Color color = Color.blue;
    private void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawSphere(transform.position, 1);
    }
#endif

    private void InstantiateCharacter()
    {
        var obj = Instantiate(prefab, transform.position, transform.rotation);
    }

}