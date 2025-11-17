using UnityEngine;

public class HealingPatchProjectile : MonoBehaviour
{
    public float healAmount = 40f;
    public float life = 4f;

    public GameObject owner;         
    public GameObject targetOverride; 

    void Start()
    {
        Destroy(gameObject, life);
    }

    void OnTriggerEnter(Collider other)
    {
        // if override exists -> ignore collisions + heal exactly this target
        if (targetOverride != null)
        {
            Health hp = targetOverride.GetComponent<Health>();
            if (hp != null)
            {
                hp.Heal(healAmount);

                var ult = owner.GetComponent<UltimateCharge>();
                if (ult != null)
                    ult.AddCharge(5f);
            }

            Destroy(gameObject);
            return;
        }

        // normal behavior (if not overridden)
        if (other.gameObject == owner) return;

        Health target = other.GetComponent<Health>();
        if (target != null)
        {
            target.Heal(healAmount);

            var ult = owner.GetComponent<UltimateCharge>();
            if (ult != null)
                ult.AddCharge(5f);
        }

        Destroy(gameObject);
    }
}
