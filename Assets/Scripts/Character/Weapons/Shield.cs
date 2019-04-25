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

    public void Use()
    {
       // ShieldOn();
    }

    public void StopUsing()
    {
       // ShieldOff();
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
        movement.Shield = this;
        CollectionObject = GetComponent<CollectionObject>();
        //if (character.IsLocalPlayer)
        //{
        //    var mov = movement as CharacterMovementPlayer;
        //    button = mov.btnJump;
        //}
        //if(character.IsLocalPlayer)
        //    StartCoroutine(PlayerUpdate());
    }

    public void BackToCollection()
    {
        Engine.Log.Print("Not implemented.");
    }

    public void Remove()
    {
        if(movement.Shield == this)
            movement.Shield = null;
        //StopAllCoroutines();
        Engine.PoolingObject.Recycle(gameObject.GetName(), gameObject, () =>
        {


        });
        CollectionObject.BackToCollection(true);
    }

//    IEnumerator PlayerUpdate()
//    {
//        while(true)
//        {
//#if UNITY_EDITOR
//            if (button.isTouched || Input.GetKey(KeyCode.LeftShift))
//#else
//            if(button.isTouched)
//#endif
//            {
//                movement.shieldUp = true;
//#if UNITY_EDITOR
//                if (button.PressedTime > 0.3f || Input.GetKey(KeyCode.LeftShift))
//#else
//                if (button.PressedTime > 0.3f)
//#endif
//                {
//                    ShieldOn();
//                }
//            }
//            else
//            {
//                ShieldOff();
//            }
//            yield return null;
//        }
//    }

    //public void ShieldOn()
    //{
    //    if (movement.MovementEnabled)
    //    {
    //        //Console.WriteLine("Was touched");
    //        movement.MovementEnabled = false;
    //        movement.anim.ResetTrigger("ShieldDown");
    //        movement.anim.SetTrigger("ShieldUp");
    //        movement.timeBeforeAnotherRoll = 0.25f;
    //        movement.ResetVelocity();
    //    }
    //}

    //public void ShieldOff()
    //{
    //    if (movement.shieldUp && !movement.isRolling)
    //    {
    //        movement.shieldUp = false;
    //        movement.anim.SetTrigger("ShieldDown");
    //        movement.MovementEnabled = true;
    //    }
    //}


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