using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

[CreateAssetMenu(fileName = "ScriptableObjects/Items/Weapons", menuName = "Items/Weapon", order = 1)]
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
    public float attackRange;
    public float attackDuration;
    public float staminaConsumption;
    public GameObject prefab;
    public override void Use()
    {
        Debug.Log("Weapon");
        Debug.Log(this);
        if (usable)
        {
            Inventory.instance.UseWeapon(this);
        }
        //Equip the weapon 
    }

    // For checking if we have created an Empty weapon_SO by loading  & deserializing Data from gameData
    public bool HasDefaultValues()
    {
        return damage == 0 && level == 0 && type == WeaponType.Axe && attackRange == 0 && attackDuration == 0 && staminaConsumption == 0 && prefab == null;
    }


}
