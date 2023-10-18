using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class AbilityUI : MonoBehaviour
{
    public static AbilityUI instance;

    [Header("Dash Ability")]
    [Space(10)]
    [SerializeField] private Image firstAbilityPassiveImage;
    [SerializeField] private Image firstAbilityActiveImage;
    [SerializeField] private Image firstAbilityActiveImageCD;
    [SerializeField] private TextMeshProUGUI firstAbilityText;
    private bool soulActive;

    [SerializeField] private Image secondAbilityActiveImage;
    [SerializeField] private Image secondAbilityActiveImageCD;
    [SerializeField] private TextMeshProUGUI secondAbilityText;

    private KeyCode firstAbilityKey;
    private KeyCode secondAbilityKey;
    private void OnEnable()
    {
        Inventory.onSoulChange += SoulState;
        Abilities.onGameStart += InitializeKeys;
    }
    private void OnDisable()
    {
        Inventory.onSoulChange -= SoulState;
        Abilities.onGameStart -= InitializeKeys;
    }

    private void InitializeKeys(KeyCode firstAbilityKey,KeyCode secondAbilityKey)
    {
        this.firstAbilityKey = firstAbilityKey;
        this.secondAbilityKey = secondAbilityKey;
    }

    
    public void SoulState(bool value)
    {
        soulActive = value;
    }
    // Start is called before the first frame update
    void Start()
    {

        firstAbilityActiveImageCD.fillAmount = 0;
        secondAbilityActiveImageCD.fillAmount = 0;
        firstAbilityText.text = firstAbilityKey.ToString();
        secondAbilityText.text = secondAbilityKey.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        //Changes the icon based on wheather the player has a soul in his inventory or not
        if (soulActive)
        {
            firstAbilityPassiveImage.enabled = false;
            firstAbilityActiveImage.enabled = true;
            firstAbilityActiveImageCD.enabled = true;

        }
        else
        {
            firstAbilityPassiveImage.enabled = true;
            firstAbilityActiveImage.enabled = false;
            firstAbilityActiveImageCD.enabled = false;
        }
    }
    
    // Animate the UI
    public void UpdateCooldown(float cooldownTimer, float cooldownDuration, int abilityIndex)
    {
        if (cooldownTimer > 0)
        {
            float fillAmount = cooldownTimer / cooldownDuration;
            if (abilityIndex == 1) firstAbilityActiveImageCD.fillAmount = fillAmount;
            if (abilityIndex == 2) secondAbilityActiveImageCD.fillAmount = fillAmount;

        }
        else
        {
            if (abilityIndex == 1) firstAbilityActiveImageCD.fillAmount = 0;
            if (abilityIndex == 2) secondAbilityActiveImageCD.fillAmount = 0;
        }
    }

}
