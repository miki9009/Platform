using UnityEngine;
using UnityEngine.UI;

public class TouchImage : MonoBehaviour
{
    public static TouchImage leftImage;
    public static TouchImage rightImage;

    Image img;
    RectTransform rect;

    public int index;
    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        if(index == 0)
        {
            leftImage = this;
        }
        else
        {
            rightImage = this;
        }
        img = GetComponent<Image>();
    }

    public void SetTouch(float alpha, Vector2 pos)
    {
        rect.anchoredPosition = pos;
        img.color = new Color(1, 1, 1, alpha);
    }

    private void OnDestroy()
    {
        leftImage = null;
        rightImage = null;
    }
}