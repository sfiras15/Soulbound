using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image fillBar;

    public void UpdateHealth(int maxHealth,int currentHealth)
    {
        fillBar.fillAmount = (float) currentHealth / maxHealth;
    }

}
