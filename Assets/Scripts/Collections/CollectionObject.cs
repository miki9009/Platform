﻿using Engine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CollectionObject : MonoBehaviour, IPoolObject
{
    public CollectionType type;
    public int val;
    public bool emmitParticles;
    public int particlesAmmount;
    public bool collected = false;

    private Character character;
    private Vector3 localScale;
    public bool AINotReachable { get; set; }

    public delegate void Collect(GameObject collector);
    public event Collect Collected;
    public event Action<GameObject> TriggerLeaved;
    [HideInInspector] public Rigidbody rigid;

    protected Coroutine collectedCoroutine;

    [NonSerializedAttribute]
    public CollectionDisplay display;

    int _id = -1;
    public int ID
    {
        get
        {
            if (_id == -1)
                _id = GetInstanceID();
            return _id;
        }

    }

    public void OnTriggerEnter(Collider other)
    {
        var obj = other.gameObject;
        if (obj.layer == 14)
        {
            if (collected) return;
            collected = true;
            Collected?.Invoke(obj);
            character = other.GetComponentInParent<Character>();
            int playerID = character.ID;
            if(character.movement.IsLocalPlayer)
                display.ShowDisplay();
            CollectionManager.Instance.SetCollection(playerID, type, val);            
            collectedCoroutine = StartCoroutine(CollectedCor());
            if (emmitParticles)
            {
                CollectionManager.Instance.EmmitParticles(type, transform.position + Vector3.up, particlesAmmount);
            }
            TargetPointerActivator pointerActivator = GetComponent<TargetPointerActivator>();
            if (pointerActivator != null)
                pointerActivator.enabled = false;
        }
    }

    void OnTriggerExit(Collider other)
    {
        TriggerLeaved?.Invoke(other.gameObject);
    }

    void OnEnable()
    {
        AddToLevelCollection();
    }


    private void OnDisable()
    {
        if(collected && collectedCoroutine != null)
        {
            var manager =  GetComponent<ActiveObject>();
            if (manager != null)
                manager.DeactivatedByManager = false;
            else
                Debug.Log("Should be Deactivated by Manager, but manager is null");
        }
        RemoveFromLevelCollection();

    }

    void RemoveFromLevelCollection()
    {
        if (!CollectionManager.Instance) return;
        if (collected && CollectionManager.Instance.LevelCollections.ContainsKey(this))
        {
            CollectionManager.Instance.LevelCollections.Remove(this);
        }
    }

    void AddToLevelCollection()
    {
        if (!CollectionManager.Instance) return;
        if (!collected && !CollectionManager.Instance.LevelCollections.ContainsKey(this))
        {
            CollectionManager.Instance.LevelCollections.Add(this, type);
        }
    }

    private void Awake()
    {
        localScale = transform.localScale;
        
        if (!GameManager.IsSceneLoaded)
        {
            GameManager.GameReady += AssignDisplayOnLoad;
        }
        else
        {
            display = CollectionDisplayManager.Instance.AssignDisplayObject(type);
        }
    }

    protected virtual void Start()
    {
        var col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = true;
        }
    }

    void AssignDisplayOnLoad()
    {
        try
        {
            display = CollectionDisplayManager.Instance.AssignDisplayObject(type);
        }
        catch(Exception ex)
        {
            enabled = false;
            Debug.Log(ex.Message);
        }
    }

    private void Update()
    {
        transform.rotation = rotation;
    }

    protected virtual IEnumerator CollectedCor()
    {
        while(transform.localScale.x > 0.05)
        {
            transform.localScale /= 1.05f;
            yield return null;
        }

        yield return new WaitForSeconds(deactivationTime);
        collectedCoroutine = null;
        Deactivate();
        yield return null;
    }

    public float deactivationTime = 0.1f;
    void Deactivate()
    {
        if(gameObject.activeInHierarchy)
            gameObject.SetActive(false);
    }

    public static Vector3 eulers = Vector3.zero;
    public static Quaternion rotation = Quaternion.identity;


    public GameObject GameObject
    {
        get { return gameObject; }
    }


    public void BackToCollection(bool activate = false)
    {
        if (character != null && !activate)
        {
            transform.position = character.transform.position;
            int collection = CollectionManager.Instance.GetCollection(character.ID, type);
            CollectionManager.Instance.SetCollection(character.ID, type, collection - val);
        }
        if(activate)
            gameObject.SetActive(true);
        if (collectedCoroutine != null)
        {
            StopCoroutine(collectedCoroutine);
        }
        collected = false;
        GetComponent<Collider>().enabled = true;
        //OnLeaveTrigger += SetCollectionObjectActive;

        transform.localScale = localScale;
    }

    void SetCollectionObjectActive(GameObject gameObject)
    {
        var character = gameObject.GetComponentInParent<Character>();
        if (character != null && character == this.character)
        {
            collected = false;
            TriggerLeaved -= SetCollectionObjectActive;
        }
    }

    public virtual void AdditionalRecycle()
    {
        BackToCollection();
    }
}

public enum CollectionType
{
    Coin = 0,
    Emmerald = 1,
    Health = 2,
    Clock = 3,
    Magnet = 4,
    Weapon = 5,
    Throwable = 6,
    Invincibility = 7,
    KeyGold = 8,
    KeySilver = 9,
    KeyBronze = 10,
    Restart = 11,
    DestroyCrate = 12,
    KillEnemy = 13,
    WaypointVisited = 14,
    FirePlaceReached = 15,
    Growth = 16,
    Fireball = 17
}

