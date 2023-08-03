using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Attached to the Inventory GameObject.Handles the inventory UI logic.

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private StarterAssetsInputs starterAssetsInputs;
    [SerializeField] private ThirdPersonController thirdPersonController;
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
        if (Input.GetKeyDown(inventoryKey))
        {
            if (!inventoryUI.activeSelf)
            {
                // unlock cursor / lock position via the scripts attached to the player
                starterAssetsInputs.SetCursorState(false);
                thirdPersonController.LockCameraPosition = true;
                inventoryUI.SetActive(true);
                UpdateUI();
            }
            else
            {
                starterAssetsInputs.SetCursorState(true);
                thirdPersonController.LockCameraPosition = false;
                inventoryUI.SetActive(false);
            }
            
        }
    }
    public void UpdateUI()
    {
        InventorySlot[] slots = GetComponentsInChildren<InventorySlot>();
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < Inventory.instance.items.Count)
            {
                slots[i].AddItem(Inventory.instance.items[i]);
            }
            else
            {
                slots[i].ClearSlot();
            }
        }
    }
}
