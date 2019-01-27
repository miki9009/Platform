using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(ScrollRect))]
public class ScrollRectSnap : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
	/// <summary>
	/// Position to snap
	/// </summary>
	public RectTransform snapCenter;
	/// <summary>
	/// Speed of addjusting position to snaped element
	/// </summary>
	public float snapTime = 5f;
	/// <summary>
	/// Number of elements per page, used to scrollig to next/prev pages
	/// </summary>
	public int elementsPerPage = 1;
	/// <summary>
	/// Use fast swite to scrol to next page
	/// </summary>
	public bool useFastSwipe = true;
	/// <summary>
	/// Enable going to first element after last and reverse
	/// </summary>
	public bool loopList = false;
	public int visibleElements = 9;
	public bool carousel = false;
	public bool snapToElement = true;
	public bool customInteria = false;
	public float customInteriaDecelerationRate = 0.1f;
    public bool isGarageItemsHolder;
    public int lastIndex = 10;
    public event System.Action<int> Snapped;

	public RectTransform CurrentElement
	{
		get
		{
			return _insideElements[_snapIndex];
		}
	}

    public Dictionary<RectTransform, ScrollRectElement> garageItems;

	public List<RectTransform> Elements { get { return _elements; } }

	public Button prevButton;
	public Button nextButton;

	public event System.Action<RectTransform> snapedElement;
	public event System.Action<RectTransform> addedElement;
	public event System.Action<RectTransform> removedElement;
	public event System.Action<RectTransform> updateElement;

    protected List<RectTransform> _elements;
    protected List<RectTransform> _insideElements;

	private ScrollRect _scrollRect;
	private bool _dragging;
	private int _snapIndex;

	private int _lockedIndex;
	private bool _hardLock;

    public bool _initialized;
    public bool initializeOnAwake = true;

    public ScrollRect ScrollRect
    {
        get
        {
            return _scrollRect;
        }
    }

    protected virtual void Awake()
	{
        if(initializeOnAwake)
		    Initialize();
	}

	public virtual void Initialize()
	{
		if (_initialized) return;

		_initialized = true;

		_scrollRect = GetComponent<ScrollRect>();
		RebuildSnap();

		if (customInteria) return;

		if (carousel)
		{
			_lock = true;
			for (var i = _snapIndex; i < _lockedIndex; i++)
			{
				_insideElements[_insideElements.Count - 1].SetAsFirstSibling();
				RebuildInsideList();
			}
			_snapIndex = _lockedIndex;
		}
		if (_elements.Count > 0)
		{
			_lastSnapedElement = _insideElements[_snapIndex];
			LerpToElement(_insideElements[_snapIndex], true);
		}


    }

    protected virtual void OnGarageEntered()
    {

    }

    protected virtual void OnDestroy()
    {
        //Garage.GarageManager.GarageEntered -= OnGarageEntered;
        //CarSequenceUI.CurrentChanged -= CurrentSequenceSlotChanged;
    }
	/// <summary>
	/// Rebuilds snap data after adding / removing elements form list
	/// </summary>
	public void RebuildSnap()
	{
        if (!_initialized) return;
		if (_scrollRect.content != null)
		{
			if (_elements == null) _elements = new List<RectTransform>();
			else _elements.Clear();

			for (var i = 0; i < _scrollRect.content.childCount; i++)
			{
				if (_scrollRect.content.GetChild(i).gameObject.activeSelf)
					_elements.Add((RectTransform)_scrollRect.content.GetChild(i));
			}
            if (isGarageItemsHolder)
            {
                garageItems = new Dictionary<RectTransform, ScrollRectElement>();
                for (int i = 0; i < _elements.Count; i++)
                {
                    ScrollRectElement item = _elements[i].GetComponent<ScrollRectElement>();
                    if (item != null)
                    {
                        garageItems.Add(_elements[i], item);
                    }
                }
            }

            RebuildInsideList();

			_lockedIndex = Mathf.CeilToInt(visibleElements / 2f);

			_scrollRect.enabled = _elements.Count >= 1;
		}
	}

	private void RebuildInsideList()
	{
		if (_insideElements == null) _insideElements = new List<RectTransform>();
		else _insideElements.Clear();

		for (var i = 0; i < _scrollRect.content.childCount; i++)
		{
			if (_scrollRect.content.GetChild(i).gameObject.activeSelf)
				_insideElements.Add((RectTransform)_scrollRect.content.GetChild(i));
		}
	}

	private void OnDisable()
	{
		_velocity = Vector2.zero;
	}

	bool _lock = false;
	bool moved = false;
	private void Update()
	{
		if (_scrollRect.content == null || snapCenter == null || _insideElements.Count == 0) return;

		if (carousel)
		{
			UpdateCarousel();
			RebuildInsideList();
		}

		if (!_dragging && snapToElement && !_lock && ((Mathf.Abs(_velocity.x) < 100f && _scrollRect.horizontal) || (Mathf.Abs(_velocity.y) < 100f && _scrollRect.vertical)))
		{
			SetCurrentElementAndMove(_elements.IndexOf(GetClosestRect()));
		}

		if ((!_dragging && _lock && !_hardLock) || _insideElements.Count == 1)
		{
			LerpToElement(_insideElements[_snapIndex]);
		}
	}

	private void UpdateCarousel()
	{
		var closest = GetClosestRect().GetSiblingIndex();

		while (closest > _lockedIndex)
		{
			var dist = ((RectTransform)_scrollRect.content.GetChild(1)).anchoredPosition
				- ((RectTransform)_scrollRect.content.GetChild(0)).anchoredPosition;
			_scrollRect.content.GetChild(0).SetAsLastSibling();
			closest--;
			_scrollRect.content.anchoredPosition += dist;
			if (_dragging) _scrollRect.OnBeginDrag(_lastDrag);
			_prevPosition = _scrollRect.content.anchoredPosition;
			moved = true;
			_snapIndex--;
			if (_snapIndex < 0)
				_snapIndex = _insideElements.Count - 1;
		}
		while (closest < _lockedIndex)
		{
			var dist = ((RectTransform)_scrollRect.content.GetChild(_scrollRect.content.childCount - 2)).anchoredPosition
				- ((RectTransform)_scrollRect.content.GetChild(_scrollRect.content.childCount - 1)).anchoredPosition;
			_scrollRect.content.GetChild(_scrollRect.content.childCount - 1).SetAsFirstSibling();
			closest++;
			_scrollRect.content.anchoredPosition += dist;
			if (_dragging) _scrollRect.OnBeginDrag(_lastDrag);
			_prevPosition = _scrollRect.content.anchoredPosition;
			moved = true;
			_snapIndex++;
			if (_snapIndex > _insideElements.Count - 1)
				_snapIndex = 0;
		}
	}


	private Vector2 _velocity;
	private Vector2 _prevPosition;

	private void LateUpdate()
	{
		if (_scrollRect == null || _scrollRect.content == null || _insideElements.Count < 0) 
		{
			if (_scrollRect == null) Debug.LogError("_scrollRect == null");
			if (_scrollRect.content == null) Debug.LogError("_scrollRect.content == null");
			if (_insideElements.Count < 0) Debug.LogError("_insideElements.Count <= 1");
			return;
		}

		if (!_dragging && customInteria)
		{
			Vector2 position = Vector2.zero;

			for (int axis = 0; axis < 2; axis++)
			{
				_velocity[axis] *= Mathf.Pow(customInteriaDecelerationRate, Time.unscaledDeltaTime);
				if (Mathf.Abs(_velocity[axis]) < 1)
					_velocity[axis] = 0;

				position[axis] = _velocity[axis] * Time.unscaledDeltaTime;
			}

			if (!_scrollRect.horizontal) position.x = 0;
			if (!_scrollRect.vertical) position.y = 0;

			_scrollRect.content.anchoredPosition -= position;
			//if (!_lock && !_hardLock) SetCurrentPage(_lockedIndex);
		}

		if (_dragging && customInteria)
		{
			//Vector2 newVelocity = (_startDragPosition - lastDrag.position) / Time.deltaTime;
			Vector2 newVelocity = (_prevPosition - _scrollRect.content.anchoredPosition) / Time.unscaledDeltaTime;
			_velocity = Vector2.Lerp(_velocity, newVelocity, Time.unscaledDeltaTime * 10);
		}

        _prevPosition = _scrollRect.content.anchoredPosition;

		if (!moved && updateElement != null)
		{
			updateElement(snapCenter);
			//Canvas.ForceUpdateCanvases();
			_scrollRect.content.anchoredPosition = _prevPosition;
		}
		moved = false;
	}

	private Vector2 _tmpPos;
	private void LerpToElement(RectTransform target, bool instant = false)
	{
		if (_insideElements.Count <= 1)
		{
			_scrollRect.content.anchoredPosition = snapCenter.anchoredPosition;
		}
		else
		{
			_tmpPos = _scrollRect.content.anchoredPosition;

			if (_scrollRect.horizontal)
			{
				var offset = (target.anchorMin.x - 0.5f) * target.sizeDelta.x;
                _tmpPos.x = Mathf.Lerp(_scrollRect.content.anchoredPosition.x, (-1f * (target.anchoredPosition.x - snapCenter.anchoredPosition.x/2)) - offset,
					instant ? 1 : Time.unscaledDeltaTime * snapTime);
			}
			else if (_scrollRect.vertical)
			{
				var offset = (target.anchorMin.y - 0.5f) * target.sizeDelta.y;
				_tmpPos.y = Mathf.Lerp(_scrollRect.content.anchoredPosition.y, (-1f * target.anchoredPosition.y) - offset,
					instant ? 1 : Time.unscaledDeltaTime * snapTime);
			}

			_scrollRect.content.anchoredPosition = _tmpPos;
		}
	}

	private Vector2 _startDragPosition;
	public virtual void OnBeginDrag(PointerEventData eventData)
	{
		if (_insideElements.Count <= 1) return;
		_dragging = true;
		_lock = false;
		_hardLock = false;

		_startDragPosition = eventData.position;
	}

	private float _minSwipeDistance = 20f;
	public virtual void OnEndDrag(PointerEventData eventData)
	{
		if (_insideElements.Count <= 1) return;
		_dragging = false;

		if (!carousel && useFastSwipe)
		{
			var disX = Mathf.Abs(eventData.position.x - _startDragPosition.x);
			var disY = Mathf.Abs(eventData.position.y - _startDragPosition.y);

			if (_scrollRect.horizontal && disX > _minSwipeDistance)
			{
				if (eventData.position.x - _startDragPosition.x > 0 && _snapIndex >= 0) PrevPage();
				else if (eventData.position.x - _startDragPosition.x < 0 && _snapIndex <= _insideElements.Count - 1) NextPage();
			}
			else if (_scrollRect.vertical && disY > _minSwipeDistance)
			{
				if (eventData.position.y - _startDragPosition.y < 0 && _snapIndex >= 0) PrevPage();
				else if (eventData.position.y - _startDragPosition.y > 0 && _snapIndex <= _insideElements.Count - 1) NextPage();
			}
		}
		else if (carousel)
		{
			//SetCurrentPage(_lockedIndex);
		}
	}

	/// <summary>
	/// Scroll list to next page
	/// </summary>
	public void NextPage()
	{
		if (_dragging) return;
		if (_insideElements.Count >= visibleElements && carousel)
		{
			_hardLock = false;
			_lock = true;
			//_insideElements[0].SetAsLastSibling();
			//RebuildInsideList();
			//LerpToElement(_insideElements[_lockedIndex - 2], true);
			SetCurrentPage(_snapIndex + 1);
		}
		else
		{
			_hardLock = false;
			_lock = true;

			int newIndex = 0;

			if (loopList && _snapIndex + elementsPerPage > _insideElements.Count - 1)
			{
				newIndex = 0;
			}
			else
			{
				newIndex = Mathf.Min(_snapIndex + elementsPerPage, _insideElements.Count - 1);
				newIndex = Mathf.Max(newIndex, GetClosestIndex());
			}

			SetCurrentPage(newIndex);
		}
	}

	public void AddPage(GameObject newPage)
	{
		var rect = newPage.GetComponent<RectTransform>();

		if (rect == null) return;

		rect.SetParent(_scrollRect.content);

		RebuildSnap();
		UpdateButtons();

		if (addedElement != null)
			addedElement(rect);
	}

	public void RemovePage(GameObject pageToRemove)
	{
		RectTransform rect = null;

		int index = 0;
		for (var i = 0; i < _insideElements.Count; i++)
		{
			if (_insideElements[i].gameObject == pageToRemove)
			{
				index = i;
				rect = _insideElements[i];
				break;
			}
		}

		if (rect == null) return;

		rect.gameObject.name = "willdestroy";

		if (removedElement != null)
			removedElement(rect);

		rect.SetParent(null);
		Destroy(rect.gameObject);

		var newIndex = _snapIndex;
		if (index <= _snapIndex) newIndex--;

		RebuildSnap();
		SetCurrentPage(newIndex);
	}

	public void RemovePage(int pageIndexToRemove)
	{
		if (pageIndexToRemove >= _insideElements.Count) return;

		RemovePage(_insideElements[pageIndexToRemove].gameObject);
	}

	/// <summary>
	/// Scroll list to prev page
	/// </summary>
	public void PrevPage()
	{
		if (_dragging) return;
		if (_insideElements.Count >= visibleElements && carousel)
		{
			_lock = true;
			_hardLock = false;
			//_insideElements[_insideElements.Count - 1].SetAsFirstSibling();
			//RebuildInsideList();
			//LerpToElement(_insideElements[_lockedIndex + 2], true);
			SetCurrentPage(_snapIndex - 1);
		}
		else
		{
			_hardLock = false;
			_lock = true;

			int newIndex = 0;

			if (loopList && _snapIndex - elementsPerPage < 0)
				newIndex = _insideElements.Count - 1;
			else
			{
				newIndex = Mathf.Max(_snapIndex - elementsPerPage, 0);
				newIndex = Mathf.Min(newIndex, GetClosestIndex());
			}

			SetCurrentPage(newIndex);
		}
	}

	public void SetCurrentElement(int index)
	{
		Initialize();

		for (var i = 0; i < _insideElements.Count; i++)
		{
			if (_insideElements[i] == _elements[index])
			{
				_lock = false;
				_hardLock = true;
				SetCurrentPage(i);
				return;
			}
		}
	}

	public void SetCurrentElementAndMove(int index)
	{
		Initialize();
        Debug.LogError("SetCurrentElementAndMove");
		for (var i = 0; i < _insideElements.Count; i++)
		{
			if (_insideElements[i] == _elements[index])
			{
				_hardLock = false;
				_lock = true;
				SetCurrentPage(i);
				return;
			}
		}
	}

    public void SetCurrentElementAndMove(RectTransform rect)
    {
        Initialize();

        for (var i = 0; i < _insideElements.Count; i++)
        {
            if (_insideElements[i] == rect)
            {
                _hardLock = false;
                _lock = true;
                SetCurrentPage(i);
                return;
            }
        }
    }

    public void SetCurrentElement(RectTransform element)
	{
		//return;
		for (var i = 0; i < _insideElements.Count; i++)
		{
			if (_insideElements[i] == element)
			{
				_lock = false;
				//if (_lastSnapedElement != _insideElements[i])
				{
					_hardLock = true;
					SetCurrentPage(i);
				}
				return;
			}
		}
	}

	private RectTransform _lastSnapedElement;
	private void SetCurrentPage(int page, bool notify = true)
	{
		var newIndex = Mathf.Clamp(page, 0, _insideElements.Count - 1);

		if (_lastSnapedElement != _insideElements[newIndex] && snapedElement != null && notify)
		{
			snapedElement(_insideElements[newIndex]);
		}

        ManageSelection(_lastSnapedElement, false);

		_lastSnapedElement = _insideElements[newIndex];

        ManageSelection(_lastSnapedElement, true);

		_snapIndex = newIndex;

		UpdateButtons();
	}

