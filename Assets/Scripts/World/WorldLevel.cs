using Engine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldLevel : MonoBehaviour
{
    [CustomLevelSelector]
    public string customLevel;
    public Light lght;
    public SphereCollider sphereCollider;

    private void Awake()
    {
        GetComponentInParent<MapFocused>().levels.Add(this);
    }

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


    //void BeginLoading(Scene scene)
    //{
    //    //SceneManager.sceneUnloaded -= BeginLoading;
    //    LevelManager.BeginCustomLevelLoadSequenceAdditive(LevelsConfig.GetSceneName(customLevel), LevelsConfig.GetLevelName(customLevel));
    //}
}