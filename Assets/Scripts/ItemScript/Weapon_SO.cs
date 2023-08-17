using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Items/Weapons", menuName = "Items/Weapon", order = 1)]
public class Weapon_SO : Item
{
    public enum WeaponType
    {
        Axe=0,
        //Sword=1,
        //Bow=2,
        Spear=1,
        Mace=2,
    }

    public int damage;
    public int level;
    public WeaponType type;
    public float AttackRange;
    public float AttackDuration;
    public override void Use()
    {
        if (usable)
        {
            Inventory.instance.UseWeapon(this);
        }
        //Equip the weapon 
    }


}
