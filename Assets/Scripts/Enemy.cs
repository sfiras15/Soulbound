using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private HealthBar healthBar;
    private Health enemyHealth;
    // Used to identify the enemy for the onHittingEnemy event
    public int id;
    //private void OnEnable()
    //{
    //    PlayerManager.onHittingEnemy += EnemyDamaged;
    //}
    //private void OnDisable()
    //{
    //    PlayerManager.onHittingEnemy -= EnemyDamaged;
    //}
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
    public void EnemyDamaged()
    {

            enemyHealth.Damage(PlayerManager.instance.inputPlayer.weapon.item.damage);
            //Update healthbar UI
            healthBar.UpdateHealth(enemyHealth.GetMaxHealth, enemyHealth.GetCurrentHealth);
         
    }
}
