using UnityEngine;


/// <summary>
/// Damages player over time if the player is in contact with the hazards
/// </summary>
public class Hazard : MonoBehaviour
{
    [SerializeField] private float damage = 3f;
    [SerializeField] private float damageTime = 0.5f;
    private bool damageTimer = false;
    private float lastDamagedTime;

    private bool secondAbilityActive;

    private void OnEnable()
    {
        Abilities.onSecondAbilityUsed += playerState;
    }
    private void OnDisable()
    {
        Abilities.onSecondAbilityUsed -= playerState;
    }

    private void playerState(bool state)
    {
        secondAbilityActive = state;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (!damageTimer)
            {
                damageTimer = true;
                lastDamagedTime = Time.time;
            }

            if (Time.time - lastDamagedTime >= damageTime)
            {
                if (!secondAbilityActive) PlayerManager.instance.DamagePlayer(damage);
                damageTimer = false;
            }

        }
        else
        {
            damageTimer = false;
        }
    }
}
