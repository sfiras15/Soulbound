using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Items/Weapons", menuName = "Items/Weapon", order = 1)]
public class Weapon_SO : Item
{
    public enum WeaponType
    {
        Axe,
        Sword,
        Bow,
    }

    public int damage;
    public int level;
    public WeaponType type;
    public int durability;

    public override void Use()
    {
        base.Use();
        //Equip the weapon 
    }


}
