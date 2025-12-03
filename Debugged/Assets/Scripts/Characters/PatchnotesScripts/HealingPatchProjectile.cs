using UnityEngine;

public class HealingPatchProjectile : MonoBehaviour
{
    public float lifetime = 4f;
    public float speed = 12f;

    GameObject owner;
    float healAmount;

    public void Init(GameObject owner, Transform target, float healAmount, float speed)
    {
        this.owner = owner;
        this.healAmount = healAmount;
        this.speed = speed;
    }

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
        lifetime -= Time.deltaTime;

        if (lifetime <= 0)
            Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!owner) return;
        if (other.gameObject == owner) return;

        var hp = other.GetComponent<Health>();
        if (hp)
        {
            hp.Heal(healAmount);
            Destroy(gameObject);
        }
    }
}
