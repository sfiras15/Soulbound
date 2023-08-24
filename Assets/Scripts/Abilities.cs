using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Abilities : MonoBehaviour
{
    [Header("FirstAbility")]
    [SerializeField] private KeyCode firstAbilityKey;
    //The duration of the ability's animation
    [SerializeField] private float firstAbilityDuration;

    [SerializeField] private float firstAbilityCooldown;
    [SerializeField] private bool firstAbilityOnCD = false;
    [SerializeField] private bool alreadyAttacking = false;

    // variable for better hit detection of the beam ability 
    [SerializeField] private float beamAngleThreshold = 15f;
    [SerializeField] private Animator animator;
    [SerializeField] private ParticleSystem beamPS;
    [SerializeField] private float attackDistance = 5f;
    [SerializeField] private float attackDamage = 50f;

    private PlayerMovement playerMovement;

    private bool isInventoryActive;
    [SerializeField] private LayerMask whatIsEnemy;

    [SerializeField] private AbilityUI abilityUI;

    //Check if the player has a soul in his inventory or not
    private bool soulActive;

    // Event called for when the player's uses their firstAbility
    public static event Action onFirstAbilityUsed;

    public static event Action<KeyCode> onGameStart;

    private void OnEnable()
    {
        Inventory.onSoulChange += SoulState;
        InventoryUI.onInventoryUIStateChanged += UpdateInventoryUIState;

    }
    private void OnDisable()
    {
        Inventory.onSoulChange -= SoulState;
        InventoryUI.onInventoryUIStateChanged -= UpdateInventoryUIState;

    }

    private void UpdateInventoryUIState(bool isActive)
    {
        isInventoryActive = isActive;
    }

    public void SoulState(bool value)
    {
        soulActive = value;
    }
    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        onGameStart?.Invoke(firstAbilityKey);
    }

    // Update is called once per frame
    void Update()
    {
        if (isInventoryActive)
        {
            // If the inventory UI is active, return early to prevent the player from attacking,locking onto enemies
            return;
        }
        if (Input.GetKeyDown(firstAbilityKey) && soulActive)
        {
            FirstAbility();      
        }

    }

    private void FirstAbility()
    {
        if (!alreadyAttacking && !firstAbilityOnCD)
        {
            alreadyAttacking = true;
            animator.SetTrigger("Ability1");

            playerMovement.enabled = false;
            
            StartCoroutine(ResetState(firstAbilityDuration));
            Debug.Log("Ability used");
        }
     
    }
    private IEnumerator ResetState(float duration)
    {
        yield return new WaitForSeconds(duration);
        alreadyAttacking = false;
        playerMovement.enabled = true;
        beamPS.gameObject.SetActive(false);
        StartCoroutine(StartCooldown(firstAbilityCooldown)); // Start cooldown coroutine here

    }
    private IEnumerator StartCooldown(float cooldownDuration)
    {
        // Wait for the attack animation to finish

        firstAbilityOnCD = true;
        float cooldownTimer = cooldownDuration;

        while (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;

            // Update the ability UI directly
            abilityUI.UpdateCooldown(cooldownTimer, cooldownDuration);

            yield return null;
        }

        firstAbilityOnCD = false;

        // Update the ability UI directly to show the cooldown ended
        abilityUI.UpdateCooldown(0, cooldownDuration);


        //Invoke the event to remove the soul from the inventory
        onFirstAbilityUsed?.Invoke();
    }

    //Function that is called inside the firstAbility animation

    //Fires a beam to where the player is looking,any enemies on that direction will be hit
    private void StartBeam()
    {
        
        beamPS.gameObject.SetActive(true);

        Vector3 beamDirection = transform.forward;
        Collider[] enemiesHit = Physics.OverlapSphere(transform.position, attackDistance, whatIsEnemy);

        foreach (Collider enemyCollider in enemiesHit)
        {
            //Debug.Log("Enemy[i] : " + enemyCollider);
            Vector3 enemyDirection = (enemyCollider.transform.position - transform.position).normalized;
            //Debug.Log("enemyDirection[i] : " + enemyDirection);
            float angle = Vector3.Angle(beamDirection, enemyDirection);
            //Debug.Log("angle[i] : " + angle);

            if (angle < beamAngleThreshold)
            {
                // The enemy is within the beam's angle threshold
                Enemy enemy = enemyCollider.GetComponent<Enemy>();
                if (enemy != null)
                {
                    // Damage the enemy
                    enemy.EnemyDamaged(attackDamage);
                    // fix locking off enemies if the beam kills , add event for killing enemies eventually
                }
            }
        }
    }


}