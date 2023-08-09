using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Items/Consumables", menuName = "Items/Consumable", order = 2)]
public class Consumable_SO : Item
{
    public enum ConsumableType
    {
        Healing,
        DamageIncrease,
        MsIncrease,
    }

    public int duration;
    public ConsumableType type;
    public int level;

    public override void Use()
    {
        base.Use();
        //Drink Consumable
    }

}
