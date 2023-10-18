using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Keeps track of enemies in combat
/// </summary>
public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;
    [SerializeField] private Enemy[] enemies;
    public int enemiesInCombat;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }

        enemies = FindObjectsOfType<Enemy>();
        enemiesInCombat = 0;
    }
}
