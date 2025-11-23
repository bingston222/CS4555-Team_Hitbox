using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [System.Serializable]
    public class HealthChangeEvent : UnityEvent<float, float> { }

    public float maxHP = 100f;
    public bool invulnerable = false;

    public float CurrentHP => hp;

    


    // UI event
    public HealthChangeEvent onHealthChanged;

    // Gameplay event (easier for enemies)
    public System.Action onDeath;

    private float hp;

    void Start()
    {
        hp = maxHP;
        onHealthChanged?.Invoke(hp, maxHP);
    }

    public void TakeDamage(float dmg)
    {
        if (invulnerable || hp <= 0f)
            return;

        hp = Mathf.Max(0f, hp - dmg);
        onHealthChanged?.Invoke(hp, maxHP);

        if (hp <= 0f)
            onDeath?.Invoke();
    }

    public void Heal(float amt)
    {
        hp = Mathf.Min(maxHP, hp + amt);
        onHealthChanged?.Invoke(hp, maxHP);
    }

    public void SetInvulnerable(bool v)
    {
        invulnerable = v;
    }
    // In your Health script, wherever you finalize death:
bool _dead; // guard so it runs once

void Die()
{
    if (_dead) return;
    _dead = true;

   
    if (TryGetComponent<EnemyDeads>(out var enemyReg))
        enemyReg.NotifyDied();

    
}

}
