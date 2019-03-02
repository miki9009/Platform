﻿using Engine;
using Engine.UI;
using UnityEngine;


public class Jetpack : MonoBehaviour
{
    public Vector3 localPosOnBack;
    public Vector3 rotationOnBack;
    Button button;
    Character character;
    public float force;
    public float distance = 5;
    public LayerMask groundColliison;
    public ParticleSystem flames;

    public void SetCharacter(Character character)
    {
        this.character = character;
        transform.SetParent(character.chest);
        transform.localPosition = localPosOnBack;
        transform.localEulerAngles = rotationOnBack;
        button = GameGUI.GetButtonByName("ButtonAttack2");
        button.gameObject.SetActive(true);
    }
    bool hit;
    float curForce = 0;
    private void FixedUpdate()
    {
        hit = CheckGround();
        if (button.isTouched && !character.IsDead && hit)
        {
            character.rb.AddForce(Vector3.up * force, ForceMode.Acceleration);
            if(!flames.isPlaying)
            {
                flames.Play();
            }
        }
        else
        {
            if (flames.isPlaying)
            {
                flames.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }
        }


    }

    bool CheckGround()
    {
        return Physics.Raycast(transform.position, Vector3.down, distance, groundColliison.value, QueryTriggerInteraction.Ignore);
    }
}