using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(Materials))]
public class MaterialsEditor : Editor
{
    delegate bool Contains(Material mat);
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Get All materials on scene"))
        {
            List<string> materials = new List<string>();
            Contains method = (x) =>
            {
                for (int i = 0; i < materials.Count; i++)
                {
                    if (materials[i] == x.name)
                        return true;
                }
                return false;
            };
            var renderers = GameObject.FindObjectsOfType<MeshRenderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i].sharedMaterials.Length > 1)
                {
                    Debug.Log(renderers[i].name + "has more than one material!");
                }
                if(!method(renderers[i].sharedMaterial))
                {
                    Debug.Log(renderers[i].transform.name + " has a material: " + renderers[i].sharedMaterial.name);
                    materials.Add(renderers[i].sharedMaterial.name);
                }

            }
        }

    }

}



