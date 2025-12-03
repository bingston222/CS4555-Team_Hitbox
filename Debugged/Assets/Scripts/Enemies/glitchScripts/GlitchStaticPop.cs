using UnityEngine;
using System.Collections;

public class GlitchStaticPop : EnemyBase
{
    [Header("Explosion")]
    public float triggerDistance = 1.8f;
    public float windupTime = 0.5f;
    public float radius = 4f;
    public float damage = 35f;
    public float knockback = 10f;
    public LayerMask playerMask;

    [Header("VFX / SFX")]
    public GameObject chargeVfx;
    public GameObject explodeVfx;
    public AudioClip chargeSfx;
    public AudioClip explodeSfx;

    bool detonating;
    GameObject chargeInstance;

    protected override void Update()
    {
        base.Update();
        if (!target || detonating) return;

        if (Vector3.Distance(transform.position, target.position) <= triggerDistance)
            StartCoroutine(Detonate());
    }

    // FIXED: match EnemyBase
    protected override void Attack()
    {
        if (!detonating) StartCoroutine(Detonate());
    }

    IEnumerator Detonate()
    {
        detonating = true;
        SafeStopAgent();

        GetComponent<GlitchAnimatorDriver>()?.PlayAttack();

        if (chargeVfx) chargeInstance = Instantiate(chargeVfx, transform.position, Quaternion.identity, transform);
        if (chargeSfx) AudioSource.PlayClipAtPoint(chargeSfx, transform.position, 1f);

        yield return new WaitForSeconds(windupTime);

        Explode();
    }

    void Explode()
    {
        if (chargeInstance) Destroy(chargeInstance);
        if (explodeVfx) Destroy(Instantiate(explodeVfx, transform.position, Quaternion.identity), 3f);
        if (explodeSfx) AudioSource.PlayClipAtPoint(explodeSfx, transform.position, 1f);

        var hits = Physics.OverlapSphere(transform.position, radius, playerMask);
        foreach (var h in hits)
        {
            h.GetComponent<Health>()?.TakeDamage(damage);
            var rb = h.attachedRigidbody;
            if (rb) rb.AddExplosionForce(knockback, transform.position, radius);
        }

        Destroy(gameObject);
    }
}
