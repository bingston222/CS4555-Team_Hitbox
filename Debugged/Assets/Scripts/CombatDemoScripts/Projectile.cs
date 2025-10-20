using UnityEngine;

public class Projectile : MonoBehaviour
{
    public GameObject shockwaveRingPrefab;
    Rigidbody rb;

    int damage;
    float speed;
    GameObject owner;

    EnemyHealth target;   // for homing
    bool homing;

    float life = 5f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb) rb.useGravity = false;
    }

    public void Init(int damage, float speed, GameObject owner)
    {
        this.damage = damage;
        this.speed = speed;
        this.owner = owner;
    }

    // Call this when ult is active to guarantee a hit
    public void SetHomingTarget(EnemyHealth t)
    {
        target = t;
        homing = (t != null);
    }

    void FixedUpdate()
    {
        // Homing motion if we have a live target
        if (homing && target != null && target.IsAlive)
        {
            Vector3 dir = (target.transform.position - transform.position).normalized;
            rb.linearVelocity = dir * speed;
            transform.forward = dir;
        }
        else
        {
            // Normal straight flight
            rb.linearVelocity = transform.forward * speed;
        }

        // lifetime
        life -= Time.fixedDeltaTime;
        if (life <= 0f) Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (owner && other.attachedRigidbody &&
            other.attachedRigidbody.gameObject == owner) return;

        var eh = other.GetComponentInParent<EnemyHealth>();
        if (eh != null && eh.IsAlive)
        {
            eh.TakeDamage(damage);

            // spawn the ring at the hit point (use our current position)
            if (shockwaveRingPrefab)
            {
                var ring = Instantiate(shockwaveRingPrefab, transform.position, Quaternion.Euler(0, 0, 0));
                var sr = ring.GetComponent<ShockwaveRing>();
                if (sr != null) sr.Play();
            }

            Destroy(gameObject);
            return;
        }

        // Die on solid environment so the ring doesn't keep flying
        if (!other.isTrigger) Destroy(gameObject);
    }

}
