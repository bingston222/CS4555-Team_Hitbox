using System.Collections;
using System.Linq;
using UnityEngine;

public class HealingPatchAbility : MonoBehaviour
{
    [Header("Settings")]
    public KeyCode key = KeyCode.E;
    public float cooldown = 10f;
    public float selfHeal = 20f;
    public float allyHeal = 20f;
    public float allyRange = 6f;

    [Header("VFX / SFX")]
    public GameObject healVFXSelf;
    public GameObject healVFXAlly;
    public AudioClip healSFX;

    [Header("Projectile")]
    public HealingPatchProjectile healingProjectilePrefab;
    public float projectileForwardOffset = 1.5f;
    public float projectileSpeed = 12f;

    bool ready = true;
    Health myHealth;

    void Start()
    {
        myHealth = GetComponent<Health>();

        if (!myHealth)
            Debug.LogError($"[{name}] Missing Health component for HealingPatchAbility.");
    }

    void Update()
    {
        var status = GetComponent<PlayerStatus>();
        if (!status) return;

        // Cannot use ability during stun, dialogue, cutscene, etc.
        if (status.abilitiesDisabled) return;

        if (ready && Input.GetKeyDown(key))
            StartCoroutine(CastRoutine());
    }

    IEnumerator CastRoutine()
    {
        ready = false;

        // --- SELF HEAL ---
        if (myHealth)
        {
            myHealth.Heal(selfHeal);

            if (healVFXSelf)
                Instantiate(healVFXSelf, transform.position, Quaternion.identity);

            if (healSFX)
                AudioSource.PlayClipAtPoint(healSFX, transform.position, 1f);
        }

        // --- HEAL ALLIES in range ---
        var allies = PlayerLocator.Players
            .Where(p => p && p != transform && InRangeAndVisible(p))
            .ToList();

        foreach (var ally in allies)
        {
            var hp = ally.GetComponent<Health>();
            if (hp)
            {
                hp.Heal(allyHeal);

                if (healVFXAlly)
                    Instantiate(healVFXAlly, ally.position, Quaternion.identity);

                if (healSFX)
                    AudioSource.PlayClipAtPoint(healSFX, ally.position, 1f);
            }
        }

        // --- FIRE HEAL PROJECTILE ---
        if (healingProjectilePrefab)
        {
            Vector3 spawnPos = transform.position + transform.forward * projectileForwardOffset;
            Quaternion spawnRot = transform.rotation;

            var proj = Instantiate(healingProjectilePrefab, spawnPos, spawnRot);
            proj.Init(gameObject, null, allyHeal, projectileSpeed);
        }

        // --- WAIT FOR COOLDOWN ---
        float timer = 0f;
        while (timer < cooldown)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        ready = true;
    }

    // ----------------------------
    // Helper functions
    // ----------------------------

    bool InRangeAndVisible(Transform other)
    {
        float dist = Vector3.Distance(transform.position, other.position);
        if (dist > allyRange) return false;

        Vector3 dir = (other.position - transform.position).normalized;
        if (Physics.Raycast(transform.position + Vector3.up, dir, out var hit, allyRange))
        {
            return hit.transform == other;
        }

        return false;
    }
}
