using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 3f;

    // Who fired this projectile
    public GameObject owner;

    // Optional damage value — only used for damaging projectiles
    public int damage = 0;

    // Callback to tell whatever ability fired this projectile WHAT it hit
    public System.Action<GameObject> onHit;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (rb != null)
            rb.linearVelocity = transform.forward * speed;

        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Ignore hitting yourself
        if (other.gameObject == owner)
            return;

        // If the shooter wants to handle the hit (healing, debuff, etc.)
        if (onHit != null)
            onHit(other.gameObject);

        // If the thing we hit has health, apply damage
        Health hp = other.GetComponent<Health>();
        if (hp != null && damage > 0)
        {
            hp.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
