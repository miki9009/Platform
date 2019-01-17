using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using UnityEngine;


[SerializableAttribute]
public class LevelSelector : PopUpAttribute
{
    public LevelSelector()
    {
        items = LevelManager.Scenes;
    }
}
