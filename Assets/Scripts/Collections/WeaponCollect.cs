using UnityEngine;

public class WeaponCollect : MonoBehaviour
{
    public GameObject weaponPrefab;
    CollectionObject collection;

    private void Start()
    {
        collection = GetComponent<CollectionObject>();
        collection.OnCollected += CreateWeapon;
    }

    void CreateWeapon(GameObject obj)
    {
        var character = obj.transform.root.GetComponent<Character>();
        var weaponObj = Instantiate(weaponPrefab, character.rightLowerArm);
        var weapon = weaponObj.GetComponent<Weapon>();
        weapon.character = character;
        weapon.collectionObject = collection;
    }
}
