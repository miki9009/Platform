using System;
using System.Collections.Generic;

namespace Engine
{
public class ConcurrentDictionary<TKey, TValue> : IDictionary<TKey, TValue>
{
    private readonly object syncLock = new object();
    private Dictionary<TKey, TValue> dict;

    public ConcurrentDictionary()
    {
        this.dict = new Dictionary<TKey, TValue>();
    }

    public TValue this[TKey key]
    {
        get
        {
            lock (syncLock)
            {
                return dict[key];
            }
        }
        set
        {
            lock (syncLock)
            {
                dict[key] = value;
            }
        }
    }

    public ICollection<TKey> Keys
    {
        get
        {
            lock (syncLock)
            {
                return dict.Keys;
            }
        }
    }
    public ICollection<TValue> Values
    {
        get
        {
            lock (syncLock)
            {
                return dict.Values;
            }
        }
    }

    public int Count
    {
        get
        {
            lock (syncLock)
            {
                return dict.Count;
            }
        }
    }

    public bool ContainsKey(TKey item)
    {
        lock (syncLock)
        {
            return dict.ContainsKey(item);
        }
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        lock (syncLock)
        {
            dict.Add(item.Key, item.Value);
        }
    }

    public void Add(TKey key, TValue value)
    {
        lock (syncLock)
        {
            dict.Add(key, value);
        }
    }

    public bool Remove(TKey key)
    {
        lock (syncLock)
        {
            return dict.Remove(key);
        }
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        lock (syncLock)
        {
            return dict.Remove(item.Key);
        }
    }

    public void Clear()
    {
        lock (syncLock)
        {
            dict.Clear();
        }
    }

    public TKey[] GetKeysArray()
    {
        lock (syncLock)
        {
            TKey[] result = new TKey[dict.Keys.Count];
            dict.Keys.CopyTo(result, 0); return result;
        }
    }

    public bool Contains(object key)
    {
        lock (syncLock)
        {
            return dict.ContainsValue((TValue)key);
        }
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        lock (syncLock)
        {
            return dict.ContainsValue(item.Value);
        }
    }

    IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
    {
        lock (syncLock)
        {
            return dict.GetEnumerator();
        }
    }

    public bool IsFixedSize
    {
        get
        {
            lock (syncLock)
            {
                return true;
            }
        }
    }

    public void CopyTo(Array array, int index)
    {
        lock (syncLock)
        {
            dict.Values.CopyTo((TValue[])array, index);
        }
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        lock (syncLock)
        {
            TKey[] result = new TKey[dict.Keys.Count];
            dict.Keys.CopyTo(result, 0);
        }
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        lock (syncLock)
        {
            return dict.TryGetValue(key, out value);
        }
    }

    public bool IsReadOnly
    {
        get
        {
            lock (syncLock)
            {
                return false;
            }
        }
    }

    public System.Collections.IEnumerator GetEnumerator()
    {
        return dict.GetEnumerator();
    }
}

}