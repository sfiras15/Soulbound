using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Keeps track of the items inside the chest and updates the UI 
/// </summary>
public class ChestUI : MonoBehaviour
{
    // Array for the prefabs that will initialize inside the chest
    [SerializeField] private Item[] prefabsInsideTheChest;

    //Array for the items currently inside The chest
    [SerializeField] private List<Item> itemsInsideTheChest = new List<Item>();


    [SerializeField] private ChestSlot[] chestSlots;

    private void OnEnable()
    {
        ChestSlot.onItemCollected += ArrangeItemList;
    }

    private void OnDisable()
    {
        ChestSlot.onItemCollected -= ArrangeItemList;
    }

    public void ArrangeItemList(Item item)
    {
        //This solution does not remove the exact item's place if there's duplicates of the same item inside the chest , fix later by changing itemsList to a dictionary that tracks 
        // the number of instances
        itemsInsideTheChest.Remove(item);
    }
    private void Awake()
    {
        chestSlots = GetComponentsInChildren<ChestSlot>();
        //Initialize the Items inside the chest
        for (int i = 0; i < prefabsInsideTheChest.Length; i++)
        {
            itemsInsideTheChest.Add(prefabsInsideTheChest[i]);
        }
        // Initialize The UI element for each chestSlot
        for (int j = 0; j < prefabsInsideTheChest.Length; j++)
        {
            chestSlots[j].AddItem(prefabsInsideTheChest[j]);
        }
    }
    private void Update()
    {
        if (gameObject.activeSelf)
        {
            UpdateUI();
        }
    }
    public void UpdateUI()
    {
        ChestSlot[] slots = GetComponentsInChildren<ChestSlot>();
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < itemsInsideTheChest.Count)
            {
                slots[i].AddItem(itemsInsideTheChest[i]);
            }
            else
            {
                slots[i].ClearSlot();
            }
        }
    }
}
