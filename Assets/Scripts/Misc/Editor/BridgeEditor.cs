using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Bridge))]
public class BridgeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        //var script = (Bridge)target;
        //if (!script.gameObject.activeSelf && !Application.isPlaying)
        //    script.gameObject.SetActive(true);
    }
}
