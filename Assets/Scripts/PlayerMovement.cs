using System.Collections;
using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float sprintSpeed;
    private float groundedSpeedMultiplier = 1f;
    private float moveSpeed;
    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;

    // the direction the player is going to move to 
    private Vector3 moveDirection;
    // to check if the player reached the goal or not
    private bool canMove;

    //speed in the air
    [SerializeField] private float airMinSpeed;
    [SerializeField] private float speedIncreaseMultiplier;


    [SerializeField] private float groundDrag;
    public Vector3 mousePosition;
    public Vector3 mouseDirection;
    [SerializeField] private float rotateSpeed;

    [Header("Jumping")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCooldown;
    [SerializeField] private float airMultiplier;
    [SerializeField] private bool readyToJump;

    [Header("Audio")]
    [SerializeField] private AudioClip LandingAudioClip;
    [SerializeField] private AudioClip[] FootstepAudioClips;
    [Range(0, 1)] [SerializeField] private float FootstepAudioVolume = 0.5f;

    [Header("Crouching")]
    [SerializeField] private float crouchedHeight = 1.4f;
    [SerializeField] private float crouchSpeed;
    [SerializeField] private float crouchedCenterOffset = 0.17f;
    private float initialAudioSound;
    private Vector3 initialCharacterCenter;

    

    [Header("Keybinds")]
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Ground Check")]
    [SerializeField] private float playerHeight;
    [SerializeField] private LayerMask whatIsGround;
    private bool grounded;
    [SerializeField] private float GroundedOffset = -0.14f;

    [SerializeField] private float GroundedRadius = 0.28f;

    [Header("References")]
    [SerializeField] private Camera cam;
    [SerializeField] private CapsuleCollider capsuleCollider;
    private Animator animator;
    private Rigidbody rb;

    // variable for checking if we are locked onto an enemy or not
    private bool lockedOnEnemy = false;
    // tracking the state of the player 
    public MovementState state;
    public enum MovementState
    {
        freeze,
        unlimited,
        walking,
        sprinting,
        wallrunning,
        climbing,
        attacking,
        crouching,
        air
    }

    public bool jumping;
    public bool sliding;
    public bool crouching;
    public bool wallrunning;
    public bool climbing;
    public bool attacking;

    public bool freeze;
    public bool unlimited;
    
    public bool restricted;

    

    private void Awake()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        initialCharacterCenter = capsuleCollider.center;
        animator = GetComponent<Animator>();
        initialAudioSound = FootstepAudioVolume;

    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;
    }

    private void Update()
    {
        // ground check

        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z);
        grounded = Physics.CheckSphere(spherePosition, GroundedRadius, whatIsGround,
            QueryTriggerInteraction.Ignore);

        MyInput();
        SpeedControl();
        StateHandler();


        // Check if the player has passed the target position and stop moving
        canMove = Vector3.Distance(transform.position, mousePosition) >= 0.7f;
        // handle drag
        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;
        animator.SetFloat("Speed", rb.velocity.magnitude);
    }


    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        Ray rayPosition = cam.ScreenPointToRay(Input.mousePosition);
        if (Input.GetMouseButtonDown(1) /*&& !inputPlayer.attacking*/) // do the raycast and the checking only if the player wanna to move
        {
            if (Physics.Raycast(rayPosition, out var hitInfo, Mathf.Infinity, whatIsGround))
            {

                mousePosition = hitInfo.point;
                mouseDirection = mousePosition - transform.position;
                mouseDirection.y = 0f;
                
                //debugSphere.forward = transform.forward;
            }
        }
        // if we are not locked onto an enemy look toward the mouseDirection
        if (!lockedOnEnemy) transform.forward = Vector3.Lerp(transform.forward, mouseDirection, rotateSpeed * Time.deltaTime);



        // when to jump
        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            jumping = true;
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        Crouch();
    }

    bool keepMomentum;


    private void Crouch()
    {

        // start crouch
        if (Input.GetKeyDown(crouchKey) && !Input.GetMouseButtonDown(1))
        {
            crouching = true;
            FootstepAudioVolume = initialAudioSound / 2f;
            capsuleCollider.height = crouchedHeight;

            capsuleCollider.center = new Vector3(initialCharacterCenter.x, initialCharacterCenter.y - crouchedCenterOffset, initialCharacterCenter.z);
        }
        // stop crouch
        if (Input.GetKeyUp(crouchKey))
        {
            crouching = false;
            FootstepAudioVolume = initialAudioSound;
            capsuleCollider.height = playerHeight;
            capsuleCollider.center = initialCharacterCenter;
        }

        // Start/ stop CrouchMovement BlendTree
        animator.SetBool("Crouch", crouching);
    }
    private void StateHandler()
    {
        // Mode - Freeze
        if (freeze)
        {
            state = MovementState.freeze;
            rb.velocity = Vector3.zero;
            desiredMoveSpeed = 0f;
        }

        // Mode - Unlimited
        else if (unlimited)
        {
            state = MovementState.unlimited;
            desiredMoveSpeed = 999f;
        }

        // Mode - attacking
        else if (attacking)
        {
            state = MovementState.attacking;
            desiredMoveSpeed = 0f;
        }
        // Mode - Crouching
        else if (crouching)
        {
            state = MovementState.crouching;
            desiredMoveSpeed = crouchSpeed;
        }

        // Mode - Sprinting
        else if (grounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            desiredMoveSpeed = sprintSpeed * groundedSpeedMultiplier;
        }

        // Mode - Walking
        else if (grounded)
        {
            state = MovementState.walking;
            desiredMoveSpeed = walkSpeed * groundedSpeedMultiplier;
        }

        // Mode - Air
        else
        {
            state = MovementState.air;

            if (moveSpeed < airMinSpeed)
                desiredMoveSpeed = airMinSpeed;

            // Stop the player after landing on the ground if he passed the target position in the air
            if (Vector3.Distance(transform.position, mousePosition) <= 1.5f)
            {
                mousePosition = transform.position;
            }
        }
            bool desiredMoveSpeedHasChanged = desiredMoveSpeed != lastDesiredMoveSpeed;

        if (desiredMoveSpeedHasChanged)
        {
            if (keepMomentum)
            {
                StopAllCoroutines();
                StartCoroutine(SmoothlyLerpMoveSpeed());
            }
            else
            {
                moveSpeed = desiredMoveSpeed;
            }
        }

        lastDesiredMoveSpeed = desiredMoveSpeed;

        // deactivate keepMomentum
        if (Mathf.Abs(desiredMoveSpeed - moveSpeed) < 0.1f) keepMomentum = false;
    }

    private IEnumerator SmoothlyLerpMoveSpeed()
    {
        // smoothly lerp movementSpeed to desired value
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        while (time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);

            
            time += Time.deltaTime * speedIncreaseMultiplier;

            yield return null;
        }

        moveSpeed = desiredMoveSpeed;
    }

    private void MovePlayer()
    {
        //if (climbingScript.exitingWall) return;
        //if (climbingScriptDone.exitingWall) return;
        if (restricted) return;

        // movement direction

        moveDirection = mouseDirection.normalized;

        // on ground
        if (grounded && canMove)
            rb.AddForce(moveDirection * moveSpeed * 10f, ForceMode.Force);
        //// in air
        else if (!grounded)
        {
            if (rb.velocity.magnitude < 0.1f)
            {
                rb.AddForce(moveDirection * moveSpeed * 10f * airMultiplier, ForceMode.Force);
            }
        }
                
    }

    private void SpeedControl()
    {

        // limiting speed on ground or in air
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // limit velocity if needed
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        readyToJump = true;
        jumping = false;

    }
    public static float Round(float value, int digits)
    {
        float mult = Mathf.Pow(10.0f, (float)digits);
        return Mathf.Round(value * mult) / mult;
    }

    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            if (FootstepAudioClips.Length > 0)
            {
                var index = Random.Range(0, FootstepAudioClips.Length);
                AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(capsuleCollider.center), FootstepAudioVolume);
            }
        }
    }

    private void OnLand(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(capsuleCollider.center), FootstepAudioVolume);
        }
    }
    public void IncreaseSpeed(float multiplier)
    {
        groundedSpeedMultiplier = multiplier;
    }

    public void EnemyLocked(bool value)
    {
        lockedOnEnemy = value;
    }
    public bool GetLockOnEnemyState
    {
        get { return lockedOnEnemy; }
    }

    public bool GetGroundedState
    {
        get { return grounded; }
    }
}
