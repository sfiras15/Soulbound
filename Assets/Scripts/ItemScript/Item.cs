using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "Items" , menuName = "Items/Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public int nbOfInstances = 1;
    public int itemId;
    public Sprite itemIcon;
    public bool usable;

    public virtual void Use()
    {
       
    }
}
