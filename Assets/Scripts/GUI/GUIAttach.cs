using UnityEngine;

namespace Engine.UI
{
    public class GUIAttach : MonoBehaviour
    {

        void Start()
        {
            var rect = GetComponent<RectTransform>();
            transform.SetParent(GameGUI.Instance.transform);
            rect.anchoredPosition = Vector2.zero;
            rect.localScale = Vector3.one;
        }
    }
}