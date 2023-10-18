using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Detects gameObjects that can be interacted with and shows's the UI element
/// </summary>
public class InteractableDetection : MonoBehaviour
{
    [SerializeField] private float interactDistance = 1.5f;
    [SerializeField] private float detectionDistance = 6f;
    [SerializeField] private LayerMask whatIsItem;
    [SerializeField] private Collider[] interactables;
    [SerializeField] private Collider closestInteractable;
    private bool inventoryError;
    private bool saveError;
    private void OnEnable()
    {
        ItemPickup.onErrorPickUp += InventoryState;
        BoneFire.onSaveError += SaveState;

    }
    private void OnDisable()
    {
        ItemPickup.onErrorPickUp -= InventoryState;
        BoneFire.onSaveError -= SaveState;
    }

    private void InventoryState(bool state)
    {
        inventoryError = state;
        Invoke(nameof(ResetInventoryState), 1.5f);
    }
    
    private void ResetInventoryState()
    {
        inventoryError = false;
    }
    private void SaveState(bool state)
    {
        saveError = state;
        Invoke(nameof(ResetSaveState), 1.5f);
    }
    private void ResetSaveState()
    {
        saveError = false;
    }

    // Update is called once per frame
    void Update()
    {
        interactables = Physics.OverlapSphere(transform.position, detectionDistance, whatIsItem);
        if (interactables.Length > 0)
        {
            closestInteractable = GetClosestInteractable(interactables);

            if (closestInteractable != null)
            {
                foreach (var interactable in interactables)
                {
                    if (interactable.TryGetComponent(out IInteractable iInteractable))
                    {
                        if (interactable == closestInteractable && Vector3.Distance(transform.position, interactable.transform.position) <= interactDistance && !inventoryError && !saveError)
                        {
                            // Show the UI for the closest interactable
                            iInteractable.ShowUI();
                        }
                        else
                        {
                            // Hide the UI for all other interactables
                            iInteractable.HideUI();
                        }
                    }
                }
            }
        }
    
    }

    private Collider GetClosestInteractable(Collider[] colliders)
    {
        Collider closestCollider = colliders[0];
        float closestDistance = Vector3.Distance(transform.position, closestCollider.transform.position);

        for (int i = 1; i < colliders.Length; i++)
        {
            float distance = Vector3.Distance(transform.position, colliders[i].transform.position);
            if (distance <= closestDistance )
            {
                closestCollider = colliders[i];
                closestDistance = distance;

            }
        }
        return closestCollider;
    }
}
