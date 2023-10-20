using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Class for serializing information about the chests in the game
[System.Serializable]
public class ChestData
{
    public Vector3 chestPosition;
    public Vector3 chestRotation;
    public List<Item> items;
    //public bool collected;
}
