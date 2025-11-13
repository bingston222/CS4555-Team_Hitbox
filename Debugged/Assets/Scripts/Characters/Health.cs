using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHP = 100f;
    public bool invulnerable = false;
    public System.Action<float,float> onHealthChanged;
    public System.Action onDeath;
    float hp;

    void Awake() => hp = maxHP;

    public void TakeDamage(float dmg)
    {
        if (invulnerable || hp <= 0f) return;
        hp = Mathf.Max(0f, hp - dmg);
        onHealthChanged?.Invoke(hp, maxHP);
        if (hp <= 0f) onDeath?.Invoke();
    }
    public void Heal(float amt)
    {
        hp = Mathf.Min(maxHP, hp + amt);
        onHealthChanged?.Invoke(hp, maxHP);
    }
    public void SetInvulnerable(bool v) => invulnerable = v;
}
