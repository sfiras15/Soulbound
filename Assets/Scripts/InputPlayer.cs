using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Handles various action done by the player such as attacking,dodging,locking on targets,interacting with objects
/// Stores information about health/stamina 
/// </summary>
public class InputPlayer : MonoBehaviour,IDataPersistence
{
    [Header("References")]
    
    [SerializeField] private Animator animator;
    [SerializeField] private Camera cam;
    private PlayerMovement playerMovement;

    [Header("Collecting")]
    [SerializeField] private KeyCode collectKey;
    [SerializeField] private float pickUpDistance = 1.5f;

    // Event for initializing interact key to the UI

    public static event Action<KeyCode> onInitializeUI;

    [Header("Attacking")]
    [SerializeField] private KeyCode attackKey;
    private bool attacking = false;
    private bool readyToAttack = true;

    //The size of the sphere that we are casting in the attack animation
    [SerializeField] private float hitsize = 0.12f;

    [SerializeField] private LayerMask whatIsEnemy;


    // Dodging to the mousePosition 
    [Header("Dodging")]
    [SerializeField] private KeyCode dodgeKey = KeyCode.LeftAlt;
    [SerializeField] private float timeBetweenDodges = 1f;
    [SerializeField] private bool dodging = false;
    [SerializeField] private float dodgeStaminaConsumption = 25f;
    // For the raycast to get the mousePosition 
    [SerializeField] private LayerMask whatIsGround;

    // moving the player's position through the dodge animation 
    private Vector3 endDdodgePosition;

    private Vector3 dodgeDirection;

    // To smothen the player's direction if he was locking on an enemy if he dodges
    private bool wasLockingOnEnemy;
    private Transform lastLockedOnEnemy;

    [Header("Using Item")]
    [SerializeField] private LayerMask whatIsItem;
    // affected by using potions atm
    private float damageMultiplier = 1f;
    // equipping weapons 
    [SerializeField] private EquipementManager equipementManager;

    // To prevent the player from attacking when he's browsing
    private bool isInventoryActive = false;

    [Header("LockOn Enemy")]

    [SerializeField] private KeyCode lockOnEnemyButton;
    [SerializeField] private float lockOnRadius = 7f;
    [SerializeField] private float lockOnSpeed = 5f;
    private Transform currentLockedEnemy;
    // nearby enemies ready for lock
    [SerializeField] private Collider[] CloseEnemies;

    // parameters for locking off enemies
    [SerializeField] private KeyCode lockOffEnemyButton;
    [SerializeField] private float lockOffDistance = 9f;

    [Header("Player's Health")]
    [SerializeField] private Bar playerHealthBar;
    private Health playerHealth;


    [Header("Player's Stamina")]
    [SerializeField] private Bar playerStaminaBar;
    private Stamina playerStamina;

    //Variables for stamina recovery/drain system
    [SerializeField] private float staminaRecoveryRate = 1f;
    private float lastRecovryTime;
    private bool recoveryTimerIsRunning = false;
    private bool drainTimerIsRunning = false;
    private float lastSprintTime;




    public void SaveData(ref GameData data)
    {
        data.playerPosition = this.transform.position;
        data.playerRotation = this.transform.eulerAngles;
        data.currentHealth = playerHealth.GetCurrentHealth;
        data.maxHealth = playerHealth.GetMaxHealth;
        data.currentStamina = playerStamina.GetCurrentStamina;
        data.maxStamina = playerStamina.GetMaxStamina;

    }

    public void LoadData(GameData data)
    {
        this.transform.position = data.playerPosition;
        this.transform.eulerAngles = data.playerRotation;
        playerHealth.SetCurrentHealth = data.currentHealth;
        playerHealth.SetMaxHealth = data.maxHealth;
        playerStamina.SetCurrentStamina = data.currentStamina;
        playerStamina.SetMaxStamina = data.maxStamina;
        
    }


    private void Awake()
    {
        playerHealth = new Health(100);
        playerStamina = new Stamina(100);
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        if (onInitializeUI != null) onInitializeUI?.Invoke(collectKey);

    }
    private void OnEnable()
    {
        // Event for when the player is using a consumable
        Inventory.onConsumableUse += playerState;

        // Event for when the player is taking damage
        PlayerManager.onPlayerDamaged += UpdateHealth;

        Abilities.onSecondAbilityUsed += SecondAbilityActive;
    }
    private void OnDisable()
    {
        Inventory.onConsumableUse -= playerState;
        PlayerManager.onPlayerDamaged -= UpdateHealth;
        Abilities.onSecondAbilityUsed -= SecondAbilityActive;
    }

