using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ItemPickup : MonoBehaviour
{

    [SerializeField] private Item item;
    [SerializeField] private float pickUpDistance = 1.5f;

    public float GetPickUpDistance
    {
        get { return pickUpDistance; }
    }

    public Item GetItem
    {
        get { return item; }
    }


}
