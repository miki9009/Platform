using Engine.UI;
using System;
using System.Collections;
using UnityEngine;

public class Shield : MonoBehaviour, ILefttArmItem
{
    public CollectionObject CollectionObject
    {
        get;set;
    }
    Character character;
    CharacterMovement movement;
    Button button;

    void OnEnable()
    {
        Apply();
    }

    public void Apply()
    {
        character = GetComponentInParent<Character>();
        if(character == null)
        {
            Debug.LogError("Character is null");
            return;
        }
        movement = character.movement;
        CollectionObject = GetComponent<CollectionObject>();
        if (character.IsLocalPlayer)
        {
            var mov = movement as CharacterMovementPlayer;
            button = mov.btnJump;
        }
        StartCoroutine(PlayerUpdate());
    }

    public void BackToCollection()
    {
        Console.WriteLine("Not implemented.");
    }

    public void Remove()
    {
        StopAllCoroutines();
        Engine.PoolingObject.Recycle(gameObject.GetName(), gameObject, () =>
        {


        });
        CollectionObject.BackToCollection(true);
    }

    IEnumerator PlayerUpdate()
    {
        while(true)
        {
#if UNITY_EDITOR
            if (button.isTouched || Input.GetKey(KeyCode.LeftShift))
#else
            if(button.isTouched)
#endif
            {
                movement.shieldUp = true;
#if UNITY_EDITOR
                if (button.PressedTime > 0.25f || Input.GetKey(KeyCode.LeftShift))
#else
                if (button.PressedTime > 0.25f)
#endif
                {
                    if (movement.MovementEnabled)
                    {
                        //Console.WriteLine("Was touched");
                        movement.MovementEnabled = false;
                        movement.anim.SetTrigger("ShieldUp");
                        var velo = movement.velocity;
                        velo.x = 0;
                        velo.z = 0;
                        movement.rb.velocity = velo;
                    }
                }
            }
            else
            {
                if(movement.shieldUp && !movement.isRolling)
                {
                    movement.shieldUp = false;
                    movement.anim.SetTrigger("ShieldDown");
                    movement.MovementEnabled = true;
                }

            }
            yield return null;
        }
    }

    void OnCharacterDead()
    {
        Remove();
    }

    //void OnShieldUp()
    //{
    //    movement.shieldUp = true;
    //}

    //void OnShieldDown()
    //{
    //    movement.shieldUp = false;
    //}
}