    private void Start()
    {
        //Update the stamina/healthbar UI
        playerHealthBar.GetHealthBarText.text = playerHealth.GetCurrentHealth.ToString() + "/" + playerHealth.GetMaxHealth.ToString();
        playerHealthBar.UpdateBar(playerHealth.GetMaxHealth, playerHealth.GetCurrentHealth);
        playerStaminaBar.UpdateBar(playerStamina.GetMaxStamina, playerStamina.GetCurrentStamina);
        PlayerManager.instance.AddXp(0);
       
    }
    private void UpdateHealth(float damage)
    {
        playerHealth.Damage(damage);
        playerHealthBar.UpdateBar(playerHealth.GetMaxHealth, playerHealth.GetCurrentHealth);
        //Debug.Log(playerHealth.GetCurrentHealth);
    }
    private void SecondAbilityActive(bool state)
    {
        if (state) playerMovement.IncreaseSpeed(2f);
        else playerMovement.IncreaseSpeed(1f);

    }


    private void playerState(Consumable_SO potion)
    {
        if (potion.type == Consumable_SO.ConsumableType.Healing)
        {
            playerHealth.Heal(potion.buffValue);
            playerHealthBar.UpdateBar(playerHealth.GetMaxHealth, playerHealth.GetCurrentHealth);
            Debug.Log("healing player");
        }
        else if (potion.type == Consumable_SO.ConsumableType.MsIncrease)
        {
            Debug.Log("Ms Increased");
            StartCoroutine(IncreaseMs(potion));
        }
        else
        {
            damageMultiplier += potion.buffValue / 100f;
            Debug.Log("Damage Increased");
            Invoke(nameof(ResetDamageMultiplier), potion.duration);
            
        }
    }

    private IEnumerator IncreaseMs(Consumable_SO potion)
    {
        float msMultiplier = 1f + potion.buffValue / 100f;
        playerMovement.IncreaseSpeed(msMultiplier);
        yield return new WaitForSeconds(potion.duration);
        playerMovement.IncreaseSpeed(msMultiplier - potion.buffValue / 100f);
    }

    private void ResetDamageMultiplier()
    {
        damageMultiplier = 1f;
    }
    private void UpdateInventoryUIState(bool isActive)
    {
        isInventoryActive = isActive;
    }
    // Update is called once per frame
    void Update()
    {
        //To test xp bar , remove later
        if (Input.GetKeyDown(KeyCode.T))
        {
            PlayerManager.instance.AddXp(70);
        }
        StaminaManagement();
        if (Input.GetKeyDown(collectKey))
        {
            Collect();
        }
        if (Input.GetKeyDown(dodgeKey) && playerStamina.GetCurrentStamina >= dodgeStaminaConsumption)
        {
            Dodge();
        }
        if (InventoryUI.isInventoryActive)
        {
            // If the inventory UI is active, return early to prevent the player from attacking,locking onto enemies
            return;
        }

        if (Input.GetKeyDown(attackKey) && readyToAttack && equipementManager.GetCurrentEquippedWeapon != null 
            && playerStamina.GetCurrentStamina >= equipementManager.GetCurrentEquippedWeapon.staminaConsumption)
        {
            Attack();
        }

        if (Input.GetKeyDown(lockOnEnemyButton))
        {
            FindLockTarget();
        }
        if (currentLockedEnemy != null)
        {
            LockOnEnemy(currentLockedEnemy);
            if (Vector3.Distance(transform.position, currentLockedEnemy.position) > lockOffDistance || Input.GetKeyDown(lockOffEnemyButton) || !currentLockedEnemy.gameObject.activeSelf)
            {
                playerMovement.mousePosition = transform.position;
                playerMovement.mouseDirection = currentLockedEnemy.position - transform.position;
                playerMovement.mouseDirection.y = 0f;
                lockOffEnemy();
            }    
        }

        
    }
    // Handles the stamina recovery/drain of the player when he's walking/sprinting
    public void StaminaManagement()
    {
        if (playerMovement.state == PlayerMovement.MovementState.walking)
        {
            if (!recoveryTimerIsRunning)
            {
                recoveryTimerIsRunning = true;
                lastRecovryTime = Time.time;
            }

            if (Time.time - lastRecovryTime >= 0.5f && playerStamina.GetCurrentStamina < playerStamina.GetMaxStamina)
            {
                playerStamina.Recover(staminaRecoveryRate);
                playerStaminaBar.UpdateBar(playerStamina.GetMaxStamina, playerStamina.GetCurrentStamina);
                recoveryTimerIsRunning = false;
            }
        }
        else
        {
            recoveryTimerIsRunning = false; // Reset the recovery timer when not walking
        }

        if (playerMovement.state == PlayerMovement.MovementState.sprinting)
        {
            if (!drainTimerIsRunning)
            {
                drainTimerIsRunning = true;
                lastSprintTime = Time.time;
            }

            if (Time.time - lastSprintTime >= 0.2f)
            {
                if (playerStamina.GetCurrentStamina >= playerMovement.GetSprintConsumptionRate)
                {
                    playerStamina.Drain(playerMovement.GetSprintConsumptionRate);
                    playerStaminaBar.UpdateBar(playerStamina.GetMaxStamina, playerStamina.GetCurrentStamina);
                    drainTimerIsRunning = false;
                }         
            }            
        }
        else
        {
            drainTimerIsRunning = false; // Reset the drain timer when not sprinting
        }
    }

