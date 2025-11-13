using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Health))]
public class EnemyBase : MonoBehaviour
{
    [Header("Senses / Combat")]
    public float sightRange = 18f;
    public float attackRange = 8f;
    public float attackCooldown = 1.5f;
    public float contactDamage = 0f;   // optional bump damage

    protected Transform target;
    protected NavMeshAgent agent;
    protected Health health;
    float cd;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        health = GetComponent<Health>();
        target = GameObject.FindGameObjectWithTag("Player")?.transform;
        health.onDeath += () => Destroy(gameObject);
    }

    protected virtual void Update()
    {
        if (!target) return;
        cd -= Time.deltaTime;

        float d = Vector3.Distance(transform.position, target.position);

        if (d > sightRange) { agent.isStopped = true; return; }

        if (d > attackRange)
        {
            agent.isStopped = false;
            agent.SetDestination(target.position);
        }
        else
        {
            agent.isStopped = true;
            if (cd <= 0f) { cd = attackCooldown; Attack(); }
        }
    }

    protected virtual void Attack() { } // override in children

    void OnCollisionEnter(Collision c)
    {
        if (contactDamage > 0f && c.gameObject.CompareTag("Player"))
            c.gameObject.GetComponent<Health>()?.TakeDamage(contactDamage);
    }
}
