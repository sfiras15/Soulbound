using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{

    public Item item;

    private InputPlayer inputPlayer;
    [SerializeField] private float pickUpDistance = 1.5f;

    private void Start()
    {
        inputPlayer = PlayerManager.instance.inputPlayer;
    }

    private void OnMouseOver()
    {
        if (inputPlayer.collectKeyPressed && Vector3.Distance(transform.position,inputPlayer.transform.position) <= pickUpDistance)
        {
            Debug.Log("item id :" + item.itemId);
            Inventory.instance.Add(item);
        }
    }
}
