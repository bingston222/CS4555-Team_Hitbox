using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    // --- Cached collider data (capsule) ---
    private CapsuleCollider capsule;
    private float baseRadius, baseHeight;

    // --- Active VFX instance (so we can clean it up) ---
    private GameObject _activeAura;

    [Header("Stats")]
    public int maxHealth = 80;
    public int defense = 3;
    public int currentHealth = 80;

    [Header("Instant Hitbox Settings")]
    [Tooltip("How much to scale the enemy's hitbox (collider) during Instant Hitbox.")]
    public float hitboxScaleMultiplier = 3f;

    [Tooltip("Optional visual prefab (e.g., a transparent sphere with HitboxAura script).")]
    public GameObject hitboxEnlargeVFX;

    // State
    private bool _isAlive = true;
    public bool IsAlive => _isAlive;

    void Awake()
    {
        _isAlive = true;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        capsule = GetComponent<CapsuleCollider>();
        if (capsule != null)
        {
            baseRadius = capsule.radius;
            baseHeight = capsule.height;
        }
        else
        {
            Debug.LogWarning($"{name}: EnemyHealth expects a CapsuleCollider (none found). " +
                             "Instant Hitbox will still show VFX but won't enlarge the hitbox.");
        }
    }

    // --- Damage / Death ---
    public void TakeDamage(int rawDamage)
    {
        if (!_isAlive) return;

        int final = Mathf.Max(1, rawDamage - defense);
        currentHealth = Mathf.Max(0, currentHealth - final);

        if (currentHealth == 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (!_isAlive) return;
        _isAlive = false;

        // Restore collider size if we die mid-ult
        if (capsule != null)
        {
            capsule.radius = baseRadius;
            capsule.height = baseHeight;
        }

        // Kill any lingering aura
        if (_activeAura) { Destroy(_activeAura); _activeAura = null; }

        // (Optional: drop loot, play SFX/VFX here)

        Destroy(gameObject);
    }

    void OnDisable()
    {
        // Extra safety: clean up aura if this object gets disabled/destroyed by other means
        if (_activeAura) { Destroy(_activeAura); _activeAura = null; }
    }

    // --- Instant Hitbox API (called by InstantHitboxManager) ---
    public Coroutine ExpandHitbox(float duration)
    {
        return StartCoroutine(ExpandRoutine(duration));
    }

    private IEnumerator ExpandRoutine(float duration)
    {
        if (!_isAlive) yield break;

        // Spawn & parent the aura (so it disappears with the enemy)
        if (hitboxEnlargeVFX != null)
        {
            _activeAura = Instantiate(hitboxEnlargeVFX, transform.position, Quaternion.identity);
            _activeAura.transform.SetParent(transform, worldPositionStays: true);

            var auraController = _activeAura.GetComponent<HitboxAura>();
            if (auraController != null)
                auraController.Play(transform, duration, hitboxScaleMultiplier);
        }

        // Enlarge the collider (hitbox) only
        if (capsule != null)
        {
            capsule.radius = baseRadius * hitboxScaleMultiplier;
            capsule.height = baseHeight * hitboxScaleMultiplier;
        }

        float end = Time.time + duration;
        while (Time.time < end)
        {
            if (!_isAlive) break; // if we died during the effect, bail early
            yield return null;
        }

        // Restore collider
        if (capsule != null)
        {
            capsule.radius = baseRadius;
            capsule.height = baseHeight;
        }

        // Remove aura if still around
        if (_activeAura) { Destroy(_activeAura); _activeAura = null; }
    }
}
