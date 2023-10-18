using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour,IDataPersistence
{
    public static Inventory instance;
    [SerializeField] private int inventorySize;

    //Next free key position in the dictionary
    private int nextAvailableKey;

    private int currentInventorySize;


    // position of the key of the item.itemId if he's in the dictionary 
    private int idLocation;

    // updating inventory UI
    public static event Action onItemChanged;

    // Event for using consumables
    public static event Action<Consumable_SO> onConsumableUse;    

    public Dictionary<int, Item> inventoryDictionary = new Dictionary<int, Item>();
    
    [SerializeField] private EquipementManager equipementManager;

    //Event that emits wheather a soul is available in the inventory or not
    public static event Action<bool> onSoulChange;


    public void SaveData(ref GameData data)
    {
        data.inventoryDictionary.Clear();
        var i = 0;
        foreach (var kvp in inventoryDictionary)
        {
            SerializableItem serializableItem = new SerializableItem();
            serializableItem.scriptType = kvp.Value.GetType().ToString();
            serializableItem.serializedData = JsonUtility.ToJson(kvp.Value);
            serializableItem.nbOfInstances = kvp.Value.nbOfInstances;

            data.inventoryDictionary.Add(i,serializableItem);
            i++;
        }
    }
    public void LoadData(GameData data)
    {
        inventoryDictionary.Clear();

        for (int i = 0; i < data.inventoryDictionary.Count; i++)
        {
            Type itemType = Type.GetType(data.inventoryDictionary[i].scriptType);
            //Debug.Log("itemType : " + itemType);
            //Debug.Log("subclass : " + itemType.IsSubclassOf(typeof(Item)));
            if (itemType != null && itemType.IsSubclassOf(typeof(Item)) || itemType == typeof(Item))
            {
                Item originalItem = DeserializeItem(data.inventoryDictionary[i].serializedData, itemType);
                for (int j = 0; j < data.inventoryDictionary[i].nbOfInstances; j++)
                {
                    Add(originalItem);
                }


            }
        }
    }
    private Item DeserializeItem(string jsonData, Type itemType)
    {
        Item deserializedItem = (Item)ScriptableObject.CreateInstance(itemType);
        JsonUtility.FromJsonOverwrite(jsonData, deserializedItem);
        return deserializedItem;
    }

    private void OnEnable()
    {
        Abilities.onFirstAbilityUsed += RemoveSoul;
    }
    private void OnDisable()
    {
        Abilities.onFirstAbilityUsed -= RemoveSoul;
    }

    private void Start()
    {
        nextAvailableKey = 0;
        currentInventorySize = 0;
    }
    public Item GetItemByType(Item.ItemType itemType)
    {
        foreach (var kvp in inventoryDictionary)
        {
            if (kvp.Value.itemType == itemType)
            {
                return kvp.Value;
            }
        }

        return null;
    }

    //Searchs for a soul item inside the player's inventory and removes it 
    public void RemoveSoul()
    {
        Item soul = GetItemByType(Item.ItemType.Soul);
        if (soul != null) RemoveItem(soul);
    }

    public void UseWeapon(Weapon_SO weapon)
    {
        equipementManager.EquipWeapon(weapon);
    }

    public void UsePotion(Consumable_SO potion)
    {
        onConsumableUse?.Invoke(potion);
    }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
       
    }
    public bool Add(Item item)
    {
        if (inventoryDictionary.Count == 0)
        {
            inventoryDictionary.Add(nextAvailableKey, item);
            item.nbOfInstances = 1;
            nextAvailableKey++;
            currentInventorySize++;

            // Invoke the event in case we add a soul item for the ability/abilityUI script
            if (item.itemType == Item.ItemType.Soul) onSoulChange?.Invoke(true);

            // Item added successfully
            if (onItemChanged != null)
                onItemChanged.Invoke();
            return true;
        }
        else
        {
            // Look for the itemId inside the dictionary, if found store the key's position
            bool itemFound = FindItemId(item.itemId);
            if (itemFound)
            {
                inventoryDictionary[idLocation].nbOfInstances++;
                // Item added successfully
                if (onItemChanged != null)
                    onItemChanged.Invoke();
                return true;
            }
            else
            {
                if (currentInventorySize >= inventorySize)
                {
                    // Inventory full, item not added
                    Debug.Log("Not enough room.");
                    return false;
                }
                else
                {
                    // Add later limitation on how much an item can stack
                    inventoryDictionary.Add(nextAvailableKey, item);
                    item.nbOfInstances = 1;
                    nextAvailableKey++;
                    currentInventorySize++;

                    if (item.itemType == Item.ItemType.Soul) onSoulChange?.Invoke(true);

                    // Item added successfully
                    if (onItemChanged != null)
                        onItemChanged.Invoke();
                    return true;
                }
            }
        }
    }



    private bool FindItemId(int id)
    {
        idLocation = -1;
        for (int i = 0; i < inventoryDictionary.Count; i++)
        {
            if (inventoryDictionary[i].itemId == id)
            {
                idLocation = i;
                return true;
            }
        }
        return false;
    }


    public void RemoveItem(Item item, int amount = 1)
    {
        bool itemFound = FindItemId(item.itemId);
        if (itemFound)
        {
            if (inventoryDictionary[idLocation].nbOfInstances <= amount)
            {
                // Invoke the event in case we remove a soul item for the ability/abilityUI script
                if (item.itemType == Item.ItemType.Soul)
                {
                    onSoulChange?.Invoke(false);
                }
                inventoryDictionary[idLocation].nbOfInstances = 0;

                inventoryDictionary.Remove(idLocation);
                
                //Reorganize Dictionary after removing the item from the dictionary
                ReorganizeKeys();
                currentInventorySize--;

            }
            else
            {
                inventoryDictionary[idLocation].nbOfInstances -= amount;
                
            }
            // Remove the weapon if it's equipped
            Debug.Log("item.itemType == Item.ItemType.Weapon :" + (item.itemType == Item.ItemType.Weapon));
            if (item.itemType == Item.ItemType.Weapon)
            {
                Debug.Log("current equipped weapon : " + equipementManager.GetCurrentEquippedWeapon);
                if (equipementManager.GetCurrentEquippedWeapon != null)
                {
                    if (item.itemId == equipementManager.GetCurrentEquippedWeapon.itemId)
                    {
                        equipementManager.RemoveWeapon(equipementManager.GetCurrentEquippedWeapon.type);
                    }
                }
                
            }
           
            if (onItemChanged != null)
                onItemChanged.Invoke();
        }
    }

    private void ReorganizeKeys()
    {
        Dictionary<int, Item> newInventory = new Dictionary<int, Item>();
        int newKey = 0;

        foreach (var kvp in inventoryDictionary)
        {
            newInventory[newKey] = kvp.Value;
            newKey++;
        }
        nextAvailableKey = newKey;
        inventoryDictionary = newInventory;
    }



}
