using System.Collections.Generic;
using UnityEngine;

public class MapFocused : MonoBehaviour
{
    public Color missionOnColorMax = Color.red;
    public Color missionOnColorMin = Color.red;
    public Material missionOnMaterial;
    public Material missionOffMaterial;
    public Material missionPassedMaterial;
    public List<WorldLevel> levels;

    float time = 0;
    bool up;

    public float lightAnimationSpeed = 5;



    private void Update()
    {

        ManageMaterial();
    }

    void ManageMaterial()
    {
        if (up)
        {
            if (time < 1)
            {
                time += Time.deltaTime * lightAnimationSpeed;
            }
            else
            {
                up = false;
            }
        }
        else
        {
            if (time > 0)
            {
                time -= Time.deltaTime * lightAnimationSpeed;
            }
            else
            {
                up = true;
            }
        }
        missionOnMaterial.color = Color.Lerp(missionOnColorMin, missionOnColorMax, time);
        for (int i = 0; i < levels.Count; i++)
        {
            if (levels[i].enabled)
                levels[i].lght.intensity = time;
        }
    }
}