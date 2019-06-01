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
    bool visited;
    public bool endLevelOnReached;

    private void OnTriggerEnter(Collider other)
    {
        if(!visited && other.gameObject.layer == Layers.Character && endLevelOnReached)
        {
            var character = other.GetComponent<Character>();
            visited = true;
            CollectionManager.Instance.SetCollection(character.ID, CollectionType.FirePlaceReached, 1);
            var window = UIWindow.GetWindow(UIWindow.END_SCREEN);
            window.Show();
            Pause.Instance.PauseEnter();
        }
    }

    public override void OnSave()
    {
        base.OnSave();
        data["EndLevelOnReached"] = endLevelOnReached;
    }

    public override void OnLoad()
    {
        base.OnLoad();
        if(data.ContainsKey("EndLevelOnReached"))
        {
            endLevelOnReached = (bool)data["EndLevelOnReached"];
        }
    }
}