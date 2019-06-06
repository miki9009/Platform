using System.Collections;
using System.Collections.Generic;
using Engine;
using UnityEngine;

public class GameDataModule : Engine.Singletons.Module
{


    static DataProperty<int> _coins;
    static DataProperty<int> _gems;

    public static int Coins
    {
        get
        {
            return _coins.Value;
        }

        private set
        {
            _coins.Value = value;
        }
    }

    public static int Gems
    {
        get
        {
            return _gems.Value;
        }

        private set
        {
            _gems.Value = value;
        }
    }

    public static void AddCoins(int val)
    {
        int cur = Coins;
        Coins = cur + val;
    }

    public static void AddGems(int val)
    {
        int gems = Gems;
        Gems = gems + val;
    }

    public override void Initialize()
    {
        var config = ConfigsManager.GetConfig<CharacterConfig>();
        var defaultCharacter = config.defaultCharacterStatistics;
        _coins = DataProperty<int>.Get("GameData_Coins", 0);
        _gems = DataProperty<int>.Get("GameData_Gems", 0);
    }

}