using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 3f;

    public GameObject owner;

    public int damage = 0;

    public System.Action<GameObject> onHit;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (rb != null)
            rb.linearVelocity = transform.forward * speed;

        Destroy(gameObject, lifetime);
    }

    public void SetDamage(int dmg)
    {
        damage = dmg;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == owner)
            return;

        if (onHit != null)
            onHit(other.gameObject);

        Health hp = other.GetComponent<Health>();
        if (hp != null && damage > 0)
        {
            hp.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
