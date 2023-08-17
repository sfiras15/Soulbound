using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
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

    private void Start()
    {
        nextAvailableKey = 0;
        currentInventorySize = 0;
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
    public void Add(Item item)
    {

        if (inventoryDictionary.Count == 0)
        {
            inventoryDictionary.Add(nextAvailableKey, item);
            item.nbOfInstances = 1;
            nextAvailableKey++;
            currentInventorySize++;
        }
        else
        {
            // look for the itemId inside the dictionary , if found store the key's position
            bool itemFound = FindItemId(item.itemId);
            if (itemFound)
            {
                inventoryDictionary[idLocation].nbOfInstances++;
            }
            else
            {
                if (currentInventorySize >= inventorySize)
                {
                    Debug.Log("Not enough room.");
                    return;
                }
                else
                {
                    // add later limitation on how much an item can stack
                    inventoryDictionary.Add(nextAvailableKey, item);
                    item.nbOfInstances = 1;
                    nextAvailableKey++;
                    currentInventorySize++;
                }
               
            }
        }

        if (onItemChanged != null)
            onItemChanged.Invoke();
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
