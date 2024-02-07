using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableObjects", menuName = "Health")]
public class Health_SO : ScriptableObject
{
    private int _currentHealth;
    private int _maxHealth;

    public int CurrentHealth
    {
        get { return _currentHealth; }
        set
        {
            _currentHealth = value;
            onHealthChanged?.Invoke(_maxHealth, _currentHealth);
            Debug.Log("Event invoked currentHEALTH");
        }
    }

    public int MaxHealth
    {
        get { return _maxHealth; }
        set
        {
            _maxHealth = value;
            onHealthChanged?.Invoke(_maxHealth, _currentHealth);
            Debug.Log("Event invoked MaxHealth");
        }
    }

    public event Action<int, int> onHealthChanged;
}
