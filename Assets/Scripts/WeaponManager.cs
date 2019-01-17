using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WeaponManager : MonoBehaviour
{
    [HideInInspector]public int weaponIndex;
    public List<Weapon> weapons;

    public Weapon GetWeapon(int index)
    {
        return weapons[index];
    }

    [Serializable]
    public class Weapon
    {
        public string weaponName;
        public int fireRate;
        public float recoil;
        public Sprite sprite;
    }
}




