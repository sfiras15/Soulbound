using System;
using UnityEngine;


public class ItemPickup : MonoBehaviour,IDataPersistence, IInteractable
{
    [SerializeField] private int id;
    [SerializeField] private Item item;

    private bool collected;

    private bool despawnTimerOn;
    [SerializeField] private float lastSpawnerTime;
    [SerializeField] private float despawnTime = 10;

    private InteractableUI interactableUI;


    //Event to update the UI in case we are trying to pick an item and the inventory is full
    public static event Action<bool> onErrorPickUp;
    public void SaveData(ref GameData data)
    {
        if (data.itemDictionary.ContainsKey(id))
        {
            data.itemDictionary.Remove(id);
        }
        ItemData itemData = new ItemData();

        itemData.itemPosition = this.transform.position;
        itemData.itemRotation = this.transform.eulerAngles;
        itemData.collected = this.collected;
        data.itemDictionary.Add(id, itemData);
    }
    public void LoadData(GameData data)
    {
        data.itemDictionary.TryGetValue(id, out ItemData item);
        if (item != null)
        {
            this.transform.position = item.itemPosition;
            this.transform.eulerAngles = item.itemRotation;
            this.collected = item.collected;
            if (item.collected)
            {
                this.gameObject.SetActive(false);
            }
            else
            {
                this.gameObject.SetActive(true);
            }

        }
    }
    private void Awake()
    {
        interactableUI = GetComponentInChildren<InteractableUI>();
    }
    private void Start()
    {
        interactableUI.InitializeItemName(item.itemName.ToString());
    }
    private void Update()
    {
        if (gameObject.activeSelf)
        {
            if (!despawnTimerOn)
            {
                despawnTimerOn = true;
                lastSpawnerTime = Time.time;
            }
            
            if (Time.time - lastSpawnerTime > despawnTime && despawnTimerOn)
            {
                
                collected = true;
                this.gameObject.SetActive(false);
                
            }

        }
        else
        {
            despawnTimerOn = false;
        }
    }

    public void ShowUI()
    {
        interactableUI.Show();
    }
    public void HideUI()
    {
        interactableUI.Hide();
    }
    public void Interact()
    {
        CollectItem();
    }
    public void CollectItem()
    {
        collected = Inventory.instance.Add(item);
        if (collected) this.gameObject.SetActive(false);
        else
        {
            onErrorPickUp?.Invoke(true);
            Invoke(nameof(ResetErrorUI), 1.5f);
            interactableUI.ShowError("Inventory Full.");
        }

    }
    public void ResetErrorUI()
    {
        interactableUI.HideError();
        
    }

    // Initialize Item info for loading the game for the first time
    public ItemData ItemInfo()
    {
        ItemData itemData = new ItemData();
        itemData.itemPosition = transform.position;
        itemData.itemRotation = transform.eulerAngles;
        itemData.collected = collected;

        return itemData;

    }
    public int GetItemId
    {
        get { return id; }  

    }

}
