using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Attached to each flower/mushroom/potion. Stores item properties and Use function
public class Item : MonoBehaviour
{
    public string itemName;
    public Sprite itemIcon;
    public bool usable;

    public void Use()
    {
        if (usable)
        {
            Inventory.instance.UseItem(this);
            Inventory.instance.RemoveItem(this.itemName);
        }
    }
}
