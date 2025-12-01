using UnityEngine;

public class ShockwaveProjectile : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 12f;
    public float lifetime = 1.2f;

    [Header("Damage")]
    public float damage = 12f;
    public LayerMask hitMask;                  // Enemy layer(s)
    public bool destroyOnFirstHit = true;

    [Header("Homing (optional)")]
    public bool enableHoming = true;
    public float homingTurnRateDeg = 120f;     // how fast it can turn
    public float maxLockDistance = 20f;        // how far to look for targets
    public float lockConeAngle = 35f;          // only lock targets in front
    public float acquisitionWindow = 0.25f;    // seconds after spawn we can acquire

    [Header("Knockback (optional)")]
    public float knockbackForce = 0f;

    [Header("VFX / SFX")]
    public ParticleSystem spawnVFX, hitVFX;
    public AudioSource audioSource;
    public AudioClip spawnClip, hitClip;
    public float spawnVFXLifetime = 3f, hitVFXLifetime = 3f;

    float age = 0f;
    Transform target;                          // current homing target
    Rigidbody rb;

    void Start()
    {
        if (spawnVFX) Destroy(Instantiate(spawnVFX, transform.position, transform.rotation).gameObject, spawnVFXLifetime);
        if (audioSource && spawnClip) audioSource.PlayOneShot(spawnClip);

        rb = GetComponent<Rigidbody>();
        if (rb) rb.linearVelocity = transform.forward * speed;

    }

    void Update()
    {
        age += Time.deltaTime;
        if (age >= lifetime) Destroy(gameObject);

        if (enableHoming && rb)
        {
            // acquire a target only in the early part of flight
            if (!target && age <= acquisitionWindow)
                target = FindTarget();

            if (target)
            {
                Vector3 desired = (target.position - transform.position).normalized;
                // rotate current forward toward desired with a max turn rate
                Vector3 newDir = Vector3.RotateTowards(transform.forward, desired,
                                  Mathf.Deg2Rad * homingTurnRateDeg * Time.deltaTime, 0f);
                transform.rotation = Quaternion.LookRotation(newDir, Vector3.up);
                rb.linearVelocity = newDir * speed;
            }
        }
    }

    Transform FindTarget()
    {
        // quick sphere search, then filter by cone angle & line-of-sight optional
        Collider[] hits = Physics.OverlapSphere(transform.position, maxLockDistance, hitMask);
        Transform best = null;
        float bestAngle = 999f;
        foreach (var c in hits)
        {
            Transform t = c.attachedRigidbody ? c.attachedRigidbody.transform : c.transform;
            Vector3 to = (t.position - transform.position);
            float angle = Vector3.Angle(transform.forward, to);
            if (angle <= lockConeAngle)
            {
                float dist = to.sqrMagnitude;
                // prefer smallest angle, then distance
                if (angle < bestAngle)
                {
                    bestAngle = angle;
                    best = t;
                }
            }
        }
        return best;
    }

    void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & hitMask.value) == 0) return;

        var h = other.GetComponentInParent<Health>();
        if (h) h.TakeDamage(damage);

        if (knockbackForce > 0f && other.attachedRigidbody)
            other.attachedRigidbody.AddForce(transform.forward * knockbackForce, ForceMode.VelocityChange);

        if (hitVFX) Destroy(Instantiate(hitVFX, transform.position, Quaternion.identity).gameObject, hitVFXLifetime);
        if (audioSource && hitClip) audioSource.PlayOneShot(hitClip);

        Destroy(gameObject); // disappear on collision
    }
}
