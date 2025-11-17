using UnityEngine;
using System;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float speed = 12f;
    public float lifetime = 4f;
    public float damage = 10f;

    Rigidbody rb;

    // Called when projectile hits an enemy
    public Action<Health> onHit;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (!rb)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.useGravity = false;
        }

        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        Destroy(gameObject, lifetime);
    }

    void FixedUpdate()
    {
        rb.linearVelocity = transform.forward * speed;
    }

    void OnCollisionEnter(Collision col)
    {
        Health hp = col.collider.GetComponent<Health>();

        // If target has Health → damage them
        if (hp)
        {
            hp.TakeDamage(damage);

            // Tell listeners this projectile hit someone
            onHit?.Invoke(hp);
        }

        // Destroy projectile on ANY collision
        Destroy(gameObject);
    }
}
