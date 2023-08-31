using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Enemy : MonoBehaviour,IDataPersistence
{

    //Added scriptable object field for the enemy when we have diferent types of enemies


    [SerializeField] private EnemyHealthBar healthBar;
    private Health enemyHealth;
    // Used to identify the enemy
    [SerializeField] private int id;
    [SerializeField] private bool killed;

    public void SaveData(ref GameData data)
    {
        if (data.enemyDictionary.ContainsKey(id))
        {
            data.enemyDictionary.Remove(id);
        }
        EnemyData enemyData = new EnemyData();
        enemyData.enemyPosition = this.transform.position;
        enemyData.enemyRotation = this.transform.eulerAngles;
        enemyData.killed = this.killed;
        data.enemyDictionary.Add(this.id,enemyData);
    }

    public void LoadData(GameData data)
    {
        //Debug.Log("enemy's id during load" + id)
        data.enemyDictionary.TryGetValue(this.id, out EnemyData enemy);
        if (enemy != null)
        {
            if (enemy.killed)
            {
                this.gameObject.SetActive(false);
            }

            this.transform.position = enemy.enemyPosition;
            this.transform.eulerAngles = enemy.enemyRotation;
            this.killed = enemy.killed;

        }
       
    }
    // Start is called before the first frame update
    void Awake()
    {
        enemyHealth = new Health(100);
    }
    public void EnemyDamaged(float damage)
    {
        // TO DO Add animation for taking damage
        enemyHealth.Damage(damage);
        //Update healthbar UI
        healthBar.UpdateHealth(enemyHealth.GetMaxHealth, enemyHealth.GetCurrentHealth);
        if (enemyHealth.GetCurrentHealth == 0)
        {
            // TO DO Add animation for death
            killed = true;
            this.gameObject.SetActive(false);
        }
    }

    public int GetEnemyHealth
    {
        get { return enemyHealth.GetCurrentHealth; }
    }
}
