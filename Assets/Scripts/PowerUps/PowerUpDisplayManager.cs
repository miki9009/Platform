using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Engine;

public class PowerUpDisplayManager : MonoBehaviour
{
    public GameObject prefab;
    public HashSet<PowerUpDisplay> displays;

    public static PowerUpDisplayManager Instance
    {
        get; private set;
    }

    private void Awake()
    {
        Instance = this;
        displays = new HashSet<PowerUpDisplay>();
        Level.LevelLoaded += ClearAllPowerUpsAndDisplays;
    }

    public PowerUpDisplay GetDisplay(CollectionType type)
    {
        foreach(var d in displays)
        {
            if (d.type == type)
            {
                Debug.Log("Returned: " + type);
                return d;
            }
        }
        Debug.Log("No PowerUp display found with type: " + type);
        return null;
    }

    public void ResetDisplay(PowerUp pwr)
    {
        var display = GetDisplay(pwr.Type);
        if (display != null)
        {
            display.ResetTo(pwr);
        }
        else
        {
            AddDisplay(pwr);
        }
    }


    void ClearAllPowerUpsAndDisplays()
    {
        PowerUp.activePowerUps = new HashSet<PowerUp>();
        displays = new HashSet<PowerUpDisplay>();
    }

    private void OnDestroy()
    {
        Level.LevelLoaded -= ClearAllPowerUpsAndDisplays;
    }

    public void AddDisplay(PowerUp powerUp)
    {
        int time = powerUp.time;
        var type = powerUp.Type;
        if (time <= 0) return;
        PowerUpDisplay d = null;
        try
        {
            d = displays.SingleOrDefault(x => x.type == type);
        }
        catch { }
        if (d == null)
        {
            var obj = Instantiate(prefab, transform);
            var display = obj.GetComponent<PowerUpDisplay>();
            display.image.sprite = CollectionDisplayManager.Instance.collectionSprites[(int)type];
            display.time = time;
            display.powerUp = powerUp;
            display.ConvertTime();
            display.type = type;
            displays.Add(display);
        }
        else
        {
            d.time = time;
            d.ConvertTime();
            d.powerUp = powerUp;
        }
    }

    public void RemoveDisplay(CollectionType type)
    {
        var display = displays.SingleOrDefault(x => x.type == type);
        if (display != null)
        {
            Destroy(display.gameObject);
        }
    }
}