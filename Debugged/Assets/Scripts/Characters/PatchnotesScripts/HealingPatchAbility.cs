using UnityEngine;
using System.Collections;
using System.Linq;

[DisallowMultipleComponent]
public class HealingPatchAbility : MonoBehaviour
{
    [Header("Input & Cooldown")]
    public KeyCode key = KeyCode.U;
    public float cooldown = 12f;

    [Header("Healing")]
    public float selfHeal = 25f;
    public float allyHeal = 20f;
    public int maxAlliesToHeal = 1;         // 1 = heal one ally; raise to heal more
    public float seekRange = 30f;           // search radius for allies
    public bool requireLineOfSight = false; // optional: only heal allies in LoS
    public LayerMask losMask = ~0;          // what blocks line of sight

    [Header("Animation")]
    public Animator animator;
    public string healTrigger = "Heal";

    [Header("UI (Optional)")]
    public AbilityUI ui;

    [Header("VFX / SFX (Optional)")]
    public ParticleSystem selfHealVfx;      // plays on self
    public ParticleSystem allyHealVfx;      // spawned at each ally
    public AudioSource audioSource;
    public AudioClip castSfx;

    bool ready = true;
    Health myHealth;

    void Awake()
    {
        myHealth   = GetComponent<Health>();
        if (!animator) animator = GetComponentInChildren<Animator>();
        if (!audioSource) audioSource = GetComponentInChildren<AudioSource>();
    }

    void Start()
    {
        if (!ui) ui = GetComponent<AbilityUI>();
    }

    void Update()
    {
        if (ready && Input.GetKeyDown(key))
            StartCoroutine(CastRoutine());
    }

    IEnumerator CastRoutine()
    {
        ready = false;
        ui?.UpdateAbilityCooldown(1f);

        // Anim & SFX
        if (animator) { animator.ResetTrigger(healTrigger); animator.SetTrigger(healTrigger); }
        if (castSfx && audioSource) audioSource.PlayOneShot(castSfx);

        // ---- APPLY HEALS ----
        ApplySelfHeal();
        ApplyAllyHeals();

        // Cooldown ticker
        float t = cooldown;
        while (t > 0f)
        {
            t -= Time.deltaTime;
            ui?.UpdateAbilityCooldown(Mathf.Clamp01(t / cooldown));
            yield return null;
        }

        ui?.UpdateAbilityCooldown(0f);
        ready = true;
    }

    void ApplySelfHeal()
    {
        if (myHealth) myHealth.Heal(selfHeal);
        PlayVfxOn(selfHealVfx, transform);
    }

    void ApplyAllyHeals()
    {
        // choose nearest allies (not self)
        var allies = PlayerLocator.Players
            .Where(p => p && p != transform && InRangeAndVisible(p))
            .OrderBy(p => Vector3.SqrMagnitude(p.position - transform.position))
            .Take(maxAlliesToHeal);

        foreach (var ally in allies)
        {
            var hp = ally.GetComponent<Health>();
            if (!hp) continue;

            hp.Heal(allyHeal);
            PlayVfxOn(allyHealVfx ? allyHealVfx : selfHealVfx, ally);
        }
    }

    bool InRangeAndVisible(Transform t)
    {
        if (!t) return false;
        float distSqr = (t.position - transform.position).sqrMagnitude;
        if (distSqr > seekRange * seekRange) return false;

        if (!requireLineOfSight) return true;

        Vector3 eye   = transform.position + Vector3.up * 1.2f;
        Vector3 chest = t.position + Vector3.up * 1.0f;
        if (Physics.Linecast(eye, chest, out var hit, losMask, QueryTriggerInteraction.Ignore))
            return hit.transform == t;
        return true;
    }

    void PlayVfxOn(ParticleSystem vfxPrefab, Transform target)
    {
        if (!vfxPrefab || !target) return;
        var fx = Instantiate(vfxPrefab, target.position, target.rotation);
        // parent to target so it follows if they move during the burst
        fx.transform.SetParent(target, worldPositionStays: true);
        fx.Play();
        Destroy(fx.gameObject, fx.main.duration + fx.main.startLifetime.constantMax + 0.25f);
    }
}