using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Health System for the enemies and the player eventually
public class Health
{
    private int currentHealth;
    private int maxHealth;

    public Health(int value)
    {
        this.maxHealth = value;
        this.currentHealth = maxHealth;
    }
    public int GetCurrentHealth
    {
        get { return currentHealth; }
    }
    public int GetMaxHealth
    {
        get { return maxHealth; }
    }
    public void Damage(int amount)
    {
        this.currentHealth -= amount;
        if (this.currentHealth < 0 ) this.currentHealth = 0;
    }
}
