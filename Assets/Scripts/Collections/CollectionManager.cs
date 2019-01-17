using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Engine;
using System.Linq;

public class CollectionManager : MonoBehaviour
{
    static CollectionManager instance;
    public Dictionary<CollectionObject, CollectionType> LevelCollections = new Dictionary<CollectionObject, CollectionType>();
    public ParticleSystem[] particles;
    public static CollectionManager Instance
    {
        get
        {
            if (instance == null)
            {
                var obj = new GameObject("CollectionManager");
                instance = obj.AddComponent<CollectionManager>();
            }
            return instance;
        } 
        private set
        {
            instance = value;
        }
    }


    private void OnDestroy()
    {
        instance = null;
        Level.LevelLoaded-= Clear;
    }


    private void Awake()
    {
        collections = new Dictionary<int, CollectionSet>();
        instance = this;
        Level.LevelLoaded += Clear;
    }

    Dictionary<int, CollectionSet> collections;

    public event System.Action<int, CollectionType, int> Collected;
    public void OnCollected(int id, CollectionType collection, int amount)
    {
        Collected?.Invoke(id, collection, amount);
    }

    void Clear()
    {
        collections.Clear();
        LevelCollections.Clear();
        ResetCollections();
    }

    public Dictionary<int, CollectionSet> AllCollections()
    {
        return collections;
    }

    public void SetCollection(int playerID, CollectionType type, int val)
    {
        if (collections.ContainsKey(playerID))
        {
            if (collections[playerID].Collection.ContainsKey(type))
            {
                collections[playerID].Collection[type] = val;
                OnCollected(playerID, type, val);
                //Debug.Log("Collected: " + type);
            }
            else
            {
                collections[playerID].Collection.Add(type, 0);
                SetCollection(playerID, type, val);
            }
        }
        else
        {
            collections.Add(playerID, new CollectionSet(type, playerID));
            SetCollection(playerID, type, val);
        }

    }

    public void AddToCollection(int playerID, CollectionType type, int val)
    {
        if (collections.ContainsKey(playerID))
        {
            if (collections[playerID].Collection.ContainsKey(type))
            {
                collections[playerID].Collection[type] += val;
                OnCollected(playerID, type, val);
                //Debug.Log("Collected: " + type);
            }
            else
            {
                collections[playerID].Collection.Add(type, 0);
                SetCollection(playerID, type, val);
            }
        }
        else
        {
            collections.Add(playerID, new CollectionSet(type, playerID));
            SetCollection(playerID, type, val);
        }

    }


    public int GetCollection(int playerID, CollectionType type)
    {
        if (collections.ContainsKey(playerID))
        {
            if (collections[playerID].Collection.ContainsKey(type))
            {
                return collections[playerID].Collection[type];
            }
            else
            {
                collections[playerID].Collection.Add(type, 0);
                return collections[playerID].Collection[type];
            }
        }
        else
        {
            collections.Add(playerID, new CollectionSet(type, playerID));
            if (!collections[playerID].Collection.ContainsKey(type))
            {
                collections[playerID].Collection.Add(type, 0);
            }
            return collections[playerID].Collection[type];
        }
    }


    public void EmmitParticles(CollectionType type, Vector3 position, int ammount)
    {
        //Debug.Log("Emmit particles");
        int index = (int)type;
        if (particles.Length > index && particles[index] != null)
        {

            var parts = particles[index];

            var emitParams = new ParticleSystem.EmitParams();
            parts.transform.position = position;
            for (int i = 0; i < ammount; i++)
            {
                emitParams.position = new Vector3(Random.Range(-1,1), Random.Range(-1, 1), Random.Range(-1, 1));

                parts.Emit(emitParams, 1);
                //Debug.Log("Did emmit");
                //parts.Emit(10);
            }

        }
    }

    public void ResetCollections()
    {
        collections.Clear();
    }

#if UNITY_EDITOR
    [Header("DEBUG")]
    public bool debug;
    public Color color;

    private void OnGUI()
    {
        if (!debug) return;
        int y = 50;
        Draw.TextColorUnity(10, y, color, instance);
        y += 30;
        foreach (var collection in collections)
        {
            foreach (var col in collection.Value.Collection)
            {
                Draw.TextColorUnity(10, y, color, string.Format("PlayerID: {0}, Collection: {1}, Value: {2}", collection.Key, col.Key, col.Value));
                y += 30;
            }

        }
    }
#endif

    public class CollectionSet
    {
        public CollectionSet(CollectionType type, int playerID)
        {
            if (collection == null)
            {
                collection = new Dictionary<CollectionType, int>();
            }
            collection.Add(type, 0);
        }
        Dictionary<CollectionType, int> collection;
        public Dictionary<CollectionType, int> Collection
        {
            get { return collection; }
        }
    }


}


