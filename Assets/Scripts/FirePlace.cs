using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Engine;
using Engine.UI;

public class FirePlace : LevelElement
{
    bool visited = false;
    private void OnTriggerEnter(Collider other)
    {
        if(!visited && other.gameObject.layer == Layers.Character)
        {
            var character = other.GetComponent<Character>();
            visited = true;
            CollectionManager.Instance.SetCollection(character.ID, CollectionType.FirePlaceReached, 1);
        }
    }
}