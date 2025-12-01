using System.Collections;
using UnityEngine;

public class HUDDriver : MonoBehaviour
{
    [Header("Hook these")]
    public HealthBarUI healthUI;   // Healthbar_P1
    public AbilityUI abilityUI;    // Player1_Ability

    [Header("Health")]
    public float maxHP = 100f;
    public float currentHP = 100f;

    [Header("Cooldown lengths (seconds)")]
    public float baseAttackCD = 1.2f;
    public float abilityCD = 4f;

    [Header("Ultimate")]
    public float ultRequired = 100f;
    private float ultCurrent = 0f;

    bool baseCooling = false;
    bool abilityCooling = false;

    void Start()
    {
        healthUI.UpdateBar(currentHP, maxHP);
        abilityUI.UpdateBaseCooldown(0f);     // ready
        abilityUI.UpdateAbilityCooldown(0f);  // ready
        abilityUI.UpdateUltimateFill(0f);
    }

    void Update()
    {
        // DEMO KEYS (change to your inputs later)
        if (Input.GetKeyDown(KeyCode.Minus)) Damage(10f);
        if (Input.GetKeyDown(KeyCode.Equals)) Heal(10f);

        if (Input.GetKeyDown(KeyCode.Alpha1)) UseBase();
        if (Input.GetKeyDown(KeyCode.Alpha2)) UseAbility();

        if (Input.GetKeyDown(KeyCode.Alpha3)) AddUlt(25f);
        if (Input.GetKeyDown(KeyCode.U)) UseUltimate();
    }

    // ---------- Health ----------
    public void Damage(float amt)
    {
        currentHP = Mathf.Max(0, currentHP - amt);
        healthUI.UpdateBar(currentHP, maxHP);
    }

    public void Heal(float amt)
    {
        currentHP = Mathf.Min(maxHP, currentHP + amt);
        healthUI.UpdateBar(currentHP, maxHP);
    }

    // ---------- Abilities ----------
    public void UseBase()
{
    if (baseCooling) return;
    abilityUI.PulseBaseUsed();           // <— NEW
    StartCoroutine(RunCooldown(baseAttackCD, true));
}

public void UseAbility()
{
    if (abilityCooling) return;
    abilityUI.PulseAbilityUsed();        // <— NEW
    StartCoroutine(RunCooldown(abilityCD, false));
}


    IEnumerator RunCooldown(float cd, bool isBase)
    {
        if (isBase) baseCooling = true;
        else abilityCooling = true;

        float left = cd;
        while (left > 0f)
        {
            left -= Time.deltaTime;
            float percent = Mathf.Clamp01(left / cd); // 1 = cooling, 0 = ready
            if (isBase) abilityUI.UpdateBaseCooldown(percent);
            else abilityUI.UpdateAbilityCooldown(percent);
            yield return null;
        }

        if (isBase)
        {
            baseCooling = false;
            abilityUI.UpdateBaseCooldown(0f);
        }
        else
        {
            abilityCooling = false;
            abilityUI.UpdateAbilityCooldown(0f);
        }
    }

    // ---------- Ultimate ----------
    public void AddUlt(float amount)
    {
        ultCurrent = Mathf.Clamp(ultCurrent + amount, 0f, ultRequired);
        abilityUI.UpdateUltimateFill(ultCurrent / ultRequired);
    }

    public void UseUltimate()
    {
        if (ultCurrent < ultRequired) return;
        ultCurrent = 0f;
        abilityUI.UpdateUltimateFill(0f);
    }
}
