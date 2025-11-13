using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    public float damage = 10f;
    public float life = 5f;

    void Start()
    {
        var col = GetComponent<Collider>(); col.isTrigger = true;
        Destroy(gameObject, life);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy")) return; // don't friendly fire
        var h = other.GetComponentInParent<Health>() ?? other.GetComponent<Health>();
        if (h) h.TakeDamage(damage);
        Destroy(gameObject);
    }
}

