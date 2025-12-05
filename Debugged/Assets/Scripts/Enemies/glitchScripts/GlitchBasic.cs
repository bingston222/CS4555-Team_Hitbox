using UnityEngine;

public class GlitchBasic : EnemyBase
{
    [Header("Ranged")]
    public ProjectileData projectile;     // assign your PD asset here
    public Transform firePoint;           // optional hand/head; otherwise uses body offset

    protected override void Attack()
    {
        if (!target || !projectile || !projectile.prefab) return;

        // decide where to spawn and where to aim
        Vector3 spawnPos = firePoint ? firePoint.position : (transform.position + Vector3.up * 1.2f);
        Vector3 aimPos   = target.position + Vector3.up * 0.6f;
        Vector3 dir      = (aimPos - spawnPos).normalized;

        // --- Muzzle VFX + Fire SFX at the shooter ---
        if (projectile.muzzleVfxPrefab)
            Destroy(Instantiate(projectile.muzzleVfxPrefab, spawnPos, Quaternion.LookRotation(dir)), 3f);
        if (projectile.fireSfx)
            AudioSource.PlayClipAtPoint(projectile.fireSfx, spawnPos, 1f);

        // --- Spawn projectile prefab ---
        GameObject go = Instantiate(projectile.prefab, spawnPos, Quaternion.LookRotation(dir));
        go.tag = "Projectile"; // optional; create the tag or remove this line

        GetComponent<GlitchAnimatorDriver>()?.PlayAttack();

        // ensure components exist & init
        Rigidbody rb = go.GetComponent<Rigidbody>() ?? go.AddComponent<Rigidbody>();
        rb.useGravity = projectile.useGravity;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.interpolation          = RigidbodyInterpolation.Interpolate;

        // projectile behaviour
        EnemyProjectileGlitch proj = go.GetComponent<EnemyProjectileGlitch>() ?? go.AddComponent<EnemyProjectileGlitch>();
        proj.Init(projectile, transform, dir);

        // (optional) visualize aim for debugging
        // Debug.DrawLine(spawnPos, spawnPos + dir * 3f, Color.cyan, 1f);
    }
}