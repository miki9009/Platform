using System.Collections.Generic;
using UnityEngine;

public class CharacterProgressPanelManager : MonoBehaviour
{
    public static CharacterProgressPanelManager Instance { get; private set; }
    public GameObject panelPrefab;
    public Transform panelsAnchor;
    public RectTransform[] anchors;

    List<CharacterProgressPanel> panels;

    private void Awake()
    {
        Instance = this;
    }

    public void Initialize(List<Character> characters)
    {
        if(panels!=null)
        {
            foreach (var panel in panels)
            {
                if (panel != null)
                    Destroy(panel.gameObject);
            }
            panels.Clear();
        }
        panels = new List<CharacterProgressPanel>();
        int i = 0;
        foreach (var character in characters)
        {
            var panel = Instantiate(panelPrefab, panelsAnchor).GetComponent<CharacterProgressPanel>();
            panel.Initialize(character);
            panel.rectTransform.anchoredPosition = anchors[i].anchoredPosition;
            panels.Add(panel);
            i++;
        }
    }

    public void RefreshPositions()
    {
        foreach (var panel in panels)
        {
            panel.rectTransform.anchoredPosition = anchors[panel.Character.gameProgress.Placement].anchoredPosition;
        }
    }
}