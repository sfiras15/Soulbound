using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Weapon_SO item;

    public BoxCollider weaponCollider;

    private void Awake()
    {
        weaponCollider = GetComponent<BoxCollider>();
    }

    private void Start()
    {
        weaponCollider.enabled = false;
    }

    //Bugged atm needs fixing

    private void OnCollisionEnter(Collision collision)
    {
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();
        if (enemy != null)
        {
            Debug.Log(enemy.id);
            PlayerManager.instance.HitEnemy(enemy.id);
        }
    }
}
