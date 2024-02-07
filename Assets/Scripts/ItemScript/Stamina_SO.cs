using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableObjects", menuName = "Stamina")]
public class Stamina_SO : ScriptableObject
{
    private int _currentStamina;
    private int _maxStamina;

    public int CurrentStamina
    {
        get { return _currentStamina; }
        set
        {
            _currentStamina = value;
            onStaminaChanged?.Invoke(_maxStamina, _currentStamina);
            Debug.Log("Event invoked currentStamina");
        }
    }

    public int MaxStamina
    {
        get { return _maxStamina; }
        set
        {
            _maxStamina = value;
            onStaminaChanged?.Invoke(_maxStamina, _currentStamina);
            Debug.Log("Event invoked MaxStamina");
        }
    }

    public event Action<int, int> onStaminaChanged;
}
