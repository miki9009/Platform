using UnityEngine;

public class WeaponCollect : ItemCollect
{
    protected override void CreateItem(GameObject obj)
    {
        base.CreateItem(obj);
        if(itemObj && character)
        {
            var weapon = itemObj.GetComponent<Weapon>();
            weapon.character = character;
            weapon.CollectionObject = collection;
        }
    }
}
