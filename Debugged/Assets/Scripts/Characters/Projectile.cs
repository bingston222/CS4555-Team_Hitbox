using UnityEngine;
using System;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    public float damage = 10f;
    public float life = 5f;

    // NEW: callback for when projectile hits something
    public Action<Health> onHit;

    void Start()
    {
        GetComponent<Collider>().isTrigger = true;
        Destroy(gameObject, life);
    }

    void OnTriggerEnter(Collider other)
    {
        // prevent friendly fire
        if (other.CompareTag("Player")) return;

        // Get health component (supports parent or current object)
        Health h = other.GetComponentInParent<Health>() ?? other.GetComponent<Health>();

        if (h != null)
        {
            // Apply damage
            h.TakeDamage(damage);

            // Trigger hit callback (PatchNotes uses this for ult charge)
            onHit?.Invoke(h);
        }

        Destroy(gameObject);
    }
}
