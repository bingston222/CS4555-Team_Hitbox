using UnityEngine;

public class GlitchStaticPop : EnemyBase
{
    [Header("Explosion")]
    public float explodeRadius = 4f;
    public float explodeDamage = 35f;

    protected override void Update()
    {
        base.Update();
        if (!target) return;

        // If close enough, explode immediately
        if (Vector3.Distance(transform.position, target.position) <= 1.8f)
            Explode();
    }

    protected override void Attack()
    {
        // Optionally “detonate” when in attack range
        Explode();
    }

    void Explode()
    {
        var cols = Physics.OverlapSphere(transform.position, explodeRadius, ~0, QueryTriggerInteraction.Collide);
        foreach (var c in cols)
            if (c.CompareTag("Player"))
                c.GetComponent<Health>()?.TakeDamage(explodeDamage);

        Destroy(gameObject);
    }
}
