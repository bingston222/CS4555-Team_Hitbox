using UnityEngine;

public class HomingFlameBeam : MonoBehaviour
{
    public float speed = 10f;
    public float turnSpeed = 360f;       // degrees per second
    public float lifetime = 2f;
    public float damagePerSecond = 10f;

    private Transform target;

    // Called right after Instantiate by PulseBeam
    public void Init(Transform target, float dps, float life)
    {
        this.target = target;
        damagePerSecond = dps;
        lifetime = life;
    }

    private void Update()
    {
        lifetime -= Time.deltaTime;
        if (lifetime <= 0f)
        {
            Destroy(gameObject);
            return;
        }

        // Rotate toward target
        if (target != null)
        {
            Vector3 dir = (target.position - transform.position).normalized;
            dir.y = 0f;

            if (dir.sqrMagnitude > 0.0001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    targetRot,
                    turnSpeed * Time.deltaTime
                );
            }
        }

        // Move forward
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        Health hp = other.GetComponent<Health>();
        if (hp != null)
        {
            hp.TakeDamage(damagePerSecond * Time.deltaTime);
        }
    }
}
