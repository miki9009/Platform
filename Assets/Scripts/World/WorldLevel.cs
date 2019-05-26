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
        WorldPointer.Click += WorldPointer_Click;
    }

    private void OnDestroy()
    {
        WorldPointer.Click -= WorldPointer_Click;
    }

    private void WorldPointer_Click(Transform t)
    {
        if(t == transform)
        {

        }
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
        LevelManager.BeginCustomLevelLoadSequenceAdditive(LevelsConfig.GetSceneName(customLevel), LevelsConfig.GetSceneName(customLevel));
    }


    //void BeginLoading(Scene scene)
    //{
    //    //SceneManager.sceneUnloaded -= BeginLoading;
    //    LevelManager.BeginCustomLevelLoadSequenceAdditive(LevelsConfig.GetSceneName(customLevel), LevelsConfig.GetLevelName(customLevel));
    //}
}