using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PuddleDamage : MonoBehaviour
{
    public float damagePerSecond = 6f;

    void Start()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;
        gameObject.tag = "Hazard";
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
            other.GetComponent<Health>()?.TakeDamage(damagePerSecond * Time.deltaTime);
    }
}
