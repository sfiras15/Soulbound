using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;


public class InputPlayer : MonoBehaviour
{
    [Header("References")]
    
    [SerializeField] private Animator animator;
    [SerializeField] private Camera cam;
    private PlayerMovement playerMovement;

    [Header("Collecting")]
    [SerializeField] private KeyCode collectKey;
    public bool collectKeyPressed;

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
    [SerializeField] bool dodging = false;
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
    public Transform currentLockedEnemy;
    // nearby enemies ready for lock
    [SerializeField] private Collider[] CloseEnemies;

    // parameters for locking off enemies
    [SerializeField] private KeyCode lockOffEnemyButton;
    [SerializeField] private float lockOffDistance = 9f;

    
   
    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }


    private void OnEnable()
    {
        // Event for checking if the player is using the UI or not so we remove some functionalities
        InventoryUI.onInventoryUIStateChanged += UpdateInventoryUIState;

        // Event for when the player is using a consumable
        Inventory.onConsumableUse += playerState;
    }
    private void OnDisable()
    {
        InventoryUI.onInventoryUIStateChanged -= UpdateInventoryUIState;
        Inventory.onConsumableUse -= playerState;
    }

    private void playerState(Consumable_SO potion)
    {
        if (potion.type == Consumable_SO.ConsumableType.Healing)
        {
            Debug.Log("healing player");
            //heal player
            // add player's healthbar later
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
        if (Input.GetKeyDown(collectKey))
        {
            Collect();
        }
        if (Input.GetKeyDown(dodgeKey))
        {
            Dodge();
        }
        if (isInventoryActive)
        {
            // If the inventory UI is active, return early to prevent the player from attacking,locking onto enemies
            return;
        }

        if (Input.GetKeyDown(attackKey) && readyToAttack && equipementManager.GetCurrentEquippedWeapon != null)
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
            if (Vector3.Distance(transform.position, currentLockedEnemy.position) > lockOffDistance || Input.GetKeyDown(lockOffEnemyButton))
            {
                playerMovement.mousePosition = transform.position;
                playerMovement.mouseDirection = currentLockedEnemy.position - transform.position;
                playerMovement.mouseDirection.y = 0f;
                lockOffEnemy();
            }    
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
            animator.SetFloat("WeaponType", (int)equipementManager.GetCurrentEquippedWeapon.item.type);
            StartCoroutine(EndAttackMotion(equipementManager.GetCurrentEquippedWeapon.item.AttackDuration));
        }
        playerMovement.attacking = true;
        StartCoroutine(ResetAttack(equipementManager.GetCurrentEquippedWeapon.item.AttackDuration));
    }

    private void Collect()
    {
        Ray rayPosition = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(rayPosition, out var hitInfo, Mathf.Infinity, whatIsItem))
        {

            if (hitInfo.collider != null)
            {
                // if we found an item to pick and we are withing the pickup range , add it to the inventory
                ItemPickup itemToPick = hitInfo.collider.GetComponent<ItemPickup>();
                if (itemToPick != null)
                {
                    if (Vector3.Distance(transform.position, itemToPick.transform.position) <= itemToPick.GetPickUpDistance) Inventory.instance.Add(itemToPick.GetItem);
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
        var randomIndex = Random.Range(0, CloseEnemies.Length);

        //for (int i = 0; i < CloseEnemies.Length; i++)
        //{
        //    if (CloseEnemies[i].transform == currentLockedEnemy)
        //    {
        //        Debug.Log("CurrentLockedEnemy index " + i);
        //    }
        //}
        //Debug.Log("randomIndex before check " + randomIndex);

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
        Physics.SphereCast(transform.position + Vector3.up, hitsize, transform.forward,out var hitInfo, equipementManager.GetCurrentEquippedWeapon.item.AttackRange,whatIsEnemy);
        if (hitInfo.collider != null)
        {
            Enemy enemy = hitInfo.collider.GetComponent<Enemy>();
            if (enemy !=null)
            {
                enemy.EnemyDamaged(equipementManager.GetCurrentEquippedWeapon.item.damage * damageMultiplier);
                if (enemy.GetEnemyHealth == 0)
                {
                    //play deathAnimation
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


}