    private void LockOnEnemy(Transform target)
    {
        var direction = target.position - transform.position;
        direction.y = 0;
        transform.forward = Vector3.Lerp(transform.forward, direction, Time.deltaTime * lockOnSpeed); 
    }
    private void Dodge()
    {
        if (!dodging)
        {
            dodging = true;
            animator.SetTrigger("Dodging");

            //Update the player's stamina accordingly
            playerStamina.Drain(dodgeStaminaConsumption);
            playerStaminaBar.UpdateBar(playerStamina.GetMaxStamina, playerStamina.GetCurrentStamina);
        }
        StartCoroutine(ResetDodge(timeBetweenDodges));
    }

    private IEnumerator ResetDodge(float duration)
    {
        yield return new WaitForSeconds(duration);
        dodging = false;
    }
    private void Attack()
    {
        if (!attacking)
        {
            attacking = true;
            animator.SetTrigger("Attacking");
            //Pass the appropriate weaponType to the Attack blend tree to intiate the right weaponAnimation
            animator.SetFloat("WeaponType", (int)equipementManager.GetCurrentEquippedWeapon.type);

            //Update the player's stamina accordingly
            playerStamina.Drain(equipementManager.GetCurrentEquippedWeapon.staminaConsumption);
            playerStaminaBar.UpdateBar(playerStamina.GetMaxStamina, playerStamina.GetCurrentStamina);

            StartCoroutine(EndAttackMotion(equipementManager.GetCurrentEquippedWeapon.attackDuration));
        }
        playerMovement.attacking = true;
        StartCoroutine(ResetAttack(equipementManager.GetCurrentEquippedWeapon.attackDuration));
    }

