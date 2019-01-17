using UnityEngine;
using Objectives;
using System.Collections.Generic;

public class ObjectivesPanelManager : MonoBehaviour
{
    public GameObject panelPrefab;
    static ObjectivesPanelManager instance;

    public static List<ObjectivePanel> panels = new List<ObjectivePanel>();

    Vector2 pos;

    private void Awake()
    {
        instance = this;
        GameManager.LevelClear += Restart;
        pos = GetComponent<RectTransform>().anchoredPosition;
    }

    private void Restart()
    {
        GetComponent<RectTransform>().anchoredPosition = pos;
        foreach (ObjectivePanel panel in panels)
        {
            if (panel.gameObject != null)
                Destroy(panel.gameObject);
        }
        panels.Clear();
    }

    public static void AddPanel(Objective objective)
    {
        if (instance == null) return;
        var obj = Instantiate(instance.panelPrefab, instance.transform);
        var panel = obj.GetComponent<ObjectivePanel>();
        panel.objective = objective;
        panel.title.text = objective.title;
        panel.progressBar.fillAmount = 0;
        panel.Initialize();
        panels.Add(panel);
    }

    private void OnDestroy()
    {
        GameManager.LevelClear -= Restart;
    }
}