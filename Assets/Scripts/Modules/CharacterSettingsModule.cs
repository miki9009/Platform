using Engine;
using Engine.Config;
using Engine.Singletons;
using System;
using UnityEngine;

public class CharacterSettingsModule : Module
{

    static DataProperty<CharacterStatistics> localCharacter;

    public static CharacterStatistics Statistics
    {
        set
        {
            localCharacter.Value = value;
        }

        get
        {
            return localCharacter.Value;
        }
    }


    public override void Initialize()
    {
        var config = ConfigsManager.GetConfig<CharacterConfig>();
        var defaultCharacter = config.defaultCharacterStatistics;
        localCharacter = DataProperty<CharacterStatistics>.Get("Character_Settings", defaultCharacter);
    }
}

