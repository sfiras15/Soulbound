using UnityEngine;


public class EnemyAnimation : MonoBehaviour
{
    [SerializeField] private EnemyAi enemyAi;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }


    // Update is called once per frame

    void Update()
    {    
        if (enemyAi.GetAttackState)
        {
            animator.SetTrigger("Attack");
        }
        animator.SetFloat("Speed", enemyAi.GetAgent.velocity.magnitude);
    }

    public void StartEnemyAttack()
    {
        Physics.SphereCast(transform.position + Vector3.up, enemyAi.GetAttackSize, transform.forward, out var hitInfo, enemyAi.GetAttackRange, enemyAi.GetPlayerLayer);
        if (hitInfo.collider != null)
        {
           if (hitInfo.collider.CompareTag("Player"))
           {
                Debug.Log("ouch");
           }
           

        }
    }
}
