using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyProjectileGlitch : MonoBehaviour
{
    private Rigidbody rb;
    private ProjectileData data;
    private Transform owner;
    private bool initialized;
    private GameObject flightVfxInstance;

    public void Init(ProjectileData d, Transform shooter, Vector3 dir)
    {
        data  = d;
        owner = shooter;
        rb    = GetComponent<Rigidbody>();

        // Rigidbody setup
        rb.useGravity = data.useGravity;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.interpolation          = RigidbodyInterpolation.Interpolate;

        // Face and launch
        transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
        rb.linearVelocity        = dir * data.speed;

        // --- attach flying VFX ---
        if (data.flightVfxPrefab)
        {
            flightVfxInstance = Instantiate(data.flightVfxPrefab, transform);
            flightVfxInstance.transform.localPosition = Vector3.zero;

            var ps = flightVfxInstance.GetComponent<ParticleSystem>();
            if (ps)
            {
                var main = ps.main;
                main.simulationSpace = ParticleSystemSimulationSpace.World; // particles stay behind
                ps.Play();
            }
        }

        // lifetime + fire sound
        if (data.lifetime > 0f) Invoke(nameof(SelfDestruct), data.lifetime);
        if (data.fireSfx) AudioSource.PlayClipAtPoint(data.fireSfx, transform.position, 1f);

        initialized = true;
    }

    private void OnCollisionEnter(Collision c)
    {
        if (!initialized) return;
        if (owner && c.transform == owner) return;

        if (((1 << c.gameObject.layer) & data.hittableLayers) != 0)
            c.gameObject.GetComponent<Health>()?.TakeDamage(data.damage);

        SpawnImpactFx(c.contacts.Length > 0 ? c.contacts[0].point : transform.position);
        SelfDestruct();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!initialized) return;
        if (owner && other.transform == owner) return;

        if (((1 << other.gameObject.layer) & data.hittableLayers) != 0)
            other.GetComponent<Health>()?.TakeDamage(data.damage);

        SpawnImpactFx(transform.position);
        SelfDestruct();
    }

    private void SpawnImpactFx(Vector3 pos)
    {
        if (data.hitVfxPrefab)
        {
            var vfx = Instantiate(data.hitVfxPrefab, pos, Quaternion.identity);
            Destroy(vfx, 4f); // remove after 4 s
        }
        if (data.hitSfx) AudioSource.PlayClipAtPoint(data.hitSfx, pos, 1f);
    }

    private void SelfDestruct()
    {
        // fade out flight VFX before destroy
        if (flightVfxInstance)
        {
            foreach (var ps in flightVfxInstance.GetComponentsInChildren<ParticleSystem>())
                ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
        Destroy(gameObject, 0.25f); // small delay lets last particles fade
    }
}