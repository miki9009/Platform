using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class LevelGameplayElements : MonoBehaviour
{
    public GameObject gui;

    private void Awake()
    {
        Instantiate(gui);
    }
}