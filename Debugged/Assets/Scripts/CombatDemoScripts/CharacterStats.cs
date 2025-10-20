using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [Header("Core")]
    public string characterName = "HitBox";
    public int maxHealth = 100;
    public int currentHealth = 100;
    public int attackPower = 20;
    public int defense = 5;

    [Header("Ability")]
    public string abilityName = "Hitbox Freeze";
    public float abilityCooldown = 120f; // 2 minutes
    public float abilityDuration = 7f;

    [Header("Ultimate")]
    public string ultimateName = "Instant Hitbox";
    public int ultimateMaxCharge = 100;
    public int ultimateCurrentCharge = 0;
    public float ultimateDuration = 6f;

    public bool IsAlive => currentHealth > 0;

    public void TakeDamage(int rawDamage)
{
    if (!IsAlive) return;

    var sc = GetComponent<StatusController>();
    if (sc != null && sc.IsInvulnerable) return;  // <- freeze makes you immune

    int final = Mathf.Max(1, rawDamage - defense);
    currentHealth = Mathf.Max(0, currentHealth - final);
}


    public void HealToFull() => currentHealth = maxHealth;

    public void GainUltCharge(int amount)
    {
        ultimateCurrentCharge = Mathf.Clamp(ultimateCurrentCharge + amount, 0, ultimateMaxCharge);
    }

    public bool HasUltimateReady => ultimateCurrentCharge >= ultimateMaxCharge;
    public void SpendUltimate() => ultimateCurrentCharge = 0;
}
