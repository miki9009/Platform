using Engine;
using UnityEngine;

public class GoToScene : MonoBehaviour
{

    [CustomLevelSelector]
    public string customLevel;


    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody != null)
        {
            var character = other.attachedRigidbody.GetComponentInParent<Character>();
            if (character != null && character.IsLocalPlayer)
                GoToLevelAdditive();
        }
    }

    void GoToLevelAdditive()
    {
        LevelManager.ChangeLevel(LevelsConfig.GetSceneName(customLevel), LevelsConfig.GetLevelName(customLevel));
    }


}