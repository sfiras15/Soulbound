using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Handles the different inputs and states of the player to trigger the animations

public class InputPlayer : MonoBehaviour
{
    [SerializeField] private KeyCode collectKey;

    private bool collectKeyPressed;
    private bool interactionKeyPressed;

    public bool usingItem;
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

    }
    private void OnCollisionStay(Collision collision)
    {
        if (collectKeyPressed)
        {
            Item item = collision.gameObject.GetComponent<Item>();
            
            if (item != null)
            {
                Inventory.instance.Add(item);
                collision.gameObject.GetComponent<MeshRenderer>().enabled = false;
                collision.gameObject.GetComponent<BoxCollider>().enabled = false;
            }
        }
    }
}
