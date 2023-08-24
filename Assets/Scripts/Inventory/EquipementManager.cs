using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class WeaponHolder
{
    public Weapon_SO.WeaponType type;
    public Transform transform;
}

// Tracks the current equiped weapon // tracks all of the weapons prefabs in the game and where each type of weapon will be stored inside the player's prefab
public class EquipementManager : MonoBehaviour
{
    [Header("Weapon holders")]
    [SerializeField] private Transform axeHolder;
    [SerializeField] private Transform spearHolder;
    [SerializeField] private Transform maceHolder;

    [SerializeField] List<WeaponHolder> holders;

    private Weapon_SO currentEquippedWeapon;

    //Dictionary for each weapon type and the appropriate transform to equip the weapon
    private Dictionary<Weapon_SO.WeaponType, Transform> weaponHolders;

    private void Awake()
    {
        weaponHolders = new Dictionary<Weapon_SO.WeaponType, Transform>();

        foreach (var item in holders)
            weaponHolders.Add(item.type, item.transform);

    }

    public void EquipWeapon(Weapon_SO weapon)
    {
        //Debug.Log("weapon type : " + weapon.type + " weapon level : " + weapon.level);
        //Debug.Log(" player level : " + PlayerManager.instance.playerLevel);
        
        if (PlayerManager.instance.playerLevel < weapon.level)
        {
            return; // player low level
        }
        if (currentEquippedWeapon != null && currentEquippedWeapon.itemId == weapon.itemId)
        {
            return; // Already equipped
        }
        
        if (currentEquippedWeapon != null) ClearWeaponHolder(currentEquippedWeapon.type);

        if (weaponHolders.TryGetValue(weapon.type, out Transform weaponHolder))
        {
            Equip(weapon, weaponHolder);
        }

    }

    public void RemoveWeapon(Weapon_SO.WeaponType type)
    {
        ClearWeaponHolder(type);
        currentEquippedWeapon = null;
    }
    // set all the children's gameobjects of the current equipped weapon holder to false
    public void ClearWeaponHolder(Weapon_SO.WeaponType type)
    {

        //Debug.Log("current equipped weapon type is : " + type);
        if (weaponHolders.TryGetValue(type, out Transform weaponHolder))
        {
            for (int i = weaponHolder.childCount - 1; i >= 0; i--)
            {
                weaponHolder.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    private void Equip(Weapon_SO weapon, Transform weaponHolder)
    {
        var weaponPrefabID = weapon.prefab.GetComponent<Weapon>().GetPrefabID; 

        for (int i = weaponHolder.childCount - 1; i >= 0; i--)
        {
            // compare the weapon.prefabID with all of the weapons id inside the player's weapon holder
            var prefabID =weaponHolder.GetChild(i).gameObject.GetComponent<Weapon>().GetPrefabID;

            //Debug.Log("prefab id : " + prefabID);
            //Debug.Log("weapon prefab id : " + weaponPrefabID);
            //Debug.Log(prefabID == weaponPrefabID);


            if (prefabID == weaponPrefabID)
            {
                weaponHolder.GetChild(i).gameObject.SetActive(true);
                currentEquippedWeapon = weapon;
                return;
            }         
        }

        
    }

    public Weapon_SO GetCurrentEquippedWeapon
    {
        get { return currentEquippedWeapon; }
    }
}
