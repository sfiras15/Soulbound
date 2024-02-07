using UnityEngine;
using UnityEngine.UI;


public class Bar : MonoBehaviour
{

    public enum BarType
    {
        Health = 0,
        Stamina = 1,
        Xp = 2,
    }
    [SerializeField] private Health_SO healthSO;
    [SerializeField] private Stamina_SO stamina_SO;
    [SerializeField] private BarType type;
    private Image fillBar;
    private Text barText;

    private void OnEnable()
    {
        if (type == BarType.Health) healthSO.onHealthChanged += UpdateBar;
        if (type == BarType.Stamina) stamina_SO.onStaminaChanged += UpdateBar;
    }
    private void OnDisable()
    {
        if (type == BarType.Health) healthSO.onHealthChanged -= UpdateBar;
        if (type == BarType.Stamina) stamina_SO.onStaminaChanged -= UpdateBar;
    }
    private void Awake()
    {
        fillBar = GetComponent<Image>();

        // if it's a healthBar get the component
        if (transform.childCount != 0) barText = GetComponentInChildren<Text>();

    }
    public void UpdateBar(int maxValue, int currentValue)
    {
        fillBar.fillAmount = (float)currentValue / maxValue;
        // if it's a healthBar get the text
        if (barText != null) barText.text = currentValue.ToString() + "/" + maxValue.ToString();
    }

    public Text GetHealthBarText
    {
        get { return barText; }
    }
}
