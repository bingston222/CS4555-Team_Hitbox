using UnityEngine;

public class ShockwaveAttack : MonoBehaviour
{
    [Header("Input")]
    public KeyCode key = KeyCode.E;

    [Header("Attack Settings")]
    public float cooldown = 0.4f;
    private float cooldownTimer = 0f;

    [Header("Projectile Settings")]
    public GameObject orbPrefab;
    public Transform firePoint;

    private PlayerStatus status;

    private void Start()
    {
        status = GetComponent<PlayerStatus>();
    }

    private void Update()
    {
        if (cooldownTimer > 0)
            cooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(key) && cooldownTimer <= 0f)
        {
            FireShockwaveOrb();
            cooldownTimer = cooldown;
        }
    }

    private void FireShockwaveOrb()
    {
        if (orbPrefab == null || firePoint == null)
        {
            Debug.LogWarning("ShockwaveAttack: Missing orbPrefab or firePoint!");
            return;
        }

        // Spawn the orb
        GameObject orb = Instantiate(orbPrefab, firePoint.position, firePoint.rotation);

        // Assign guaranteed hit into the orb
        var orbProj = orb.GetComponent<ShockwaveOrbProjectile>();
        if (orbProj != null)
            orbProj.guaranteedHit = status.IsGuaranteedHit;
    }
}
