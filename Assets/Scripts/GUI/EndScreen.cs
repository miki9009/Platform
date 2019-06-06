using Engine.UI;
using UnityEngine;

public class EndScreen : UIWindow
{
    public override void OnShown()
    {
        Debug.Log("Mission Index: " + GameManager.MissionIndex);
        if (MissionConfigHandler.CurrentMissionIndex <= GameManager.MissionIndex)
            MissionConfigHandler.CurrentMissionIndex = GameManager.MissionIndex + 1;

        SaveCollections();
    }

    void SaveCollections()
    {
        int localID = Character.GetLocalPlayer().ID;
        var collectionManager = CollectionManager.Instance;
        int coins = collectionManager.GetCollection(localID, CollectionType.Coin);
        int gems = collectionManager.GetCollection(localID, CollectionType.Emmerald);

        GameDataModule.AddCoins(coins);
        GameDataModule.AddGems(gems);

        Debug.Log("Coins is: " + GameDataModule.Coins);
        Debug.Log("Gems is: " + GameDataModule.Gems);

    }
}