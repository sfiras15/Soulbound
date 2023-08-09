using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Attached to the Inventory GameObject.Handles the inventory UI logic.

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private KeyCode inventoryKey;
    [SerializeField] private GameObject inventoryUI;
    private void OnEnable()
    {
        Inventory.onItemChanged += UpdateUI;
    }
    private void OnDisable()
    {
        Inventory.onItemChanged -= UpdateUI;
    }
    private void Update()
    {

        // fix a bug where the player can still attack while browsing the inventory
        if (Input.GetKeyDown(inventoryKey))
        {
            if (!inventoryUI.activeSelf)
            {
                inventoryUI.SetActive(true);
                UpdateUI();
            }
            else
            {
                inventoryUI.SetActive(false);
            }
        }
    }

    public void UpdateUI()
    {
        InventorySlot[] slots = GetComponentsInChildren<InventorySlot>();
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < Inventory.instance.inventoryDictionary.Count)
            {
                slots[i].AddItem(Inventory.instance.inventoryDictionary[i]);
            }
            else
            {
                slots[i].ClearSlot();
            }
        }
    }
}
