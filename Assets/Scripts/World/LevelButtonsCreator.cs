using System.Collections.Generic;
using UnityEngine;

public class LevelButtonsCreator : MonoBehaviour
{
    public List<LevelButton> buttons;
    public static LevelButtonsCreator instance;

    private void Awake()
    {
        instance = this;
    }

    public static void PrepareButtons(List<Mission> missions)
    {
        var buttons = instance.buttons;

        if(missions.Count > buttons.Count)
        {
            Debug.LogError("More missions than buttons");
            return;
        }
        int i = 0;

        foreach (var button in buttons)
        {
            if(i < missions.Count)
            {
                button.gameObject.SetActive(true);
                button.mission = missions[i];
            }
            else
            {
                button.gameObject.SetActive(false);
            }
            i++;
        }
    }
}