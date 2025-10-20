using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public int damage = 10;
    public float lifetime = 5f;

    SphereCollider col;
    Rigidbody rb;

    void Awake()
    {
        col = GetComponent<SphereCollider>();
        if (col == null) col = gameObject.AddComponent<SphereCollider>();
        col.isTrigger = true;                

        rb = GetComponent<Rigidbody>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic; // better at high speed
    }

    void Start() => Destroy(gameObject, lifetime);

    void OnTriggerEnter(Collider other)
    {
        // Hit the player?
        if (!other.CompareTag("Player")) return;

        var stats = other.GetComponent<CharacterStats>();
        if (stats != null) stats.TakeDamage(damage);

        Destroy(gameObject);
    }
}
