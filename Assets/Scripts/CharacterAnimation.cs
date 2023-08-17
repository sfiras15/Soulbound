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
        
        if (playerMovement.state == MovementState.air || playerMovement.state  == MovementState.crouching && playerMovement.jumping && !playerMovement.attacking)
        {
            animator.SetBool("Jump", true);
            animator.SetBool("Grounded", playerMovement.GetGroundedState);
        }
        else animator.SetBool("Jump", false);
        animator.SetBool("Grounded", playerMovement.GetGroundedState);
       
    }
}
