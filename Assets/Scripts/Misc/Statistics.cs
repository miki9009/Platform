using Engine.UI;
using UnityEngine;
using UnityEngine.UI;

public class Statistics : MonoBehaviour
{
    public Text coins;
    public Text emmeralds;
    public Text goldKyes;
    public Text silverKeys;
    public Text bronzeKeys;
    public Text restarts;
    public UIWindow window;

    private void Awake()
    {
        window.BeginShow += AssignStats;
    }


    void AssignStats()
    {
        var data = DataManager.Collections;
        coins.text = "x" + data.coins;
        emmeralds.text = "x" + data.emmeralds;
        goldKyes.text = "x" + data.goldKeys;
        silverKeys.text = "x" + data.silverKeys;
        bronzeKeys.text = "x" + data.bronzeKeys;
        restarts.text = "x" + data.restarts;
    }
}