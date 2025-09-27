using UnityEngine;

public class EnemyChaseAndAttack : MonoBehaviour
{
    [Header("Chase")]
    public Transform target;          
    public float moveSpeed = 2.5f;
    public float stopDistance = 1.2f;   // how close to approach
    public float disengageDistance = 6f; // how far away before it stops attacking

    [Header("Attack")]
    public int damage = 25;
    public float attackCooldown = 1.0f; 

    float _lastAttackTime;

    void Start()
    {
        if (target == null)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) target = player.transform;
        }
    }

    void Update()
    {
        if (!target) return;

        Vector3 toPlayer = target.position - transform.position;
        toPlayer.y = 0f;

        float dist = toPlayer.magnitude;

        // face the player
        if (dist > 0.001f)
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(toPlayer),
                Time.deltaTime * 8f
            );

        // chase only if closer than disengageDistance
        if (dist <= disengageDistance && dist > stopDistance)
        {
            Vector3 step = toPlayer.normalized * moveSpeed * Time.deltaTime;
            transform.position += step;
        }
    }

    void OnTriggerStay(Collider other)
    {  
        if (!other.CompareTag("Player")) return;
        Debug.Log("Enemy touching player"); 
        if (Time.time - _lastAttackTime < attackCooldown) return;

        float dist = Vector3.Distance(other.transform.position, transform.position);
        if (dist > disengageDistance) return; // too far away, don't attack

        var hp = other.GetComponent<PlayerHealth>();
        if (hp != null)
        {
            hp.TakeDamage(damage);
            _lastAttackTime = Time.time;
        }
    }
}
