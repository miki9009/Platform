
using UnityEngine;

public class Key : Collection
{
    string propertyKey;
    Engine.DataProperty<bool> itemCollected;


    protected override void Start()
    {
        base.Start();
        if (string.IsNullOrEmpty(propertyKey))
        {
            propertyKey = GetComponent<Significant>().propertyKey;
        }
        itemCollected = Engine.DataProperty<bool>.Get(propertyKey, false);
        if(itemCollected.Value)
        {
            collected = true;
            gameObject.SetActive(false);
        }
        OnCollected += (x) => { itemCollected.Value = true; };
    }
}