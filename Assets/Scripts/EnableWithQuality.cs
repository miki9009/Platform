using System.Collections.Generic;
using UnityEngine;

public class EnableWithQuality : MonoBehaviour
{
    public List<GameQualitySettings.GameQuality> modes;

    private void Start()
    {
        var mode = GameQualitySettings.CurrentMode;
        for (int i = 0; i < modes.Count; i++)
        {
            if(modes[i] == mode)
            {
                return;
            }
        }
        gameObject.SetActive(false);
    }
}