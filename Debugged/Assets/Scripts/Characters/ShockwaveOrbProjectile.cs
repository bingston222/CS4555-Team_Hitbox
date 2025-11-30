using UnityEngine;

public class ShockwaveOrbProjectile : MonoBehaviour
{
    public float speed = 12f;
    public float lifetime = 2f;
    public float damage = 10f;

    // This is set by ShockwaveAttack.cs when ultimate is active
    [HideInInspector]
    public bool guaranteedHit = false;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy"))
            return;

        Health hp = other.GetComponent<Health>();
        if (hp == null)
            return;

        // INSTANT AUTO-HIT DURING ULTIMATE
        if (guaranteedHit)
        {
            hp.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        // Normal shockwave hit
        hp.TakeDamage(damage);
        Destroy(gameObject);
    }
}
