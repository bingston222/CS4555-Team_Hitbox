using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HitBoxController : MonoBehaviour
{
    [Header("Refs")]
    public Transform muzzle;
    public GameObject shockwaveProjectile;

    [Header("UI (optional)")]
    public Text hpText;
    public Text abilityText;
    public Text ultText;

    CombatSystem combat;
    CharacterStats stats;

    float nextAbilityTime;

    void Awake()
    {
        stats = GetComponent<CharacterStats>();
        combat = GetComponent<CombatSystem>();
    }

    void Update()
    {
        InstantHitboxState.Tick();  

        // Basic WASD movement placeholder
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        transform.position += new Vector3(h, 0, v) * 6f * Time.deltaTime;
        transform.forward = new Vector3(h, 0, v).sqrMagnitude > 0.01f ? new Vector3(h, 0, v) : transform.forward;

        // Base Attack
        // Base Attack (Space)
    if (Input.GetKeyDown(KeyCode.Space))
    {
        combat.PerformBaseAttack(muzzle, shockwaveProjectile);
    }

    // Ability (Q)
    if (Input.GetKeyDown(KeyCode.Q) && Time.time >= nextAbilityTime)
    {
        nextAbilityTime = Time.time + stats.abilityCooldown;
        combat.ApplyAbilityInvulnerability(stats.abilityDuration);
        StartCoroutine(ShowAbilityWindow(stats.abilityDuration));
    }   

    // Ultimate (E)
    if (Input.GetKeyDown(KeyCode.E) && stats.HasUltimateReady)
    {
        stats.SpendUltimate();
        InstantHitboxManager.Trigger(stats.ultimateDuration);
        StartCoroutine(ShowUltimateWindow(stats.ultimateDuration));
    }

        UpdateUI();
    }

    void UpdateUI()
    {
        if (hpText) hpText.text = $"HP: {stats.currentHealth}/{stats.maxHealth}";
        if (abilityText)
        {
            float cd = Mathf.Max(0, nextAbilityTime - Time.time);
            abilityText.text = cd > 0 ? $"Ability CD: {cd:0}s" : "Ability: Ready";
        }
        if (ultText) ultText.text = stats.HasUltimateReady ? "ULT: READY" : $"ULT: {stats.ultimateCurrentCharge}/{stats.ultimateMaxCharge}";
    }

    IEnumerator ShowAbilityWindow(float dur)
    {
        // Optional visual: tint player while invulnerable
        var rend = GetComponentInChildren<Renderer>();
        Color? old = null;
        if (rend) { old = rend.material.color; rend.material.color = Color.cyan; }
        yield return new WaitForSeconds(dur);
        if (rend && old.HasValue) rend.material.color = old.Value;
    }

    IEnumerator ShowUltimateWindow(float dur)
    {
        // Optional: slight screen shake or player glow
        yield return null;
    }
}
