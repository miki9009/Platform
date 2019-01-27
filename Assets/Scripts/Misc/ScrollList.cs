using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class ScrollList : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{

    public RectTransform content;
    public Transform snapCenter;
    public float elementSizeY;
    public ScrollRect scrollRect;

    public Button downArrow;
    public Button upArrow;
    //public UnityEngine.UI.Button downArrow;
    //public UnityEngine.UI.Button upArrow;
    public RectTransform viewPort;
    public float scrollSensitivity = 10;

    float multiplier = 1.25f;
    bool isDragging;
    public float lerpSpeed = 0.25f;
    private int index;
    public event System.Action<int> Snapped;
    public bool loop;
    public bool IsMoving
    {
        get
        {
            return Mathf.Abs(scrollRect.velocity.y) > 10;
        }
    }
    void OnEnable()
    {
        scrollRect.movementType = ScrollRect.MovementType.Unrestricted;
        scrollRect.inertia = true;
        scrollRect.decelerationRate = 0.01f;
        scrollRect.scrollSensitivity = scrollSensitivity;
        SetSnap();
    }

    public void SetIndex(int i)
    {
        content.anchoredPosition = new Vector2(content.anchoredPosition.x, elementSizeY * i);
        SetSnap();
    }

    Vector2 Snap()
    {
        Vector2 currentPos = content.anchoredPosition;
        int max = content.childCount - 1;
        float snappedPosition = Mathf.RoundToInt((currentPos.y / elementSizeY)) * elementSizeY;
        if (!loop)
        {
            if (snappedPosition < 0)
                snappedPosition = 0;
            else if (snappedPosition > (SizeY - ViewPortY))
                snappedPosition = (SizeY - ViewPortY);
        }
        else
        {
            if (snappedPosition < 0)
            {
                snappedPosition = 0;
            }

            else if (snappedPosition > SizeY - ViewPortY)
            {
                snappedPosition =0;
            }
        }
        return new Vector2(currentPos.x, snappedPosition);
    }

    public void ScrollDown()
    {
        snapPos = Snap();
        content.anchoredPosition = snapPos + new Vector3(0, elementSizeY + elementSizeY / 8);
        SetSnap();
    }

    public void ScrollUp()
    {
        snapPos = Snap();
        content.anchoredPosition = snapPos - new Vector3(0, elementSizeY + elementSizeY / 8);
        SetSnap();
    }

    private void Reset()
    {
        scrollRect = GetComponent<ScrollRect>();
    }

    bool canSnap = true;
    Vector3 snapPos;
    private void LateUpdate()
    {
        if ((canSnap && scrollRect && !isDragging && scrollRect.velocity.magnitude < 150))
        {
            scrollRect.StopMovement();
            snapPos = Snap();
            content.anchoredPosition = Vector2.Lerp(content.anchoredPosition, snapPos, lerpSpeed);
            float dis = Vector3.Distance(content.anchoredPosition, snapPos);
            if (canSnap && dis < 2)
            {
                content.anchoredPosition = snapPos;
                ButtonsManagement();
                canSnap = false;
            }
        }
        if(loop)
        {
            if (ContentY < elementSizeY)
            {
                //content.anchoredPosition += new Vector2(0, elementSizeY);
                scrollRect.content.anchoredPosition += new Vector2(0, elementSizeY);
                var child = content.GetChild(content.childCount - 1);
                child.SetAsFirstSibling();
            }

            else if (ContentY > SizeY - ViewPortY - elementSizeY)
            {
                scrollRect.content.anchoredPosition -= new Vector2(0, elementSizeY);
                var child = content.GetChild(0);
                child.SetAsLastSibling();

            }
        }


    }


    void SetSnap()
    {
        canSnap = true;
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        snapPos = Snap();
        SetSnap();
    }

    public float SizeY
    {
        get
        {
            return content.sizeDelta.y;
        }
    }

    public float ContentY
    {
        get
        {
            return content.anchoredPosition.y;
        }
    }

    public float ViewPortY
    {
        get
        {
            return viewPort.sizeDelta.y;
        }
    }

    void ButtonsManagement()
    {
        if (upArrow != null && downArrow != null)
        {
            if (!loop)
            {
                if (content.anchoredPosition.y < elementSizeY / 2)
                {
                    upArrow.interactable = false;
                }
                else
                {
                    upArrow.interactable = true;
                }
                if (viewPort)
                {
                    if (content.anchoredPosition.y + viewPort.sizeDelta.y > content.childCount * elementSizeY - elementSizeY * multiplier)
                    {
                        downArrow.interactable = false;
                    }
                    else
                    {
                        downArrow.interactable = true;
                    }
                }
            }

            if (snapCenter != null)
            {
                int children = content.childCount;
                float dis = Mathf.Infinity;
                float dis2;
                Vector3 pos = snapCenter.transform.position;
                Transform nearest = null;
                for (int i = 0; i < children; i++)
                {
                    var child = content.GetChild(i);
                    dis2 = Vector3.Distance(pos, child.position);
                    if (dis2 < dis)
                    {
                        dis = dis2;
                        nearest = child;
                    }
                }
                if (nearest != null)
                {
                    index = nearest.GetSiblingIndex();
                    Snapped?.Invoke(index);
                }

            }

        }
    }

}



