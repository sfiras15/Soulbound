using UnityEngine;
using UnityEngine.UI;


public class Bar : MonoBehaviour
{
    private Image fillBar;
    private Text barText;
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
