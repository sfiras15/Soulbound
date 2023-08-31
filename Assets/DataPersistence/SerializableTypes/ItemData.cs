using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Class for serializing information about the items in the game
[System.Serializable]
public class ItemData
{
    public Vector3 itemPosition;
    public Vector3 itemRotation;
    public bool collected;
}
