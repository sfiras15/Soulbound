//using StarterAssets;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using static Cinemachine.CinemachineOrbitalTransposer;

//public class Movement : MonoBehaviour
//{
//    //public Transform debugSphere;

//    public Transform Cam;
//    [Header("References")]

//    [SerializeField] private ThirdPersonController thirdPersonController;

//    [SerializeField] private CharacterController characterController;


//    [Header("Crouching")]

//    [SerializeField] private KeyCode crouchKey;

//    private float horizontalInput;
//    private float verticalInput;

//    private float initialHeight;
//    [SerializeField] private float crouchedHeight = 1.4f;

//    [SerializeField] private float crouchedCenterOffset = 0.17f;
//    private Vector3 initialCharacterCenter;

//    private float initialAudioSound;

//    private float initialSpeed;

//    [SerializeField] private bool crouching;

//    private Animator animator;


//    [Header("LedgeDetection")]
//    public Transform orientation;
//    public Vector3 direction;
//    public float ledgeDetectionLength;
//    public float ledgeSphereCastRadius;
//    public LayerMask whatIsLedge;
//    private RaycastHit ledgeHit;
//    public bool holding;
//    private Transform lastLedge;
//    private Transform currLedge;

//    public float maxLedgeGrabDistance;

//    public CharacterAim characterAim;

//    public float moveToLedgeSpeed;

//    public KeyCode jumpKey;

//    public StarterAssetsInputs starterAssetsInputs;

//    // Start is called before the first frame update
//    private void Awake()
//    {
//        animator = GetComponent<Animator>();

//        characterController = GetComponent<CharacterController>();

//        thirdPersonController = GetComponent<ThirdPersonController>();

//        characterAim = GetComponent<CharacterAim>();

//        starterAssetsInputs = GetComponent<StarterAssetsInputs>();

//        initialCharacterCenter = characterController.center;

//        initialHeight = characterController.height;

//        initialAudioSound = thirdPersonController.FootstepAudioVolume;

//        initialSpeed = thirdPersonController.MoveSpeed;
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        Crouch();
//        //LedgeDetection();
//        //CharacterState();
//    }

//    private void Crouch()
//    {
//        horizontalInput = Input.GetAxisRaw("Horizontal");
//        verticalInput = Input.GetAxisRaw("Vertical");

//        // start crouch
//        if (Input.GetKeyDown(crouchKey) && horizontalInput == 0 && verticalInput == 0)
//        {
//            crouching = true;
//            thirdPersonController.FootstepAudioVolume = initialAudioSound / 2f;
//            thirdPersonController.MoveSpeed = initialSpeed / 1.25f;
//            characterController.height = crouchedHeight;

//            characterController.center = new Vector3(initialCharacterCenter.x, initialCharacterCenter.y - crouchedCenterOffset, initialCharacterCenter.z);
//        }
//        // stop crouch
//        if (Input.GetKeyUp(crouchKey))
//        {
//            crouching = false;
//            thirdPersonController.FootstepAudioVolume = initialAudioSound;
//            thirdPersonController.MoveSpeed = initialSpeed;
//            characterController.height = initialHeight;
//            characterController.center = initialCharacterCenter;
//        }

//        thirdPersonController.Crouched = crouching;

//        // Start/ stop CrouchMovement BlendTree
//        animator.SetBool("Crouch", crouching);
//    }

//    private void LedgeDetection()
//    {
//        bool ledgeDetected = Physics.SphereCast(orientation.position, ledgeSphereCastRadius, characterAim.direction, out ledgeHit, ledgeDetectionLength, whatIsLedge);    

//        if (!ledgeDetected) return;

//        float distanceToLedge = Vector3.Distance(transform.position, ledgeHit.transform.position);
  
//        if (ledgeHit.transform == lastLedge) return;

//        if (distanceToLedge < maxLedgeGrabDistance && !holding) EnterLedgeHold();
//    }

//    private void EnterLedgeHold()
//    {
//        holding = true;


//        animator.SetFloat("Speed", 0);
//        thirdPersonController.enabled = false;

//        characterAim.enabled = false;

//        currLedge = ledgeHit.transform;
//        lastLedge = ledgeHit.transform;
//    }

//    private void CharacterState()
//    {


//        // SubState 1 - Holding onto ledge
//        if (holding)
//        {
//            FreezeRigidbodyOnLedge();

//            if (Input.GetKeyDown(jumpKey))
//            {
//                thirdPersonController.enabled = true;
//                starterAssetsInputs.JumpInput(true);
//                characterAim.enabled = true;
//            }
//        }

//        //// Substate 2 - Exiting Ledge
        
//    }
//    private void FreezeRigidbodyOnLedge()
//    {
        
//        Vector3 directionToLedge = currLedge.position - transform.position;
//        float distanceToLedge = Vector3.Distance(transform.position, currLedge.position);

//        // Move player towards ledge
//        if (distanceToLedge <= maxLedgeGrabDistance)
//        {
//            Vector3 ledgePosition = new Vector3(currLedge.position.x, transform.position.y,currLedge.position.z);   
//            transform.position = Vector3.Lerp(transform.position, ledgePosition, moveToLedgeSpeed * Time.deltaTime);
//        }

//        //// Hold onto ledg

//        //// Exiting if something goes wrong
//        //if (distanceToLedge > maxLedgeGrabDistance) ExitLedgeHold();
//    }

//}000000000000000000000000
