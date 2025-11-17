using UnityEngine;

public class GlitchBasic : EnemyBase
{
    [Header("Ranged")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 16f;
    public float projectileDamage = 10f;

    /*protected override void Attack()
    {
        if (!target || !projectilePrefab) return;

        Vector3 spawnPos = transform.position + Vector3.up * 1.2f;

        // Spawn projectile
        GameObject go = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
        go.tag = "Projectile";

        // Rigidbody setup
        Rigidbody rb = go.GetComponent<Rigidbody>();
        if (!rb) rb = go.AddComponent<Rigidbody>();
        rb.useGravity = true;

        // Projectile script setup
        Projectile proj = go.GetComponent<Projectile>();
        if (!proj) proj = go.AddComponent<Projectile>();

        // Properly assign damage using setter
        proj.SetDamage((int)projectileDamage);

        // Fire toward target
        Vector3 dir = (target.position + Vector3.up * 0.6f - spawnPos).normalized;
        rb.linearVelocity = dir * projectileSpeed;
    }*/
}
