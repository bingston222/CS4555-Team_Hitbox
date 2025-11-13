using UnityEngine;

public class GlitchBasic : EnemyBase
{
    [Header("Ranged")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 16f;
    public float projectileDamage = 10f;

    protected override void Attack()
    {
        if (!target || !projectilePrefab) return;
        var spawnPos = transform.position + Vector3.up * 1.2f;
        var go = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
        go.tag = "Projectile";
        var rb = go.GetComponent<Rigidbody>();
        if (!rb) rb = go.AddComponent<Rigidbody>();
        rb.useGravity = true;

        var proj = go.GetComponent<Projectile>();
        if (!proj) proj = go.AddComponent<Projectile>();
        proj.damage = projectileDamage;

        rb.linearVelocity = (target.position + Vector3.up * 0.6f - spawnPos).normalized * projectileSpeed;
    }
}
