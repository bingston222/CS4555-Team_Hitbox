using UnityEngine;
using System;

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHP = 100f;

    // Other scripts read this directly
    public float CurrentHP;

    // Events for UI and status scripts
    public event Action<float, float> onHealthChanged; // (current, max)
    public event Action onDeath;

    private bool invulnerable = false;
    private bool isDead = false;

    // Used only to know if this is a player (players have PlayerStatus)
    private PlayerStatus playerStatus;

    private void Awake()
    {
        CurrentHP = maxHP;
        playerStatus = GetComponent<PlayerStatus>();   // null on enemies, not null on players
        RaiseHealthChanged();
    }

    // ------------------ Public API ------------------

    public void TakeDamage(float amount)
    {
        if (isDead) return;
        if (invulnerable) return;
        if (amount <= 0f) return;

        CurrentHP -= amount;
        if (CurrentHP < 0f) CurrentHP = 0f;

        RaiseHealthChanged();

        if (CurrentHP <= 0f)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (isDead) return;
        if (amount <= 0f) return;

        CurrentHP += amount;
        if (CurrentHP > maxHP) CurrentHP = maxHP;

        RaiseHealthChanged();
    }

    public void SetInvulnerable(bool value)
    {
        invulnerable = value;
    }

    // Some scripts may call this directly
    public void Kill()
    {
        if (isDead) return;

        CurrentHP = 0f;
        RaiseHealthChanged();
        Die();
    }

    /// <summary>
    /// Used by PlayerStatus when respawning the player.
    /// </summary>
    public void ResetFull()
    {
        isDead = false;
        CurrentHP = maxHP;
        RaiseHealthChanged();
    }

    // ------------------ Internal ------------------

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        // Notify listeners first (PlayerStatus listens to this)
        onDeath?.Invoke();

        // If this is NOT a player, destroy the object (enemies despawn)
        if (playerStatus == null)
        {
            Destroy(gameObject);
        }
        // If it IS a player, PlayerStatus will handle respawn via onDeath.
    }

    private void RaiseHealthChanged()
    {
        onHealthChanged?.Invoke(CurrentHP, maxHP);
    }
}
