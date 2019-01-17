using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Engine;

public class Crate : MonoBehaviour, IDestructible
{
    public Transform Transform { get { return transform; } set { Transform = value; } }
    public GameObject collection;
    public int collectionAmmount;
    public LayerMask layer;
    //public TriggerBroadcast broadcast;

    ParticleSystem crateExplosion;
    Renderer rend;
    BoxCollider boxCol;
    SphereCollider sCol;
    Rigidbody rb;
    bool destructed = false;
    public float upForce = 10;

    public Rigidbody Rigidbody
    {
        get
        {
            return rb;
        }
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        crateExplosion = StaticParticles.Instance.crateExploded;
        rend = GetComponent<Renderer>();
        boxCol = GetComponent<BoxCollider>();
        sCol = GetComponent<SphereCollider>();
        //broadcast.TriggerEntered += OnTriggerBroadcast;
        for (int i = 0; i < collectionAmmount; i++)
        {
            PoolingObject.AddSpawn(collection.name, Instantiate(collection));
        }
    }

    //void OnTriggerBroadcast(Collider col)
    //{
    //    Debug.Log("Hit");
    //    if(!destructed)
    //    {
    //        var character = col.GetComponentInParent<CharacterMovement>();
    //        float angle = Vector3.Dot(character.rb.rotation.Vector(), Vector3.down);
    //        Debug.Log(character.rb.velocity.y);
    //        if(character!=null && character.rb.velocity.y < -5)
    //        {
    //            Hit(character);
    //            character.rb.velocity = new Vector3(character.rb.velocity.x,15, character.rb.velocity.z);
    //            character.onGround = false;
    //            //character.rb.AddForce(Vector3.up * upForce, ForceMode.VelocityChange);
    //        }
    //    }
    //}

    //void OnTriggerEnter(Collider other)
    //{
    //    var rb = other.attachedRigidbody;
    //    if (rb != null && rb.velocity.y < -10)
    //    {
    //        var velo = rb.velocity;
    //        rb.AddForce(Vector3.up * 25, ForceMode.VelocityChange);
    //        Debug.Log("New velocity: " + rb.velocity);
    //    }
    //}

    readonly Vector3[] dirs = {
        new Vector3(0,-0.5f,0),
        new Vector3(1, -0.5f, 1),
         new Vector3(1,-0.5f,0),
          new Vector3(1,-0.5f,-1),
           new Vector3(0,-0.5f,1),
            new Vector3(-1,-0.5f,1),
             new Vector3(-1,-0.5f,0),
              new Vector3(-1,-0.5f,1)
    };
    const int LENGTH = 8;
    public IEnumerator CollectionCreate()
    {
        if (collectionAmmount > LENGTH)
        {
            collectionAmmount = LENGTH;
        }
        var coins = new GameObject[collectionAmmount];
        for (int i = 0; i < collectionAmmount; i++)
        {
            coins[i] = PoolingObject.GetSpawn(collection.name, transform.position, Quaternion.identity);
            coins[i].GetComponent<Collection>().enabled = false;
            yield return null;
        }
        int steps = 15;

            for (int i = 0; i < steps; i++)
            {
                for (int j = 0; j < coins.Length; j++)
                {
                    if (coins[j] != null)
                    {
                        coins[j].transform.position += dirs[j] * 0.1f;
                    }
                }
                yield return null;
            }
            for (int i = 0; i < coins.Length; i++)
            {
                if (coins[i] != null)
                    coins[i].GetComponent<Collection>().enabled = true;
            }
        gameObject.SetActive(false);
    }

    public void Hit(CharacterMovement character)
    {
        if (destructed) return;
        destructed = true;
        CollectionManager.Instance.SetCollection(character.character.ID, CollectionType.DestroyCrate, 1);
        crateExplosion.transform.position = transform.position;
        crateExplosion.Play();
        StartCoroutine(CollectionCreate());
        rend.enabled = false;
        boxCol.enabled = false;
        sCol.enabled = false;
    }
}
