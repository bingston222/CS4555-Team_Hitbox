using UnityEngine;

public class HealingPatchProjectile : MonoBehaviour
{
    public float healAmount = 40f;
    public float life = 4f;

    void Start()
    {
        Destroy(gameObject, life);
    }

    void OnTriggerEnter(Collider other)
    {
        var h = other.GetComponentInParent<Health>() ?? other.GetComponent<Health>();

        if (h)
            h.Heal(healAmount);

        Destroy(gameObject);
    }
}
