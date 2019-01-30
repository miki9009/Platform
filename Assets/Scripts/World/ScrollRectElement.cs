using UnityEngine;
using UnityEngine.UI;

public class ScrollRectElement : MonoBehaviour
{
    public Image outline;
    public Image background;
    public GameObject additionalInfo;
    public Text text;
    bool selected;
    public event System.Action Selected;

    void Awake()
    {
        IsSelected = false;
    }
    public bool IsSelected
    {
        get
        {
            return selected;
        }

        set
        {
            selected = value;
            outline.enabled = value;
            additionalInfo.SetActive(value);
            background.color = value ? new Color(1, 1, 1, 1) : new Color(1, 1, 1, 0.25f);
            Selected?.Invoke();
        }
    }
}

