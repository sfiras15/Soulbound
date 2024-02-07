using UnityEngine;


[CreateAssetMenu(fileName = "ScriptableObjects/Items", menuName = "Items/Item")]

public class Item : ScriptableObject
{
    public string itemName;
    public int nbOfInstances;
    public int itemId;
    public Sprite itemIcon;
    public bool usable;
    public ItemType itemType;
    public enum ItemType
    {
        Soul,
        Artifact,
        Weapon,
        Consumable
    }
    
    public virtual void Use()
    {
        Debug.Log("item");
    }
}
