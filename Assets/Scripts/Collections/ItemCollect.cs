using UnityEngine;

public class ItemCollect : MonoBehaviour
{
    public enum ItemSlot
    {
        RightLowerArm,
        LeftLowerArm
    }
    public GameObject itemPrefab;
    protected CollectionObject collection;
    public ItemSlot slot;
    protected Character character;
    protected GameObject itemObj;

    private void Start()
    {
        collection = GetComponent<CollectionObject>();
        collection.Collected += CreateItem;
    }

    protected virtual void CreateItem(GameObject obj)
    {
        character = obj.transform.GetComponentInParent<Character>();
        itemObj = Instantiate(itemPrefab, GetCharacterPart(slot, character));
    }

    Transform GetCharacterPart(ItemSlot _slot, Character character)
    {
        if(_slot == ItemSlot.LeftLowerArm)
        {
            return character.leftLowerArm;
        }
        else if(_slot == ItemSlot.RightLowerArm)
        {
            return character.rightLowerArm;
        }
        Debug.LogError("Not implemented ItemSlot");
        return null;
    }
}
