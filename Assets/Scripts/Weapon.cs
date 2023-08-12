using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Weapon_SO item;

    public BoxCollider weaponCollider;
    int id = 0;

    private void Awake()
    {
        weaponCollider = GetComponent<BoxCollider>();
        id = (int)item.type;
    }

    private void Start()
    {
        weaponCollider.enabled = false;

    }

    //Bugged atm needs fixing
    private void OnTriggerEnter(Collider other)
    {
       // Debug.Log(other.name);
        Enemy enemy = other.gameObject.GetComponent<Enemy>();
        if (enemy != null)
        {
            Debug.Log(enemy.id);
            enemy.EnemyDamaged();

        }
    }

}
