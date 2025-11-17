using UnityEngine;

public class Projectile : MonoBehaviour
{
    public GameObject shockwaveRingPrefab;
    private Rigidbody rb;

    [SerializeField] private int damage;     // now serialized + private
    public int Damage => damage;            // safe public getter

    private float speed;
    private GameObject owner;

    private EnemyHealth target;   // for homing
    private bool homing;

    private float life = 5f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb) rb.useGravity = false;
    }

    // Setter used by GlitchBasic.cs
    public void SetDamage(int d)
    {
        damage = d;
    }

    public void Init(int damage, float speed, GameObject owner)
    {
        this.damage = damage;
        this.speed = speed;
        this.owner = owner;
    }

    public void SetHomingTarget(EnemyHealth t)
    {
        target = t;
        homing = (t != null);
    }

    void FixedUpdate()
    {
        if (homing && target != null && target.IsAlive)
        {
            Vector3 dir = (target.transform.position - transform.position).normalized;
            rb.linearVelocity = dir * speed;
            transform.forward = dir;
        }
        else
        {
            rb.linearVelocity = transform.forward * speed;
        }

        life -= Time.fixedDeltaTime;
        if (life <= 0f) Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        // Ignore hitting the shooter
        if (owner && other.attachedRigidbody &&
            other.attachedRigidbody.gameObject == owner) return;

        // Hit enemy
        EnemyHealth eh = other.GetComponentInParent<EnemyHealth>();
        if (eh != null && eh.IsAlive)
        {
            eh.TakeDamage(damage);

            if (shockwaveRingPrefab)
            {
                GameObject ring = Instantiate(shockwaveRingPrefab, transform.position, Quaternion.identity);
                ShockwaveRing sr = ring.GetComponent<ShockwaveRing>();
                if (sr != null) sr.Play();
            }

            Destroy(gameObject);
            return;
        }

        // Hit environment
        if (!other.isTrigger)
            Destroy(gameObject);
    }
}
