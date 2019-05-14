using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CharacterStatistics
{
    public static event Action<Character, int> HealthChanged;

    public float runSpeed;
    public int startHealth;
    public float jumpForce;
    public float turningSpeed;
    public float damage;
    public float attackForce;
    public float pushForce = 0.2f;
    public string armorType = "Normal";

    public Character Character
    {
        get; private set;
    }

    int _health;
    public int Health
    {
        get
        {
            return _health;
        }

        set
        {
            _health = value;
            HealthChanged?.Invoke(Character, value);
        }
    }


    public void Initialize(Character character)
    {
        Debug.Log("Stats initialized");
        _health = startHealth;
        Character = character;
    }
}


