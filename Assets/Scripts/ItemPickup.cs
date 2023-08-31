using UnityEngine;


public class ItemPickup : MonoBehaviour,IDataPersistence
{
    [SerializeField] private int id;
    [SerializeField] private Item item;
    [SerializeField] private float pickUpDistance = 1.5f;
    private bool collected;


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
        data.itemDictionary.Add(this.id, itemData);
    }
    public void LoadData(GameData data)
    {
        data.itemDictionary.TryGetValue(this.id, out ItemData item);
        if (item != null)
        {
            if (item.collected)
            {
                this.gameObject.SetActive(false);
            }

            this.transform.position = item.itemPosition;
            this.transform.eulerAngles = item.itemRotation;
            this.collected = item.collected;

        }
    }

    public void CollectItem()
    {
        Inventory.instance.Add(item);
        collected = true;
        this.gameObject.SetActive(false);
    }
    public float GetPickUpDistance
    {
        get { return pickUpDistance; }
    }


}
