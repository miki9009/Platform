using Engine;
using System.Collections.Generic;
using UnityEngine;

public class SignificantCollection : LevelElement
{
    static Dictionary<int, CollectionObject> collections = new Dictionary<int, CollectionObject>();
    public static event System.Action<int> SignificantCollected;

    CollectionObject collection;
    public int ID
    {
        get { return collection.ID; }
    }
    private void Awake()
    {
        collection = GetComponent<CollectionObject>();
        collection.Collected += OnCollected;
    }

    private void OnEnable()
    {
        if(!collections.ContainsKey(ID))
        {
            collections.Add(ID, collection);
        }
    }

    private void OnDisable()
    {
        if(collections.ContainsKey(ID))
        {
            collections.Remove(ID);
        }
    }

    void OnCollected(GameObject gameObject)
    {
         Remove();
         SignificantCollected?.Invoke(ID);
    }

    private void OnDestroy()
    {
        if(collection)
            collection.Collected -= OnCollected;
    }

    public bool isSignificant;
    public override void OnSave()
    {        
        base.OnSave();
        if(data!=null)
        {
            data.Add("IsSignificant", isSignificant);
        }
    }

    public override void OnLoad()
    {
        base.OnLoad();
        if(data!=null)
        {
            if(data.ContainsKey("IsSignificant"))
            {
                isSignificant = (bool)data["IsSignificant"];
            }
        }
    }

    public static void Remove(CollectionObject collection)
    {
        if(collection != null && collections.ContainsKey(collection.GetInstanceID()))
        {
            collections.Remove(collection.GetInstanceID());
            if(collections.Count == 0)
            {
                OnEmpty();
            }
        }
    }

    public static event System.Action Empty;
    static void OnEmpty()
    {
        Empty?.Invoke();
    }

    void Remove()
    {
        if (collections.ContainsKey(ID))
        {
            collections.Remove(ID);
        }
        if(collections.Count == 0)
        {
            OnEmpty();
        }
    }

    public static int Count
    {
        get { return collections.Count; }
    }



    public static CollectionObject FindNearest(Vector3 position)
    {
        float dis = Mathf.Infinity;
        float dis2 = 0;
        CollectionObject nearest = null;
        foreach (var item in collections.Values)
        {
            if (item.AINotReachable) continue;

            dis2 = Vector3.Distance(position, item.transform.position);
            if(dis2 < dis)
            {
                dis = dis2;
                nearest = item;
            }
        }
        return nearest;
    }
}