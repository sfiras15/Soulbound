using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerMovement;


//Handles the basic movement animation, still need work

public class CharacterAnimation : MonoBehaviour
{

    [SerializeField] private Animator animator;

    [SerializeField] private PlayerMovement playerMovement;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }
    // Start is called before the first frame update
    void Start()
    {
        animator.SetFloat("MotionSpeed", 1);
    }

    // Update is called once per frame
    void Update()
    {
        
        if (playerMovement.state == MovementState.air)
        {
            animator.SetBool("Jump", true);
            animator.SetBool("Grounded", playerMovement.grounded);
        }
        else animator.SetBool("Jump", false);
        animator.SetBool("Grounded", playerMovement.grounded);
       
    }
}
