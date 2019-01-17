using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[DefaultExecutionOrder(-50)]
public class CollectionDisplayManager : MonoBehaviour
{
    public Sprite[] collectionSprites;
    public GameObject prefab;
    public bool isDisplaying { get; set; }

    public static CollectionDisplayManager Instance
    {
        get; private set;
    }

    public List<CollectionDisplay> displayList;

    private void Awake()
    {
        Instance = this;
        displayList = new List<CollectionDisplay>();
    }

    public void StopDisplaying()
    {
        displayList.ForEach(x =>
        {
            x.StopAllCoroutines();
            x.coroutine = null;
            x.enabled = false;
            x.group.alpha = 0;
        });
    }

    public CollectionDisplay AssignDisplayObject(CollectionType type)
    {
        CollectionDisplay obj = null;
        displayList.ForEach(x =>
        {
            if (x.type == type)
            {
                obj = x;
            }
        });
        if (obj == null)
        {
            var o = Instantiate(prefab, transform);
            obj = o.GetComponent<CollectionDisplay>();
            obj.name = "CollectionDisplay - " + type;
            obj.type = type;
            int index = (int)type;
            if (index < collectionSprites.Length)
            {
                obj.image.sprite = collectionSprites[index];
            }
            else
            {
                obj.image.sprite = collectionSprites[0];
                Debug.LogError("Sprite array is smaller than index");
            }
            displayList.Add(obj);
        }

        return obj;
    }
}