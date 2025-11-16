using UnityEngine;

public class BossBase : MonoBehaviour
{
    public int maxHP = 150;
    public int currentHP;

    public bool isWeakened = false;

    private void Start()
    {
        currentHP = maxHP;
    }

    public virtual void TakeDamage(int dmg)
    {
        currentHP -= dmg;

        if (currentHP <= maxHP * 0.5f && !isWeakened)
        {
            isWeakened = true;
            OnWeakened();
        }

        if (currentHP <= 0)
        {
            Die();
        }
    }

    protected virtual void OnWeakened()
    {
        // Boss-specific weaken effects added in child classes
    }

    protected virtual void Die()
    {
        // Play animation, drop loot, etc.
        Destroy(gameObject);
    }
}
