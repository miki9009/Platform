using UnityEngine;

public class GameObjectEnabler : MonoBehaviour
{
    public GameObjectContainer[] gameObjects;

    private void OnEnable()
    {
        Console.WriteLine("On Enable", Console.LogColor.Lime);
        for (int i = 0; i < gameObjects.Length; i++)
        {
            gameObjects[i].OnEnable();
        }    
    }

    private void OnDisable()
    {
        Console.WriteLine("On Disable", Console.LogColor.Lime);
        for (int i = 0; i < gameObjects.Length; i++)
        {
            gameObjects[i].OnDisable();
        }
    }

    public enum Activity
    {
        None, Activate, Deactivate
    }

    [System.Serializable]
    public class GameObjectContainer
    {
        public GameObject gameObject;
        public Activity onEnable;
        public Activity onDisable;

        public void OnEnable()
        {
            if(onEnable != Activity.None)
            {
                if (onEnable == Activity.Deactivate)
                    gameObject.SetActive(false);
                else
                    gameObject.SetActive(true);
            }    
        }

        public void OnDisable()
        {
            if (onDisable != Activity.None)
            {
                if (onDisable == Activity.Deactivate)
                    gameObject.SetActive(false);
                else
                    gameObject.SetActive(true);
            }
        }
    }
}