using System;
using UnityEngine;
using Engine;
using Engine.Config;
using System.Collections.Generic;

[SerializableAttribute]
public class SpawnsCollectionSelector : PopUpAttribute
{
    public SpawnsCollectionSelector()
    {
        items = new string[4];
        items[0] = SpawnsCollectionType.COLLECTOIN;
        items[1] = SpawnsCollectionType.ENEMY;
        items[2] = SpawnsCollectionType.OBSTACLE;
        items[3] = SpawnsCollectionType.POWER_UP;
    }
}
