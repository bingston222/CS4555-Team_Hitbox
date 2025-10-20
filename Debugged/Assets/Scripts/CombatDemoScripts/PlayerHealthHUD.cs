using UnityEngine;
using UnityEngine.UI;
using TMPro;  // add this line

public class PlayerHealthHUD : MonoBehaviour
{
    public CharacterStats playerStats;
    public Slider slider;
    public TextMeshProUGUI valueText; // change type to TMP
    // ^ instead of "Text"

    void Awake()
    {
        if (slider == null) slider = GetComponent<Slider>();
    }

    void Update()
    {
        if (playerStats == null) return;

        slider.value = playerStats.currentHealth;

        if (valueText != null)
            valueText.text = $"{playerStats.currentHealth} / {playerStats.maxHealth}";
    }
}
