using UnityEngine;

public class Hazard : MonoBehaviour
{
    [SerializeField] private float damage = 3f;
    [SerializeField] private float damageTime = 0.5f;
    private bool damageTimer = false;
    private float lastDamagedTime;



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
                if (!PlayerManager.secondAbilityActive) PlayerManager.instance.DamagePlayer(damage);
                damageTimer = false;
            }

        }
        else
        {
            damageTimer = false;
        }
    }
}
