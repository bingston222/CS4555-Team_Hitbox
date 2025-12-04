using UnityEngine;

public class EnemyProjectileGlitch : MonoBehaviour
{
    private ProjectileData data;
    private Vector3 direction;
    private Transform shooter;
    private float timer;

    GameObject flightVfxInstance;

    public void Init(ProjectileData d, Transform s, Vector3 dir)
    {
        data = d;
        shooter = s;
        direction = dir;
        timer = 0f;

        // Create flight VFX that follows projectile
        if (data.flightVfxPrefab)
        {
            flightVfxInstance = Instantiate(data.flightVfxPrefab, transform.position, Quaternion.identity, transform);
        }

        // Apply gravity settings
        var rb = GetComponent<Rigidbody>();
        if (rb)
            rb.useGravity = data.useGravity;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= data.lifetime)
        {
            CleanupVfx();
            Destroy(gameObject);
            return;
        }

        transform.position += direction * data.speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == shooter) return;

        // COLLSION FILTERING
        if ((data.hittableLayers.value & (1 << other.gameObject.layer)) == 0)
            return;

        // DAMAGE
        Health hp = other.GetComponent<Health>();
        if (hp)
            hp.TakeDamage(data.damage);

        // HIT VFX
        if (data.hitVfxPrefab)
            Destroy(Instantiate(data.hitVfxPrefab, transform.position, Quaternion.identity), 2f);

        // HIT SFX
        if (data.hitSfx)
            AudioSource.PlayClipAtPoint(data.hitSfx, transform.position, 1f);

        CleanupVfx();
        Destroy(gameObject);
    }

    private void CleanupVfx()
    {
        if (flightVfxInstance)
            Destroy(flightVfxInstance);
    }
}
