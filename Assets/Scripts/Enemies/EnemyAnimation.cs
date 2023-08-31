using System;
using UnityEngine;


public class EnemyAnimation : MonoBehaviour
{
    [SerializeField] private EnemyAi enemyAi;
    private Animator animator;

    private float enemyDamage;
    private float enemyAttackSize;
    private float enemyRange;
    private LayerMask playerLayer;
    private bool attackState;
    private float enemySpeed;

       private void Awake()
    {
        animator = GetComponent<Animator>();
    }


    public void EnemyStats(float damage,float attackSize,float range,LayerMask playerLayer)
    {
        enemyDamage = damage;
        enemyAttackSize = attackSize;
        enemyRange = range;
        this.playerLayer = playerLayer;
    }

    public void EnemyAttackState(bool attackState)
    {
        this.attackState = attackState;
    }
    public void EnemySpeed(float speed)
    {
        enemySpeed = speed;

    }
    // Update is called once per frame

    void Update()
    {    
        if (attackState)
        {
            animator.SetTrigger("Attack");
        }
        animator.SetFloat("Speed", enemySpeed);
    }

    public void StartEnemyAttack()
    {
        Physics.SphereCast(transform.position + Vector3.up, enemyAttackSize, transform.forward, out var hitInfo, enemyRange, playerLayer);
        if (hitInfo.collider != null)
        {
           if (hitInfo.collider.CompareTag("Player"))
           {
                //
                PlayerManager.instance.DamagePlayer(enemyDamage);
           }
           

        }
    }

    
}
