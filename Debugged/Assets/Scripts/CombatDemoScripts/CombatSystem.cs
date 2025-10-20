using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    CharacterStats stats;

    [Header("Auto-Hit Settings (during Instant Hitbox)")]
    public float autoHitRange = 30f;           // how far we can 'guarantee' a hit
    public LayerMask enemyMask = ~0;           // optionally set to your Enemy layer

    void Awake() => stats = GetComponent<CharacterStats>();

    public void PerformBaseAttack(Transform muzzle, GameObject projectilePrefab, float speed = 18f)
{
    if (!stats.IsAlive) return;

    if (InstantHitboxState.IsActive)
    {
        // Find nearest enemy to home in on
        EnemyHealth target = FindNearestEnemy(muzzle.position, autoHitRange);

        var go = Instantiate(projectilePrefab, muzzle.position, muzzle.rotation);
        var p = go.GetComponent<Projectile>();
        if (p == null) { Debug.LogError("Projectile missing component"); Destroy(go); return; }

        p.Init(stats.attackPower, speed, gameObject);
        if (target != null) p.SetHomingTarget(target);  // <- guarantee hit
        return;
    }

    // Normal (non-ult) projectile
    var proj = Instantiate(projectilePrefab, muzzle.position, muzzle.rotation);
    var pp = proj.GetComponent<Projectile>();
    if (pp == null) { Debug.LogError("Projectile missing component"); Destroy(proj); return; }
    pp.Init(stats.attackPower, speed, gameObject);
}

EnemyHealth FindNearestEnemy(Vector3 from, float range)
{
    EnemyHealth best = null; float bestSqr = float.MaxValue;
    var enemies = Object.FindObjectsOfType<EnemyHealth>();
    foreach (var e in enemies)
    {
        if (!e || !e.IsAlive) continue;
        float d2 = (e.transform.position - from).sqrMagnitude;
        if (d2 <= range * range && d2 < bestSqr) { best = e; bestSqr = d2; }
    }
    return best;
}


    void AutoHitNearest(Vector3 from, float range, int damage)
    {
        EnemyHealth best = null;
        float bestSqr = float.MaxValue;

        // Fast: scan all enemies (simple for your demo); for big games, keep a registry.
        var enemies = Object.FindObjectsOfType<EnemyHealth>();
        foreach (var e in enemies)
        {
            if (!e || !e.IsAlive) continue;
            float d2 = (e.transform.position - from).sqrMagnitude;
            if (d2 <= range * range && d2 < bestSqr)
            {
                best = e; bestSqr = d2;
            }
        }

        if (best != null)
        {
            best.TakeDamage(damage);
            stats.GainUltCharge(10); // keep your reward on 'hit'
        }
        else
        {
            // (Optional) No enemy in range—spawn normal projectile as fallback
            // or show a small 'no target' UI.
        }
    }
    
    public void ApplyAbilityInvulnerability(float duration)
{
    var sc = GetComponent<StatusController>();
    if (sc == null) sc = gameObject.AddComponent<StatusController>();
    StartCoroutine(sc.Invulnerability(duration));
}


    // If you prefer to hit ALL enemies in range during ult, replace AutoHitNearest with this:
    /*
    void AutoHitAll(Vector3 from, float range, int damage)
    {
        var enemies = Object.FindObjectsOfType<EnemyHealth>();
        foreach (var e in enemies)
        {
            if (!e || !e.IsAlive) continue;
            if ((e.transform.position - from).sqrMagnitude <= range * range)
                e.TakeDamage(damage);
        }
    }
    */
}
