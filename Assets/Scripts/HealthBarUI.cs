using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public Slider slider;

    void Start()
    {
        if (playerHealth == null)
            playerHealth = FindObjectOfType<PlayerHealth>();

        if (slider == null)
            slider = GetComponent<Slider>();

        slider.minValue = 0;
        slider.maxValue = playerHealth.maxHealth;
        slider.value = playerHealth.currentHealth;

        // subscribe via code (optional if you wire it in Inspector)
        playerHealth.onHealthChanged.AddListener(OnHealthChanged);
    }

    void OnHealthChanged(int current, int max)
    {
        slider.maxValue = max;
        slider.value = current;
    }
}
