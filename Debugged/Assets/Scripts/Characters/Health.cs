using UnityEngine;
using System;

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHP = 100f;

    [Tooltip("If TRUE, this object is destroyed on 0 HP (enemies). If FALSE, object stays (players).")]
    public bool destroyOnDeath = true;

    public float CurrentHP { get; private set; }

    // HP change event (current, max)
    public event Action<float, float> onHealthChanged;

    // Death event
    public event Action onDeath;

    private bool isDead = false;
    private bool invulnerable = false;

    private void Awake()
    {
        CurrentHP = maxHP;

        // Send initial value to UI
        onHealthChanged?.Invoke(CurrentHP, maxHP);
    }

    // ------ External helpers for older scripts ------
    public void InvokeHealthChanged(float current, float max)
    {
        onHealthChanged?.Invoke(current, max);
    }

    public void ForceUpdateEvent()
    {
        onHealthChanged?.Invoke(CurrentHP, maxHP);
    }
    // -------------------------------------------------

    public void SetInvulnerable(bool value)
    {
        invulnerable = value;
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;
        if (invulnerable) return;

        CurrentHP -= amount;
        if (CurrentHP < 0) CurrentHP = 0;

        onHealthChanged?.Invoke(CurrentHP, maxHP);

        if (CurrentHP <= 0)
            Die();
    }

    public void Heal(float amount)
    {
        if (isDead) return;

        CurrentHP += amount;
        if (CurrentHP > maxHP)
            CurrentHP = maxHP;

        onHealthChanged?.Invoke(CurrentHP, maxHP);
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        // Fire death event for other systems (optional)
        onDeath?.Invoke();

        // ENEMIES DESPAWN — PLAYERS DO NOT
        if (destroyOnDeath)
        {
            Destroy(gameObject);        // Instant despawn — time-friendly
        }
        else
        {
            // Player death behavior (simple freeze)
            var playerStatus = GetComponent<PlayerStatus>();
            if (playerStatus)
                playerStatus.abilitiesDisabled = true;

            var rb = GetComponent<Rigidbody>();
            if (rb)
                rb.isKinematic = true;
        }
    }
}
