using UnityEngine;

public class GlitchBasic : EnemyBase
{
    [Header("Ranged Attack")]
    public ProjectileData projectile;
    public Transform firePoint;
    public float shootDistance = 12f;

    protected override void Update()
    {
        base.Update();

        if (!target) return;
        if (attackTimer > 0f) return;

        float dist = Vector3.Distance(transform.position, target.position);
        if (dist <= shootDistance)
        {
            Attack();
            attackTimer = attackCooldown;
        }
    }

    protected override void Attack()   // ✔ FIXED: protected not public
    {
        if (!projectile || !projectile.prefab) return;

        Vector3 spawnPos = firePoint ? firePoint.position : transform.position + Vector3.up * 1.2f;
        Vector3 aim = target.position + Vector3.up * 0.6f;
        Vector3 dir = (aim - spawnPos).normalized;

        if (projectile.muzzleVfxPrefab)
            Destroy(Instantiate(projectile.muzzleVfxPrefab, spawnPos, Quaternion.LookRotation(dir)), 3f);

        if (projectile.fireSfx)
            AudioSource.PlayClipAtPoint(projectile.fireSfx, spawnPos);

        GameObject go = Instantiate(projectile.prefab, spawnPos, Quaternion.LookRotation(dir));
        var rb = go.GetComponent<Rigidbody>() ?? go.AddComponent<Rigidbody>();
        rb.useGravity = projectile.useGravity;

        var proj = go.GetComponent<EnemyProjectileGlitch>() ?? go.AddComponent<EnemyProjectileGlitch>();
        proj.Init(projectile, transform, dir);

        GetComponent<GlitchAnimatorDriver>()?.PlayAttack();
    }
}
