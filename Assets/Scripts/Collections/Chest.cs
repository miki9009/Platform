using Engine;
using System.Collections;
using UnityEngine;

public class Chest : MonoBehaviour, IActivationTrigger
{
    public Transform cover;
    public bool Activated { get; set; }
    public bool Used { get; set; }
    bool wasUsed = false;

    public GameObject collection;
    public int ammount;
    SphereCollider sphere;

    protected virtual void Awake()
    {
        sphere = GetComponent<SphereCollider>();
    }

    public virtual void Activate()
    {
        if (Activated && !wasUsed && Vector3.Distance(transform.position, Character.GetLocalPlayer().transform.position) < sphere.radius)
        {
            wasUsed = true;
            if (enabled)
            StartCoroutine(OpenChest());
            Used = true;
            ActivationTrigger.activatedTriggers -= ActivationTrigger.CHARACTER_TRIGGERS;
            if (ActivationTrigger.activatedTriggers <= 0)
            {
                GetComponent<ActivationTrigger>().DeactivateTrigger();
            }
        }
    }

    Vector3 rotationVector = new Vector3(2, 0, 0);
    IEnumerator OpenChest()
    {
        while (cover.localRotation.eulerAngles.x > 10)
        {
            cover.localRotation = Quaternion.Euler(cover.localRotation.eulerAngles + rotationVector);
            yield return null; ;
        }
        StartCoroutine(CollectionCreate());
        yield return null;
    }

    protected void OpenChestImmediate()
    {
        cover.localEulerAngles = new Vector3(10,0,0);
    }

    readonly Vector3[] dirs = {
        new Vector3(0,0.5f,0),
        new Vector3(1, 0.5f, 1),
         new Vector3(1,0.5f,0),
          new Vector3(1,0.5f,-1),
           new Vector3(0,-0.5f,1),
            new Vector3(-1,0.5f,1),
             new Vector3(-1,0.5f,0),
              new Vector3(-1,0.5f,1)
    };
    const int LENGTH = 8;
    public IEnumerator CollectionCreate()
    {
        if (ammount > LENGTH)
        {
            ammount = LENGTH;
        }
        var coins = new GameObject[ammount];
        for (int i = 0; i < ammount; i++)
        {
            coins[i] = Instantiate(collection, transform.position, Quaternion.identity);
            var collect = coins[i].GetComponent<Collection>();
            if (collect != null) collect.enabled = false;
            yield return null;
        }
        int steps = 30;

        for (int i = 0; i < steps; i++)
        {
            for (int j = 0; j < coins.Length; j++)
            {
                if (coins[j] != null)
                    coins[j].transform.position += dirs[j] * 0.1f;
            }
            yield return null;
        }
        for (int i = 0; i < coins.Length; i++)
        {
            if (coins[i] != null)
            {
                var collect = coins[i].GetComponent<Collection>();
                if (collect != null)
                {
                    collect.enabled = true;
                }
            }
        }
        yield return null;
        if (Controller.Instance.gameType == Controller.GameType.Ortographic)
        {
            for (int j = 0; j < coins.Length; j++)
            {
                if (coins[j] != null)
                {
                    coins[j].transform.position = Character.GetLocalPlayer().transform.position;
                }
                yield return null;
            }
        }
    }
}