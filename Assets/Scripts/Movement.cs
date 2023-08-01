using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Cinemachine.CinemachineOrbitalTransposer;

public class Movement : MonoBehaviour
{

    [Header("References")]

    [SerializeField] private ThirdPersonController thirdPersonController;

    [SerializeField] private CharacterController characterController;


    [Header("Crouching")]

    [SerializeField] private KeyCode crouchKey;

    private float horizontalInput;
    private float verticalInput;

    private float initialHeight;
    [SerializeField] private float crouchedHeight = 1.4f;

    [SerializeField] private float crouchedCenterOffset = 0.17f;
    private Vector3 initialCharacterCenter;

    private float initialAudioSound;

    private float initialSpeed;

    [SerializeField] private bool crouching;

    private Animator animator;


    //[Header("LedgeDetection")]
    //public Transform orientation;

    //public float ledgeDetectionLength;
    //public float ledgeSphereCastRadius;
    //public LayerMask whatIsLedge;
    //private RaycastHit ledgeHit;
    //public bool holding;
    //private Transform lastLedge;
    //private Transform currLedge;

    //public float maxLedgeGrabDistance;

    // Start is called before the first frame update
    private void Awake()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        thirdPersonController = GetComponent<ThirdPersonController>();

        initialCharacterCenter = characterController.center;
        initialHeight = characterController.height;
        initialAudioSound = thirdPersonController.FootstepAudioVolume;
        initialSpeed = thirdPersonController.MoveSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        Crouch();
        //LedgeDetection();
    }

    private void Crouch()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // start crouch
        if (Input.GetKeyDown(crouchKey) && horizontalInput == 0 && verticalInput == 0)
        {
            crouching = true;
            thirdPersonController.FootstepAudioVolume = initialAudioSound / 2f;
            thirdPersonController.MoveSpeed = initialSpeed / 1.25f;
            characterController.height = crouchedHeight;

            characterController.center = new Vector3(initialCharacterCenter.x, initialCharacterCenter.y - crouchedCenterOffset, initialCharacterCenter.z);
        }
        // stop crouch
        if (Input.GetKeyUp(crouchKey))
        {
            crouching = false;
            thirdPersonController.FootstepAudioVolume = initialAudioSound;
            thirdPersonController.MoveSpeed = initialSpeed;
            characterController.height = initialHeight;
            characterController.center = initialCharacterCenter;
        }

        thirdPersonController.Crouched = crouching;

        // Start/ stop CrouchMovement BlendTree
        animator.SetBool("Crouch", crouching);
    }

    //private void LedgeDetection()
    //{
    //    bool ledgeDetected = Physics.SphereCast(transform.position, ledgeSphereCastRadius, transform.forward, out ledgeHit, ledgeDetectionLength, whatIsLedge);

    //    if (!ledgeDetected) return;

    //    float distanceToLedge = Vector3.Distance(transform.position, ledgeHit.transform.position);

    //    if (ledgeHit.transform == lastLedge) return;

    //    if (distanceToLedge < maxLedgeGrabDistance && !holding) EnterLedgeHold();
    //}

    //private void EnterLedgeHold()
    //{
    //    holding = true;
    //    //Freeze the character's position

    //    // update the ledges variables

    //}

}
