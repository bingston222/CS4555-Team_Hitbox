using UnityEngine;
using UnityEngine.AI;

public class FirewallOverseer : BossBase
{
    [Header("Abilities")]
    public PulseBeam pulseBeam;          // Only ability for now

    [Header("Movement / Targeting")]
    public Transform[] targets;          // Player 1 + Player 2
    public NavMeshAgent agent;
    public float chaseRange = 25f;
    public float attackRange = 15f;
    public float attackCooldown = 5f;
    public float turnSpeed = 5f;         // how quickly he rotates during attack

    [Header("Activation")]
    public bool autoActivateOnEnable = true;

    // internal state
    private bool isActive = false;
    private float attackTimer = 0f;
    private Transform currentTarget;

    private void Awake()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();
    }

    private void OnEnable()
    {
        if (autoActivateOnEnable)
            ActivateBoss();
    }

    private void Start()
    {
        if (autoActivateOnEnable && !isActive)
            ActivateBoss();
    }

    public void ActivateBoss()
    {
        isActive = true;
        attackTimer = 1f; // small delay before first attack
    }

    private void Update()
    {
        if (!isActive)
            return;

        if (agent != null && !agent.isOnNavMesh)
            return;

        UpdateCurrentTarget();
        if (currentTarget == null)
            return;

        attackTimer -= Time.deltaTime;
        float dist = Vector3.Distance(transform.position, currentTarget.position);

        // ---------- MOVEMENT (chase nearest player) ----------
        if (!pulseBeam.isFiring && agent != null)
        {
            if (dist <= chaseRange)
            {
                agent.isStopped = false;
                agent.SetDestination(currentTarget.position);
            }
            else
            {
                agent.isStopped = true;
            }
        }

        // ---------- ATTACK (Pulse Beam) ----------
        if (dist <= attackRange && attackTimer <= 0f && !pulseBeam.isFiring)
        {
            if (agent != null)
                agent.isStopped = true;

            // Smoothly rotate toward the player on the horizontal plane
            Vector3 dir = currentTarget.position - transform.position;
            dir.y = 0f;

            if (dir.sqrMagnitude > 0.0001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRot,
                    turnSpeed * Time.deltaTime
                );
            }

            // Fire the homing flame towards that target
            pulseBeam.TryFire(currentTarget);

            attackTimer = attackCooldown;
        }
    }

    private void UpdateCurrentTarget()
    {
        float bestDist = Mathf.Infinity;
        Transform best = null;

        if (targets == null || targets.Length == 0)
        {
            currentTarget = null;
            return;
        }

        foreach (Transform t in targets)
        {
            if (t == null) continue;

            float d = Vector3.Distance(transform.position, t.position);
            if (d < bestDist)
            {
                bestDist = d;
                best = t;
            }
        }

        currentTarget = best;
    }

    // We don't need weakening behavior yet, so leave this empty.
    protected override void OnWeakened() { }
}
