using UnityEngine;
using UnityEngine.UI;

public class ChestSignificant : Chest
{
    string propertyKey;
    Engine.DataProperty<bool> itemCollected;

    public Color gold;
    public Color silver;
    public Color bronze;

    public enum KeyType { Gold, Silver, Bronze}
    public int requiredKeys;
    public KeyType keyType;
    public TextMesh requiredText;
    public RequiredUI requiredFrame;
    public Sprite[] keys;

    Sprite keySprite;
    GameObject characterEntered;

    int possesedKeys;
    bool collected;

    protected override void  Awake()
    {
        base.Awake();
        requiredText.text = requiredKeys.ToString();
        if (keyType == KeyType.Bronze)
        {
            keySprite = keys[2];
            requiredText.color = bronze;
        }else if(keyType == KeyType.Gold)
        {
            keySprite = keys[0];
            requiredText.color = gold;

        }else
        {
            requiredText.color = silver;
            keySprite = keys[1];
        }
        requiredFrame.gameObject.SetActive(false);

    }

    private void Start()
    {
        propertyKey = GetComponentInChildren<Significant>().propertyKey;
        CheckCollected();
    }

    void CheckCollected()
    {
        itemCollected = Engine.DataProperty<bool>.Get(propertyKey, false);
        if(itemCollected.Value)
        {
            GetComponent<SphereCollider>().enabled = false;
            OpenChestImmediate();
        }
    }


    public override void Activate()
    {
        if (!itemCollected.Value)
        {
            if (Open())
            {
                base.Activate();
            }
            itemCollected.Value = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!itemCollected.Value)
        {
            requiredFrame.gameObject.SetActive(true);
            AssignRequiredFrame();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        requiredFrame.gameObject.SetActive(false);
    }

    void AssignRequiredFrame()
    {
        requiredFrame.Assign(requiredKeys, possesedKeys, keySprite);
    }

    bool Open()
    {
        CollectionType type = CollectionType.KeyGold;
        if (keyType == KeyType.Bronze)
            type = CollectionType.KeyBronze;
        else if (keyType == KeyType.Silver)
            type = CollectionType.KeySilver;
        else
            type = CollectionType.KeyGold;

        int keys = CollectionManager.Instance.GetCollection(Character.GetLocalPlayer().ID, type);

        var data = DataManager.Collections;

        if (type == CollectionType.KeyGold)
            keys += data.goldKeys;
        if (type == CollectionType.KeyBronze)
            keys += data.bronzeKeys;
        if (type == CollectionType.KeySilver)
            keys += data.silverKeys;

        possesedKeys = keys;

        if (keys >= requiredKeys)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}