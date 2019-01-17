using Engine.Config;
using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(menuName = key)]
public class CharacterConfig : Config
{
    public const string key = "Configs/CharacterConfig";

    public CharacterStatistics defaultCharacterStatistics;
    public List<DefinedCharacter> characters;
    
    public Mesh GetMesh(string id)
    {
        for (int i = 0; i < characters.Count; i++)
        {
            if (characters[i].id == id)
                return characters[i].mesh;
        }
        Debug.LogError("Mesh with id: " + id + " was not found");
        return null;
    }
   
}

[Serializable]
public class DefinedCharacter
{
    public string id;
    public Mesh mesh;
}
