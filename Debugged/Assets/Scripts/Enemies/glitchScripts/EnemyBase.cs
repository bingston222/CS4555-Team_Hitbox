using UnityEngine;
using UnityEngine.AI;
using System.Linq;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Health))]
public class EnemyBase : MonoBehaviour
{
    [Header("Senses / Combat")]
    public float sightRange = 18f;
    public float attackRange = 8f;
    public float attackCooldown = 1.5f;
    public float contactDamage = 0f;

    [Header("Targeting")]
    public float retargetInterval = 0.5f;   // how often to reconsider target

    protected Transform target;
    protected NavMeshAgent agent;
    protected Health health;

    float cd;
    float retargetTimer;

    // ------------------------------
    // PATCH NOTES ULTIMATE SUPPORT
    // ------------------------------
    PlayerStatus charmer; // player who activated System Restore

    protected virtual void Awake()
    {
        agent  = GetComponent<NavMeshAgent>();
        health = GetComponent<Health>();

        AcquireTarget(true);

        health.onDeath += () => Destroy(gameObject);
    }

    protected virtual void Update()
    {
        // ---------------------------------------------------
        // CHARM CHECK — SYSTEM RESTORE ULTIMATE ACTIVE
        // ---------------------------------------------------
        if (charmer && charmer.turnEnemiesFriendly)
        {
            // Stop all movement + attacking
            SafeStopAgent();
            cd = attackCooldown; // prevent queued attacks

            return; // Exit Update — enemy does nothing while charmed
        }
        else
        {
            // Charm ended — ensure normal behavior resumes
            if (charmer && !charmer.turnEnemiesFriendly)
                charmer = null;
        }
        // ---------------------------------------------------

        // periodic re-target (and also if current target died/vanished/out of range)
        retargetTimer -= Time.deltaTime;
        if (retargetTimer <= 0f || !IsValidTarget(target))
        {
            AcquireTarget(false);
            retargetTimer = retargetInterval;
        }

        if (!target) { SafeStopAgent(); return; }

        cd -= Time.deltaTime;

        float d = Vector3.Distance(transform.position, target.position);

        if (d > sightRange)
        {
            SafeStopAgent();
            return;
        }

        if (d > attackRange)
        {
            SafeResumeAgent();
            SafeSetDestination(target.position);
        }
        else
        {
            SafeStopAgent();
            if (cd <= 0f)
            {
                cd = attackCooldown;
                Attack();
            }
        }
    }

    protected virtual void Attack() { /* override */ }

    void OnCollisionEnter(Collision c)
    {
        if (contactDamage > 0f && c.gameObject.CompareTag("Player"))
            c.gameObject.GetComponent<Health>()?.TakeDamage(contactDamage);
    }

    // ---------- Target acquisition ----------
    void AcquireTarget(bool initial)
    {
        // choose nearest living player within (or just outside) sight range
        Transform best = null;
        float bestDist = float.MaxValue;

        foreach (var p in PlayerLocator.Players)
        {
            if (!IsValidTarget(p)) continue;

            float dist = Vector3.Distance(transform.position, p.position);
            if (dist < bestDist && dist <= sightRange * 1.25f)
            {
                best = p;
                bestDist = dist;
            }
        }

        target = best;

        // ---------------------------------------------
        // SYSTEM RESTORE ULT — DETECT IF THIS PLAYER IS THE CHARMER
        // ---------------------------------------------
        if (target)
        {
            var ps = target.GetComponent<PlayerStatus>();
            if (ps && ps.turnEnemiesFriendly)
            {
                charmer = ps; // store who charmed them
            }
        }
        // ---------------------------------------------

        if (initial)
            Debug.Log($"{name} target {(target ? target.name : "NONE")} (players:{PlayerLocator.Players.Count})", this);
    }

    bool IsValidTarget(Transform t)
    {
        if (!t) return false;
        var hp = t.GetComponent<Health>();
        if (hp && hp.CurrentHP <= 0f) return false;
        return true;
    }

    // ---------- SAFE AGENT HELPERS ----------
    protected void SafeStopAgent()
    {
        if (agent && agent.enabled && agent.isOnNavMesh && !agent.isOnOffMeshLink)
        {
            agent.isStopped = true;
            agent.ResetPath();
            agent.velocity = Vector3.zero;
        }
    }

    protected void SafeResumeAgent()
    {
        if (agent && agent.enabled && agent.isOnNavMesh && !agent.isOnOffMeshLink)
            agent.isStopped = false;
    }

    protected void SafeSetDestination(Vector3 dest)
    {
        if (agent && agent.enabled && agent.isOnNavMesh)
        {
            if (NavMesh.SamplePosition(dest, out var hit, 2f, NavMesh.AllAreas))
                agent.SetDestination(hit.position);
        }
    }
}
