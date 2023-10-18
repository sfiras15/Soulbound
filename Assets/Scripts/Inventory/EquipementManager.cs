using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public class WeaponHolder
{
    public Weapon_SO.WeaponType type;
    public Transform transform;
}

// Tracks the current equiped weapon // tracks all of the weapons prefabs in the game and where each type of weapon will be stored inside the player's prefab
public class EquipementManager : MonoBehaviour,IDataPersistence
{
    [Header("Weapon holders")]
    [SerializeField] private Transform axeHolder;
    [SerializeField] private Transform spearHolder;
    [SerializeField] private Transform maceHolder;

    [SerializeField] List<WeaponHolder> holders;

    private Weapon_SO currentEquippedWeapon;

    //Dictionary for each weapon type and the appropriate transform to equip the weapon
    private Dictionary<Weapon_SO.WeaponType, Transform> weaponHolders;

    public void SaveData(ref GameData data)
    {
        if (currentEquippedWeapon != null)
        {
            SerializableWeapon_SO serializableWeapon_SO = new SerializableWeapon_SO();

            serializableWeapon_SO.weaponData = JsonUtility.ToJson(currentEquippedWeapon);
            Debug.Log("Serialized Weapon Data: " + serializableWeapon_SO.weaponData);

            serializableWeapon_SO.weaponPrefabId = currentEquippedWeapon.prefab.GetComponent<Weapon>().GetPrefabID;
            data.currentWeapon = serializableWeapon_SO;
        }
        else
        {
            data.currentWeapon = null;
        }
        //Debug.Log("CurrentWeapon on Save : " + currentEquippedWeapon);
        //Debug.Log("CurrentSavedWeapon on Save : " + data.currentWeapon);

    }



    public void LoadData(GameData data)
    {
        //Debug.Log("CurrentWeapon on Load : " + currentEquippedWeapon);
        currentEquippedWeapon = null;
        //Debug.Log("CurrentSavedWeapon on Load : " + data.currentWeapon);
        if (data.currentWeapon != null)
        {
            //SerializableWeapon_SO currentSavedWeapon = new SerializableWeapon_SO();
            //currentSavedWeapon = JsonUtility.FromJson<SerializableWeapon_SO>(data.currentWeapon.weaponData);
            Weapon_SO currentSavedWeapon = (Weapon_SO) DeserializeItem(data.currentWeapon.weaponData);
            if (currentSavedWeapon != null && !currentSavedWeapon.HasDefaultValues())
            {
                Debug.Log("WTF");
                EquipWeapon((Weapon_SO)currentSavedWeapon, data.currentWeapon.weaponPrefabId);
            }
        }
    }


    private Item DeserializeItem(string jsonData)
    {
        Item deserializedItem = ScriptableObject.CreateInstance<Weapon_SO>();
        JsonUtility.FromJsonOverwrite(jsonData, deserializedItem);
        return deserializedItem;
    }

    private void Awake()
    {
        weaponHolders = new Dictionary<Weapon_SO.WeaponType, Transform>();

        foreach (var item in holders)
            weaponHolders.Add(item.type, item.transform);
    }

    public void EquipWeapon(Weapon_SO weapon,int wpPrefab = -1)
    {
        if (weapon != null)
        {
            Debug.Log("weapon :" + weapon.itemId);
            Debug.Log("WpPrefab :" + wpPrefab);
            //Debug.Log("weapon type : " + weapon.type + " weapon level : " + weapon.level);
            //Debug.Log("weapon damage : " + weapon.damage);
            //Debug.Log("weapon attackRange : " + weapon.attackRange);
            //Debug.Log("weapon attackDuration : " + weapon.attackDuration);
            //Debug.Log("weapon staminaConsumption : " + weapon.staminaConsumption);
            //Debug.Log("weapon prefab : " + weapon.prefab);
            ////Debug.Log(" player level : " + PlayerManager.instance.playerLevel);
            //Debug.Log("current equipped weapon " + currentEquippedWeapon);
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
                Equip(weapon, weaponHolder, wpPrefab);
            }
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

    private void Equip(Weapon_SO weapon, Transform weaponHolder,int wpPrefabId = -1)
    {
        //Debug.Log("wpPrefabId :" + wpPrefabId);
        var weaponPrefabID = weapon.prefab.GetComponent<Weapon>().GetPrefabID;
        //Debug.Log("WeaponPrefabID : " + weaponPrefabID);

        for (int i = weaponHolder.childCount - 1; i >= 0; i--)
        {
            // compare the weapon.prefabID with all of the weapons id inside the player's weapon holder
            var prefabID =weaponHolder.GetChild(i).gameObject.GetComponent<Weapon>().GetPrefabID;

            //Debug.Log("prefab id : " + prefabID);
            //Debug.Log("weapon prefab id : " + weaponPrefabID);
            //Debug.Log(prefabID == weaponPrefabID);


            if (prefabID == weaponPrefabID || prefabID == wpPrefabId)
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