//    public void SetSelected(string id)
//    {
//        if (string.IsNullOrEmpty(id)) return;
////        Debug.Log("Set selected: " + id);
    //    Selected = garageItems.Values.FirstOrDefault(x => x.ID == id);


    //}

    private ScrollRectElement selected;
    public ScrollRectElement Selected
    {
        set
        {
            if (value == null) return;
            if(selected != null)
            {
                selected.selected = false;
            }
            selected = value;
            selected.selected = true;
        }
        get
        {
            return selected;
        }
    }

    void DefineSelected(ScrollRectElement garageItem)
    {
        //RandomizedItemData item = null;
        //if (garageItem.GetType() == typeof(GarageVehicleTypeID))
        //{
        //    item = GarageManager.CurrentCar.GetCarModsItem();
        //}
        //else if (garageItem.GetType() == typeof(GarageWheelTypeID))
        //{
        //    item = WheelsHandler.CurrentGarageWheels;
        //}
        //else if (garageItem.GetType() == typeof(GarageHubTypeID))
        //{
        //    item = WheelsHandler.CurrentGarageWheelHubs;
        //}
        //else
        //{
        //    Debug.Log("Not implemented GarageItemType");
        //}
        //if (item != null)
        //{
        //    CarUpgradeUI.OnItemSelected(item);
        //    Debug.Log("Current selected item: " + item.ItemUniqueID());
        //}
    }

    

    [System.NonSerialized]
    public ScrollRectElement preview;
    [System.NonSerialized]
    public ScrollRectElement lastChanged;

    public virtual void ManageSelection(RectTransform rect, bool visible)
    {
        if (rect && garageItems != null)
        {
            if(garageItems.ContainsKey(rect))
            {
                var item = garageItems[rect];

                if(visible)
                {
                    //if(item.IsNew)
                    //{
                    //    item.SetNewToFalseOnItem();
                    //}
                    preview = item;
                    DefineSelected(item);
                }
            }
        }
        Snapped?.Invoke(GetClosestIndex());
    }

    //protected void CurrentSequenceSlotChanged(CarSequenceSlot slot)
    //{
    //    if (Selected == null || Selected.GetType() != typeof(GarageVehicleTypeID)) return;
    //    if (slot.Empty)
    //        Selected.garageItem.selection.enabled = false;
    //    else
    //        Selected.garageItem.selection.enabled = true;
    //}

	public void UpdateButtons()
	{
        try
        {
            if (prevButton != null) prevButton.gameObject.SetActive((_snapIndex > 0 || loopList || carousel) && _insideElements.Count > 1);
            if (nextButton != null) nextButton.gameObject.SetActive((_snapIndex < _insideElements.Count - 1 || loopList || carousel) && _insideElements.Count > 1);
        }
        catch (System.Exception e) { Debug.LogError("UpdateButtons ERROR: " + e.ToString()); }
	}


	private int GetClosestIndex()
	{
        //Debug.LogError("Get Closest index");
		float minDistance = Mathf.Infinity;
		int index = 0;
		for (int i = 0; i < _insideElements.Count; i++)
		{
			var dist = Vector3.SqrMagnitude(_insideElements[i].anchoredPosition + ((RectTransform)_insideElements[i].parent).anchoredPosition - snapCenter.anchoredPosition - _insideElements[i].sizeDelta / 2f);
			if (minDistance > dist)
			{
				minDistance = dist;
				index = i;
			}
		}

		return index;
	}

	private RectTransform GetClosestRect()
	{
		float minDistance = Mathf.Infinity;
		int index = 0;
		for (int i = 0; i < _elements.Count; i++)
		{
            var dist = Vector3.SqrMagnitude(_elements[i].anchoredPosition + ((RectTransform)_elements[i].parent).anchoredPosition - snapCenter.anchoredPosition - _elements[i].sizeDelta / 2f);
			if (minDistance > dist)
			{
				minDistance = dist;
				index = i;

			}
		}

		if (minDistance < 10) _lock = false;
		return _elements[index];
	}

	PointerEventData _lastDrag;
	public virtual void OnDrag(PointerEventData eventData)
	{
		if (_insideElements.Count <= 1) return;

		_lastDrag = eventData;
	}
}