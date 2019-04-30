using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Engine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SceneryElementSelector : PopUpAttribute
{
    public SceneryElementSelector()
    {
        var objs = Resources.FindObjectsOfTypeAll<GameObject>();
        var paths = new List<string>();
#if UNITY_EDITOR
        foreach (var obj in objs)
        {
            string path = AssetDatabase.GetAssetPath(PrefabUtility.GetCorrespondingObjectFromSource(obj));
            if (!string.IsNullOrEmpty(path) && path.Length > 17)
            {
                path = path.Substring(17);
                if (path.Length > 7)
                    paths.Add(path.Substring(0, path.Length - 7));
            }
        }
#endif
        items = paths.ToArray();
    }
}
