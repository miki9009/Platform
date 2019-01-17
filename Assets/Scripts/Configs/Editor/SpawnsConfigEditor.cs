using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SpawnsConfig))]
public class SpawnsConfigEditor : Editor
{
    SpawnsConfig script;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        script = (SpawnsConfig)target;
        if (script.spawnCollections == null || script.spawnCollections.Count == 0) return;
        float chances = 0;
        for (int i = 0; i < script.spawnCollections.Count; i++)
        {
            chances += script.spawnCollections[i].chances;
        }
        if (chances > 100)
        {
            Debug.LogError("Chances for Spawns Collections are equal to " + chances);
        }

        for (int i = 0; i < script.spawnCollections.Count; i++)
        {
            var spawnSettings = script.spawnCollections[i].spawnSettings;
            chances = 0;
            for (int j = 0; j < spawnSettings.Count; j++)
            {
                chances++;
            }
            if (chances > 100)
            {
                Debug.LogError("Chances equal " + chances + " for settings " + script.spawnCollections[i].collectionName);
            }
        }

    }
}