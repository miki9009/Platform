using UnityEngine;

public class DeathTrigger : MonoBehaviour
{

    Character character;
    private void OnTriggerEnter(Collider other)
    {
        character = other.GetComponentInParent<Character>();
        if (character != null)
        {
            Controller.Instance.gameCamera.GetComponent<GameCamera>().SetTarget(null);
            character.enabled = false;
            Invoke("InvokeMe", 2);
        }

        
    }

    void InvokeMe()
    {
        if (character != null && Controller.Instance.IsRestarting) return;
        character.movement.DieNonAnimation();
        character.movement.characterHealth.RemoveHealth(character.stats.health);
    }
}