    private void Collect()
    {
        Ray rayPosition = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(rayPosition, out var hitInfo, Mathf.Infinity, whatIsItem))
        {
            if (hitInfo.collider != null)
            {
                // if we found an interactable object, interact with it
                if (hitInfo.collider.TryGetComponent(out IInteractable interactable))
                {
                    if (Vector3.Distance(transform.position, hitInfo.collider.transform.position) <= pickUpDistance) interactable.Interact();
                }
            }
        }
    }

    private void FindLockTarget()
    {
        // Find enemies inside the lockOn radius
        CloseEnemies = Physics.OverlapSphere(transform.position, lockOnRadius, whatIsEnemy);
        if (CloseEnemies.Length == 0 ) return;
        
        // leave the function early if we are already locked on an enemy and he's the only one around
        if (CloseEnemies.Length == 1 && CloseEnemies[0].transform == currentLockedEnemy)
        {
            Debug.Log("already locked on that enemy");
            return;
        }

        // to prevent the player from looking at the mouseDirection instead of the enemy
        playerMovement.EnemyLocked(true);


        // Lock onto a random enemy from nearby enemies
        var randomIndex = UnityEngine.Random.Range(0, CloseEnemies.Length);

        if (CloseEnemies[randomIndex].transform == currentLockedEnemy)
        {
            randomIndex++;
            if (randomIndex >= CloseEnemies.Length)
            {
                randomIndex = 0;
            }
        }
        //Debug.Log("randomIndex after check " + randomIndex);

        currentLockedEnemy = CloseEnemies[randomIndex].transform;
    }

    private void lockOffEnemy()
    {
        currentLockedEnemy = null;

        playerMovement.EnemyLocked(false);
    }
    private IEnumerator ResetAttack(float duration)
    {
        readyToAttack = false;
        //AttackTime
        yield return new WaitForSeconds(duration);
        readyToAttack = true;

    }
    private IEnumerator EndAttackMotion(float duration)
    {
        //AttackTime
        yield return new WaitForSeconds(duration);
        attacking = false;
        playerMovement.attacking = false;
    }


    //Function inside the attackAnimation
    public void StartAttack()
    {
        Physics.SphereCast(transform.position + Vector3.up, hitsize, transform.forward,out var hitInfo, equipementManager.GetCurrentEquippedWeapon.attackRange,whatIsEnemy);
        if (hitInfo.collider != null)
        {
            Enemy enemy = hitInfo.collider.GetComponent<Enemy>();
            if (enemy !=null)
            {
                enemy.EnemyDamaged(equipementManager.GetCurrentEquippedWeapon.damage * damageMultiplier);
                if (enemy.GetEnemyHealth == 0)
                {
                    if (enemy.transform == currentLockedEnemy)
                    {
                        lockOffEnemy();
                    }
                }
            }
        }
        
    }

    // Function inside the dodge animation
    public void StartDodge()
    {
        // to prevent the player from dodging towards the enemy instead of the mouseDirection
        if (currentLockedEnemy != null) 
        {
            wasLockingOnEnemy = true;
            lastLockedOnEnemy = currentLockedEnemy;
            lockOffEnemy();
        } 

        // to prevent the player from walking towards the new mousePosition since we are changing it in this script
        playerMovement.enabled = false;

        // So the player can move with the animation
        animator.applyRootMotion = true;

        // Dodge towards the mousePosition
        Ray rayPosition = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(rayPosition, out var hitInfo, Mathf.Infinity, whatIsGround))
        {

            var mousePosition = hitInfo.point;
            dodgeDirection = mousePosition - transform.position;
            dodgeDirection.y = 0f;
            //Debug.Log(dodgeDirection);
            transform.forward = Vector3.Lerp(transform.forward, dodgeDirection, Time.deltaTime * 50f);
        }
       
    }
    // Function inside the dodge animation
    public void EndDodge()
    {
        animator.applyRootMotion = false;
        if (wasLockingOnEnemy && Vector3.Distance(transform.position, lastLockedOnEnemy.position) <= lockOffDistance)
        {
            playerMovement.EnemyLocked(true);
            LockOnEnemy(lastLockedOnEnemy);
            currentLockedEnemy = lastLockedOnEnemy;
            wasLockingOnEnemy = false;
        }
        playerMovement.enabled = true;

        // to prevent the player from moving randomly after dodging
        playerMovement.mousePosition = transform.position;
        playerMovement.mouseDirection = dodgeDirection;
    }


    public Stamina GetPlayerStamina
    {
        get { return playerStamina; }
    }
    public Transform GetCurrentLockedEnemy
    {
        get {
            return currentLockedEnemy;
        }
    }

    // Initialize player info for loading the game for the first time
    public PlayerData playerInfo()
    {
        PlayerData playerData = new PlayerData();
        playerData.playerPosition = transform.position;
        playerData.playerRotation = transform.eulerAngles;
        playerData.currentHealth = playerHealth.GetCurrentHealth;
        playerData.maxHealth = playerHealth.GetMaxHealth;
        playerData.currentStamina = playerStamina.GetCurrentStamina;
        playerData.maxStamina = playerStamina.GetMaxStamina;
        return playerData;
    }

}
[System.Serializable]
public class PlayerData
{
    public Vector3 playerPosition;
    public Vector3 playerRotation;
    public int currentHealth;
    public int maxHealth;
    public int currentStamina;
    public int maxStamina;
}
