using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorUtilities : MonoBehaviour
{
    public bool randomRotation = true;


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GetAllActiveMaterials()
    {
        var materials = GameObject.FindObjectsOfType<Material>();
        print("Materials " + Resources.FindObjectsOfTypeAll(typeof(Material)).Length);
        foreach (Material mat in materials)
        {
            Debug.Log(mat.name);
        }
    }

}