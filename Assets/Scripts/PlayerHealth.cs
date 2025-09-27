using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Events")]
    public UnityEvent<int,int> onHealthChanged; // (current, max)
    public UnityEvent onDied;

    void Awake()
    {
        currentHealth = maxHealth;
        onHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void RestoreHealth(int amount)
    {
        if (amount <= 0) return;
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        onHealthChanged?.Invoke(currentHealth, maxHealth);
        // Debug.Log($"Heal {amount} → {currentHealth}/{maxHealth}");
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0) return;
        currentHealth = Mathf.Max(currentHealth - amount, 0);
        onHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth == 0)
        {
            // Debug.Log("Player died");
            onDied?.Invoke();
            // TODO: disable input / respawn / reload scene
        }
    }

    public void HealToFull()
    {
        currentHealth = maxHealth;
        onHealthChanged?.Invoke(currentHealth, maxHealth);
    }
}
