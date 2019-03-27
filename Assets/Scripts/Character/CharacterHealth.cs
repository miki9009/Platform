using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CharacterHealth : MonoBehaviour
{
    public Image progressBar;


    CharacterStatistics stats;

    private void Awake()
    {
        Character.CharacterCreated += AssignHealth;
        CharacterStatistics.HealthChanged += SetHealth;
    }

    float startHealth;

    void AssignHealth(Character character)
    {
        if (!character.IsLocalPlayer) return;
        startHealth = character.Health;

        try
        {
            stats = Controller.Instance.character.stats;
            Controller.Instance.character.movement.characterHealth = this;
            SetHealth(character, (int)startHealth);
        }
        catch
        {
            enabled = false;
        }

    }

    public void SetHealth(Character character, int newHealth)
    {
        if(character.IsLocalPlayer)
            progressBar.fillAmount = newHealth / startHealth;
    }

    private void OnDestroy()
    {
        Character.CharacterCreated -= AssignHealth;
        CharacterStatistics.HealthChanged -= SetHealth;
    }

}