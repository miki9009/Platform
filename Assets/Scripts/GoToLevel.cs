using Engine;
using UnityEngine;

public class GoToLevel : LevelElement
{

    [CustomLevelSelector]
    public string customLevel;


    private void OnTriggerEnter(Collider other)
    {
        if(other.attachedRigidbody != null)
        {
            var character = other.attachedRigidbody.GetComponentInParent<Character>();
            if(character!=null && character.IsLocalPlayer)
                GoToLevelAdditive();
        }
    }

    void GoToLevelAdditive()
    {
        LevelManager.ChangeLevel(LevelsConfig.GetSceneName(customLevel), LevelsConfig.GetLevelName(customLevel));
    }

    public override void OnLoad()
    {
        base.OnLoad();
        if(data!=null)
        {
            if (data.ContainsKey("CustomLevel"))
                customLevel = (string)data["CustomLevel"];
        }
    }

    public override void OnSave()
    {
        base.OnSave();
        if(data!=null)
        {
            data["CustomLevel"] = customLevel;
        }
    }

}