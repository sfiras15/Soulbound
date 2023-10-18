using System;
using UnityEngine;


/// <summary>
/// Saves the game progress for the player when interacting with the attached gameObject
/// Shows The name's of the Object when the player is close
/// </summary>
public class BoneFire : MonoBehaviour, IInteractable
{
    private InteractableUI interactableUI;


    public static event Action<bool> onSaveError;


    private void Awake()
    {
        interactableUI = GetComponentInChildren<InteractableUI>();
        interactableUI.InitializeItemName("BoneFire");
    }

    public void Interact()
    {
        Save();
    }

    public void ShowUI()
    {
        interactableUI.Show();

    }
    public void HideUI()
    {
        interactableUI.Hide();
    }

    private void Save()
    {
        // Can't save when in combat with enemies
        if (EnemyManager.instance.enemiesInCombat == 0)
        {
            DataPersistenceManger.Instance.SaveGame();
            Debug.Log("Saved");
        }
        else
        {
            // To prevent the interactableDetector script from showing the UI element 
            onSaveError?.Invoke(true);
            Invoke(nameof(ResetErrorUI), 1.5f);
            interactableUI.ShowError("Can't save in combat");
        }

    }
    public void ResetErrorUI()
    {
        interactableUI.HideError();

    }


}
