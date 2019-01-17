using System.Collections.Generic;
using UnityEngine;
using Engine;

public class Magnet : MonoBehaviour, ILefttArmItem
{
    public bool use;
    [HideInInspector] public Transform upperArm;
    [HideInInspector] public Transform lowerArm;
    public Vector3 upperRotation;
    public Vector3 lowerRotation;
    public float magnetForce;
    Collider col;
    public Character Character { get; set; }

    List<CollectionObject> coins;

    public CollectionObject CollectionObject
    {
        get;
        set;
    }

    private void Start()
    {
        coins = new List<CollectionObject>();
        col = transform.root.GetComponent<Collider>();
        Character.AddItem(this);
    }

    public void Apply()
    {
        Debug.Log("Applied Magnet");
        use = true;
    }

    public void Remove()
    {
        DestroyMe();
    }

    public void BackToCollection()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        var collection = other.GetComponent<CollectionObject>();
        if (collection.type == CollectionType.Coin)
        {
            coins.Add(collection);
        }
    }

    public void DestroyMe()
    {
        PoolingObject.AddSpawn(gameObject.GetName(), gameObject);
    }

    private void LateUpdate()
    {
        upperArm.localRotation = Quaternion.Euler(upperRotation);
        lowerArm.localRotation = Quaternion.Euler(lowerRotation);
        var curPos = transform.root.position;
        for (int i = 0; i < coins.Count; i++)
        {
            var coin = coins[i];
            if (coin != null)
            {
                var dir = Vector.Direction(coin.transform.position, curPos);
                var dis = Vector3.Distance(curPos, coin.transform.position);
                if (dis > 0.1f)
                {
                    coin.transform.position += dir * magnetForce / dis;
                }

                if (coin.rigid == null)
                {
                    coin.rigid = coin.GetComponent<Rigidbody>();
                    if (coin.rigid == null)
                    {
                        coin.rigid = coin.gameObject.AddComponent<Rigidbody>();
                    }
                    coin.rigid.useGravity = false;
                }
                if (dis < 1)
                {
                    coins.Remove(coin);
                }
            }
            else
            {
                coins.Remove(coin);
            }
        }
    }


}