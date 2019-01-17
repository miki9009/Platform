using UnityEngine;

public class MonkeyCoconut : MonoBehaviour
{
    GameObject parent;
    public GameObject Parent
    {
        get
        {
            return parent;
        }
        set
        {
            parent = value;
        }
    }
    public GameObject playerCoconut;
    Rigidbody rigid;
    private void OnCollisionEnter(Collision other)
    {
        var script = Parent.gameObject.GetComponent<MonkeyThrowable>();
        rigid = GetComponent<Rigidbody>();
        if (script != null)
        {
            var character = other.gameObject.GetComponent<Character>();
            if (character != null)
            {
                if (character.movement.isAttacking)
                {
                    rigid.velocity = Engine.Vector.Direction(transform.position, Parent.transform.position + Vector3.up * 2) * 40;
                    Parent = character.gameObject;
                }
                else
                {
                    character.movement.Hit();
                }
            }
        }
        else
        {
            var monkey = other.gameObject.GetComponent<MonkeyThrowable>();
            if(monkey != null)
            {
                if(Parent!= null)
                {
                   var c =  Parent.gameObject.GetComponent<Character>();
                    if(c!= null)
                    {
                        monkey.Hit(c.movement);
                        gameObject.SetActive(false);
                    }
                }

            }
        }
    }
}