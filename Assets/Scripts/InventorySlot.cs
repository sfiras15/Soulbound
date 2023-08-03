using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//Attached to the inventory Slot. each slot store information about the item to properly render it on the menu.

public class InventorySlot : MonoBehaviour
{
    [SerializeField] private Image itemButtonIcon;
    [SerializeField] private Button exitButton;
    Item item;
    public void AddItem(Item newItem)
    {
        item = newItem;
        itemButtonIcon.sprite = newItem.itemIcon;
        itemButtonIcon.enabled = true;
        exitButton.interactable = true;
    }
    public void ClearSlot()
    {
        item = null;
        itemButtonIcon.sprite = null;
        itemButtonIcon.enabled = false;
        exitButton.interactable = false;
    }
    public void RemoveFromInventory()
    {
        Inventory.instance.RemoveItem(item.itemName);
    }
    public void UseItem()
    {
        if (item != null)
        {
            item.Use();
        }
    }
}
