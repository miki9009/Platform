using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CharacterHealth : MonoBehaviour
{

    public GameObject[] hearts;

    CharacterStatistics stats;

    private void Awake()
    {
        Character.CharacterCreated += AssignHealth;
    }

    void AssignHealth(Character character)
    {
        if (!character.IsLocalPlayer) return;
        Debug.Log("Controller: " + Controller.Instance);
        try
        {
            stats = Controller.Instance.character.stats;
            Controller.Instance.character.movement.characterHealth = this;
            CreateHealthFromStats(0, true);
        }
        catch
        {
            enabled = false;
        }

    }

    public void CreateHealthFromStats(int startIndex, bool active)
    {
        for (int i = startIndex; i < stats.health; i++)
        {
            hearts[i].SetActive(active);
            Effects.ScalePulse(hearts[i].transform, 2, 2, ()=>hearts[i].transform.localScale = Vector3.one);
        }
    }

    public void AddHealth(int health)
    {
        hearts[health-1].SetActive(true);
        Effects.ScalePulse(hearts[health-1].transform, 2, 2, () => hearts[health-1].transform.localScale = Vector3.one);
    }

    public void RemoveHealth(int newHealth)
    {
        Effects.ScalePulseVanish(hearts[newHealth].transform, 2, 2, () => hearts[newHealth].SetActive(false));
    }

    private void OnDestroy()
    {
        Character.CharacterCreated -= AssignHealth;
    }

}