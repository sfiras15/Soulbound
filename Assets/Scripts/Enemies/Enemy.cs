using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    //Added scriptable object field for the enemy when we have diferent types of enemies


    [SerializeField] private EnemyHealthBar healthBar;
    private Health enemyHealth;
    // Used to identify the enemy
    // Start is called before the first frame update
    void Awake()
    {
        enemyHealth = new Health(100);
    }
    private void Update()
    {
        // Kill the enemy , play deathAnimation
        if (enemyHealth.GetCurrentHealth == 0)
        {
            gameObject.SetActive(false);
        }
    }
    public void EnemyDamaged(float damage)
    {

        enemyHealth.Damage(damage);
        //Update healthbar UI
        healthBar.UpdateHealth(enemyHealth.GetMaxHealth, enemyHealth.GetCurrentHealth);
    }

    public int GetEnemyHealth
    {
        get { return enemyHealth.GetCurrentHealth; }
    }
}
