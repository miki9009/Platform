using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    public MonoBehaviour[] scripts;


    private void Start()
    {
        Invoke("DisableScripts", 0.1f);
    }

    void DisableScripts()
    {
        for (int i = 0; i < scripts.Length; i++)
        {
            scripts[i].enabled = false;
        }
    }
}
