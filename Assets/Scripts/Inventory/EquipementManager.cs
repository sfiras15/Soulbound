using System.Collections.Generic;
using UnityEngine;



// Tracks the current equiped weapon // tracks all of the weapons prefabs in the game and where each type of weapon will be stored inside the player's prefab
public class EquipementManager : MonoBehaviour
{
    [Header("Weapon holders")]
    [SerializeField] private Transform axeHolder;
    [SerializeField] private Transform spearHolder;
    [SerializeField] private Transform maceHolder;


    // list of all the weapons in the game
    [SerializeField] private Weapon[] weaponsPrefab;
    private Weapon currentEquippedWeapon;


    //Dictionary for each weapon type and the appropriate transform to equip the weapon
    private Dictionary<Weapon_SO.WeaponType, Transform> weaponHolders;

    private void Awake()
    {
        weaponHolders = new Dictionary<Weapon_SO.WeaponType, Transform>
        {
            { Weapon_SO.WeaponType.Axe, axeHolder },
            { Weapon_SO.WeaponType.Spear, spearHolder },
            { Weapon_SO.WeaponType.Mace, maceHolder }
        };
    }

    public void EquipWeapon(Weapon_SO weapon)
    {
        Debug.Log("weapon type : " + weapon.type + " weapon level : " + weapon.level);
        Debug.Log(" player level : " + PlayerManager.instance.playerLevel);
        
        if (PlayerManager.instance.playerLevel < weapon.level)
        {
            return; // player low level
        }
        if (currentEquippedWeapon != null && currentEquippedWeapon.item.itemId == weapon.itemId)
        {
            return; // Already equipped
        }

        ClearWeaponHolders();

        foreach (Weapon weaponPrefab in weaponsPrefab)
        {
            // search for the right weapon to equip inside the the list of all the weapons in the game
            if (weaponPrefab.item.itemId == weapon.itemId )
            {
                if (weaponHolders.TryGetValue(weaponPrefab.item.type, out Transform weaponHolder))
                {
                    Equip(weaponPrefab, weaponHolder);
                }
                return;
            }
        }
    }


    // clear all the children of the weapon holders
    private void ClearWeaponHolders()
    {
        foreach (var holder in weaponHolders.Values)
        {
            for (int i = holder.childCount - 1; i >= 0; i--)
            {
                Destroy(holder.GetChild(i).gameObject);
            }
        }
    }

    private void Equip(Weapon weapon, Transform weaponHolder)
    {
        Instantiate(weapon.gameObject, weaponHolder);
        currentEquippedWeapon = weapon;
    }

    public Weapon GetCurrentEquippedWeapon
    {
        get { return currentEquippedWeapon; }
    }
}
