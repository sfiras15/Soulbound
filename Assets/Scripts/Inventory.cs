using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Game Manager.Handles the inventory & the items picked up/crafted 

public class Inventory : MonoBehaviour
{
    public static Inventory instance;
    [SerializeField] private int inventorySize;

    public static event Action onItemChanged;
    public static event Action onItemUsed;
    
    public List<Item> items = new List<Item>();


    public void UseItem(Item item)
    { 
        if (onItemUsed != null)
            onItemUsed.Invoke();

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
        if (items.Count >= inventorySize)
        {
            Debug.Log("Not enough room.");
            return;
        }

        items.Add(item);

        if (onItemChanged != null)
            onItemChanged.Invoke();
    }
    public void RemoveItem(string itemName,int amount = 1)
    {
        for(int i = 0; i < items.Count; i++)
        {
            if (items[i].itemName == itemName)
            {
                if (amount  >= 1)
                {
                    items.Remove(items[i]);
                    amount--;
                }
            }
                
        }
        

        if (onItemChanged != null)
            onItemChanged.Invoke();
    }
    public int itemAmount(Item item,string itemName)
    {
        int amount = 0;
        for (int i = 0; i < items.Count; i++)
        {
            if (item.itemName == items[i].itemName)
            {
                amount++;
            }
        }
        return amount;
    }


}
