using UnityEngine;

[RequireComponent(typeof(Collider))]
public class HealingPatchProjectile : MonoBehaviour
{
    [Header("Motion")]
    public float speed = 16f;
    public float turnRate = 720f;       // how fast it can turn towards target (deg/sec)
    public float life = 4f;

    [Header("Healing")]
    public float healAmount = 20f;

    [Header("Target")]
    public GameObject owner;
    public GameObject targetOverride;   // if set, will home to this target

    Vector3 travelDir;                  // used when no target set

    public void Init(GameObject owner, GameObject target, float healAmount, float speed)
    {
        this.owner = owner;
        this.targetOverride = target;
        this.healAmount = healAmount;
        this.speed = speed;

        // initial direction is forward at spawn
        travelDir = transform.forward;

        // ensure trigger collider
        var col = GetComponent<Collider>();
        col.isTrigger = true;

        Destroy(gameObject, life);
    }

    void Update()
    {
        if (targetOverride)
        {
            // home towards target (aim at chest-ish height)
            Vector3 aimPoint = targetOverride.transform.position + Vector3.up * 1.0f;
            Vector3 toTarget = (aimPoint - transform.position).normalized;

            // smooth turning
            float maxRadians = turnRate * Mathf.Deg2Rad * Time.deltaTime;
            travelDir = Vector3.RotateTowards(travelDir, toTarget, maxRadians, 0f);
        }

        transform.position += travelDir * speed * Time.deltaTime;
        if (travelDir.sqrMagnitude > 0.0001f)
            transform.rotation = Quaternion.LookRotation(travelDir, Vector3.up);
    }

    void OnTriggerEnter(Collider other)
    {
        // ignore owner
        if (other.attachedRigidbody && owner && other.attachedRigidbody.gameObject == owner)
            return;
        if (owner && other.gameObject == owner) return;

        // if we had an explicit target, only heal that on contact
        if (targetOverride != null)
        {
            if (other.gameObject != targetOverride) return;

            var hp = targetOverride.GetComponent<Health>();
            if (hp != null) hp.Heal(healAmount);

            // optional: ult charge for owner
            var ult = owner ? owner.GetComponent<UltimateCharge>() : null;
            if (ult) ult.AddCharge(5f);

            Destroy(gameObject);
            return;
        }

        // otherwise: heal any valid Health we touch (not the owner)
        var target = other.GetComponent<Health>();
        if (target && (!owner || other.gameObject != owner))
        {
            target.Heal(healAmount);

            var ult = owner ? owner.GetComponent<UltimateCharge>() : null;
            if (ult) ult.AddCharge(5f);

            Destroy(gameObject);
        }
    }
}
