using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(WeaponManager))]
public class WeaponManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var script = (WeaponManager)target;

        var weapons = script.weapons.Select(x => x.weaponName).ToArray();

        script.weaponIndex = EditorGUILayout.Popup("Weapon", script.weaponIndex, weapons);

        base.DrawDefaultInspector();
    }
}
