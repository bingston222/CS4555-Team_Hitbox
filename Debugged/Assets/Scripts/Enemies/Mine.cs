using UnityEngine;

public class Mine : MonoBehaviour
{
    public float delay = 2f;
    public float radius = 3f;
    public float damage = 20f;

    void Start()
    {
        Invoke(nameof(Explode), delay);
    }

    void Explode()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, radius);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                Health hp = hit.GetComponent<Health>();
                if (hp) hp.TakeDamage(damage);

                // Optional future use:
                // hit.GetComponent<PlayerStatus>()?.ApplyFreeze(1.5f);
            }

            if (hit.CompareTag("Glitch"))
            {
                // Mines can kill glitches too
                Destroy(hit.gameObject);
            }
        }

        Destroy(gameObject);
    }
}
