using UnityEngine;
using UnityEngine.AI;

public class EnemyBase : MonoBehaviour
{
    [Header("Targeting")]
    public Transform target;

    [Header("Combat")]
    public float attackRange = 2f;
    public float attackCooldown = 2f;
    protected float attackTimer = 0f;

    protected NavMeshAgent agent;
    protected Health health;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        health = GetComponent<Health>();

        // Death behavior
        if (health)
            health.onDeath += OnDeath;
    }

    protected virtual void Start()
    {
        // Auto-find player by tag
        var p = GameObject.FindWithTag("Player");
        if (p != null)
            target = p.transform;
    }

    protected virtual void Update()
    {
        if (attackTimer > 0f)
            attackTimer -= Time.deltaTime;

        if (target == null) return;
        if (agent == null) return;

        agent.SetDestination(target.position);

        if (Vector3.Distance(transform.position, target.position) <= attackRange)
            TryAttack();
    }

    protected virtual void TryAttack()
    {
        if (attackTimer > 0f) return;

        Attack();
        attackTimer = attackCooldown;
    }

    // Overriden in child glitch classes
    protected virtual void Attack() { }

    protected virtual void OnDeath()
    {
        SafeStopAgent();
        Destroy(gameObject);
    }

    // ---- Required by your glitch enemy scripts ----
    protected virtual void SafeStopAgent()
    {
        if (agent == null) return;

        agent.ResetPath();
        agent.velocity = Vector3.zero;
    }
}
