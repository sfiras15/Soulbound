using System.Collections;
using UnityEngine;

// Handles the different inputs and states of the player to trigger the animations

public class InputPlayer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private Animator animator;

    [Header("Collecting")]
    [SerializeField] private KeyCode collectKey;
    public bool collectKeyPressed;

    [Header("Attacking")]
    [SerializeField] private KeyCode attackKey;
    private bool attacking = false;
    private bool readyToAttack = true;
    public Weapon weapon;

    //[Header("Using item")]
    public bool usingItem;
    
    private void Awake()
    {
        animator = GetComponent<Animator>();
        weapon = GetComponentInChildren<Weapon>();
        //playerMovement = GetComponent<PlayerMovement>();
    }
    private void OnEnable()
    {
        Inventory.onItemUsed += PlayerState;
    }
    private void OnDisable()
    {
        Inventory.onItemUsed -= PlayerState;
    }
    public void PlayerState()
    {
        usingItem = true;
        
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(collectKey))
        {
            collectKeyPressed = true;
        }
        else
        {
            collectKeyPressed = false;
        }

        if (Input.GetKeyDown(attackKey) && readyToAttack)
        {
            if (!attacking)
            {      
                attacking = true;
                // Set trigger depending on the equiped weapon for now its just axe
                animator.SetTrigger("Attack");
                StartCoroutine(EndAttackMotion());
            }
            playerMovement.attacking = true;
            StartCoroutine(ResetAttack());
        }
    }

    private IEnumerator ResetAttack()
    {
        readyToAttack = false;
        yield return new WaitForSeconds(1.3f);
        readyToAttack = true;

    }
    private IEnumerator EndAttackMotion()
    {
        yield return new WaitForSeconds(1.3f);
        attacking = false;
        playerMovement.attacking = false;
    }


    //Enable /Disable collider inside the attacking animation
    public void StartAttack()
    {
        weapon.weaponCollider.enabled = true;
    }
    public void EndAttack()
    {
        weapon.weaponCollider.enabled = false;

    }
}
