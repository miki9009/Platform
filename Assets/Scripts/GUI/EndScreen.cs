using Engine.UI;
using UnityEngine;

public class EndScreen : UIWindow
{
    public override void OnShown()
    {
        Debug.Log("Mission Index: " + GameManager.MissionIndex);
        if (MissionConfigHandler.CurrentMissionIndex <= GameManager.MissionIndex)
            MissionConfigHandler.CurrentMissionIndex = GameManager.MissionIndex + 1;
    }
}