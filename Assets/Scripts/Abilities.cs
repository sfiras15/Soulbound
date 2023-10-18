using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Handles the Ability casting logic for the player
/// Updating the UI element for each ability
/// </summary>
public class Abilities : MonoBehaviour
{
    [Header("FirstAbility")]
    [SerializeField] private KeyCode firstAbilityKey;
    //The duration of the ability's animation
    [SerializeField] private float firstAbilityDuration;

    [SerializeField] private float firstAbilityCooldown;

    //to prevent player from spamming the ability mid animation
    [SerializeField] private bool alreadyAttacking = false;


    [SerializeField] private ParticleSystem beamPS;
    [SerializeField] private float attackDistance = 5f;
    [SerializeField] private float attackDamage = 50f;

    [Header("SecondAbility")]
    [SerializeField] private KeyCode secondAbilityKey;
    //The duration of the ability's animation
    [SerializeField] private float secondAbilityDuration;

    [SerializeField] private float secondAbilityEffectDuration;

    [SerializeField] private float secondAbilityCooldown;

    //Particles used for the secondAbility animation
    [SerializeField] private ParticleSystem powerDraw;
    [SerializeField] private ParticleSystem groundBlast;
    [SerializeField] private ParticleSystem energyNova;
    [SerializeField] private ParticleSystem aura;

    //to prevent player from spamming the ability mid animation
    private bool secondAbilityUsed = false;

    private PlayerMovement playerMovement;


    //Used for detecting enemies for the first ability
    [SerializeField] private LayerMask whatIsEnemy;

    [SerializeField] private Animator animator;
    [SerializeField] private AbilityUI abilityUI;


    // Matchs the ability with it's respective cooldown bool
    private Dictionary<int, bool> abilityCooldowns = new Dictionary<int, bool>();


    //Check if the player has a soul in his inventory or not
    private bool soulActive;

    // Event called for when the player's uses their firstAbility
    public static event Action onFirstAbilityUsed;

    // Event called for when the player's uses their firstAbility
    public static event Action<bool> onSecondAbilityUsed;


    //Event called to update the UI text of the abilities
    public static event Action<KeyCode,KeyCode> onGameStart;

    private void OnEnable()
    {
        Inventory.onSoulChange += SoulState;
    }
    private void OnDisable()
    {
        Inventory.onSoulChange -= SoulState;

    }

    public void SoulState(bool value)
    {
        soulActive = value;
    }
    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        onGameStart?.Invoke(firstAbilityKey, secondAbilityKey);
        abilityCooldowns.Add(1, false); // First ability
        abilityCooldowns.Add(2, false); // Second ability
    }

    // Update is called once per frame
    void Update()
    {
        if (InventoryUI.isInventoryActive)
        {
            // If the inventory UI is active, return early to prevent the player from Casting abilties
            return;
        }
        if (Input.GetKeyDown(firstAbilityKey) && soulActive && !secondAbilityUsed && !playerMovement.jumping)
        {
            FirstAbility();      
        }

        if (Input.GetKeyDown(secondAbilityKey)  && !alreadyAttacking && playerMovement.GetLockOnEnemy == null && !playerMovement.jumping)
        {
            SecondAbility();
        }

    }

    private void FirstAbility()
    {
        // If the player is not already casting the ability and the ability is not on cooldown
        if (!alreadyAttacking && !abilityCooldowns[1])
        {
            // to prevent the player from casting another spell mid animation
            alreadyAttacking = true;

            animator.SetTrigger("Ability1");

            playerMovement.enabled = false;
            
            StartCoroutine(ResetState(firstAbilityDuration));
        }
     
    }
    private void SecondAbility()
    {
        // If the player is not already casting the ability and the ability is not on cooldown
        if (!abilityCooldowns[2] && !secondAbilityUsed)
        {
            abilityCooldowns[2] = true;
            // to prevent the player from casting another spell mid animation
            secondAbilityUsed = true;

            //Start the animation/ particle effect related to the ability
            powerDraw.gameObject.SetActive(true);
            animator.SetTrigger("Ability2");
            playerMovement.enabled = false;

            StartCoroutine(ResetSecondAbilityEvent(secondAbilityDuration));
         
        }

    }

    //Called mid animation of the second Ability
    private void PowerRelease()
    {
        powerDraw.gameObject.SetActive(false);
        energyNova.gameObject.SetActive(true);
        groundBlast.gameObject.SetActive(true);
    }
    private IEnumerator ResetSecondAbilityEvent(float duration)
    {
        yield return new WaitForSeconds(duration);
        onSecondAbilityUsed?.Invoke(true);
        secondAbilityUsed = false;
        playerMovement.enabled = true;
        aura.gameObject.SetActive(true);
        groundBlast.gameObject.SetActive(false);
        energyNova.gameObject.SetActive(false);

        //Disable particles of the effect
        Invoke(nameof(DisableEffect), secondAbilityEffectDuration);

        //Starts counting the cooldown once the animation of the ability ends
        SecondAbilityCooldown(secondAbilityCooldown);

    }
    private void DisableEffect()
    {
        aura.gameObject.SetActive(false);
        onSecondAbilityUsed?.Invoke(false);
       
    }
    private void SecondAbilityCooldown(float cooldownDuration)
    {
        StartCoroutine(StartTimer(cooldownDuration, 2));
    }
    private IEnumerator ResetState(float duration)
    {
        yield return new WaitForSeconds(duration);
        alreadyAttacking = false;
        playerMovement.enabled = true;
        beamPS.gameObject.SetActive(false);
        FirstAbilityCooldown(firstAbilityCooldown);
    }
    private void FirstAbilityCooldown(float cooldownDuration)
    {
        //Invoke the event to remove the soul from the inventory
        onFirstAbilityUsed?.Invoke();
        StartCoroutine(StartTimer(cooldownDuration, 1));
    }
   

    //Updates the cooldown of the used ability and it's corresponding UI element
    private IEnumerator StartTimer(float cooldownDuration,int abilityIndex)
    {
        abilityCooldowns[abilityIndex] = true;
        float cooldownTimer = cooldownDuration;
        while (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
            // Update the ability UI directly
            abilityUI.UpdateCooldown(cooldownTimer, cooldownDuration, abilityIndex);

            yield return null;
        }
        // Update the ability UI directly to show the cooldown ended
        abilityUI.UpdateCooldown(0, cooldownDuration, abilityIndex);
        abilityCooldowns[abilityIndex] = false;
    }

    //Function that is called inside the firstAbility animation

    //Fires a beam in front of the player to where he's looking,any enemies on that direction will be hit
    private void StartBeam()
    {
        
        beamPS.gameObject.SetActive(true);

        // Define the size of the overlap box
        Vector3 boxSize = new Vector3(0.5f, 0.5f, attackDistance * 0.5f);

        // Calculate the position for the overlap box
        Vector3 boxCenter = transform.position + Vector3.up + transform.forward * (attackDistance * 0.5f);
        // Get all colliders within the overlap box
        Collider[] hitColliders = Physics.OverlapBox(boxCenter, boxSize, transform.rotation, whatIsEnemy);

        // Loop through the colliders and damage enemies
        foreach (Collider collider in hitColliders)
        {
            Enemy enemy = collider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.EnemyDamaged(attackDamage);
            }
        }
    }


}