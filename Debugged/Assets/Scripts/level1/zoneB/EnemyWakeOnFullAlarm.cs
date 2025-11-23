using UnityEngine;
using UnityEngine.AI;

public class EnemyWakeOnFullAlarm_Polling : MonoBehaviour
{
    [Header("Targets")]
    [SerializeField] private Transform player; // leave empty if your player root has the "Player" tag

    [Header("Movement")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private float repathInterval = 0.2f;
    [SerializeField] private float stopDistance   = 1.2f;

    [Header("Hard Lock Until Full Alarm")]
    [Tooltip("Patrol/AI/Attack scripts to keep OFF until alarm 2.")]
    [SerializeField] private MonoBehaviour[] disableUntilFullAlarm;

    [Tooltip("Weapon/hitbox colliders to keep OFF until alarm 2.")]
    [SerializeField] private Collider[] disableCollidersUntilFull;

    [Tooltip("Prevent root-motion/auto-attack before wake.")]
    [SerializeField] private bool lockAnimatorUntilFull = true;
    [SerializeField] private Animator animator;
    [SerializeField] private string speedParam = "Speed";
    [SerializeField] private string attackBool = "Attack";

    [Header("Debug")]
    [SerializeField] private bool debugLogs = false;

    private bool chasing;
    private float lastPath;

    void Reset()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }

    void Awake()
    {
        if (!agent) agent = GetComponent<NavMeshAgent>();
        if (!player)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
        }
    }

    void OnEnable()
    {
        SetIdleHard();
    }

    void Update()
    {
        // 1) If not yet chasing, poll AlarmManager and wake when level hits max
        if (!chasing)
        {
            var am = AlarmManager.Instance;
            if (am != null && am.AlertLevel >= am.MaxAlarms)
            {
                if (debugLogs) Debug.Log($"[EnemyWakeOnFullAlarm_Polling] Waking {name} (Alert={am.AlertLevel}/{am.MaxAlarms})");
                StartChase();
            }
            else
            {
                // stay fully locked
                return;
            }
        }

        // 2) Chasing logic
        if (!player || !agent || !agent.enabled) return;

        if (Time.time - lastPath >= repathInterval)
        {
            lastPath = Time.time;
            agent.SetDestination(player.position);
        }

        bool close = (Vector3.SqrMagnitude(transform.position - player.position) <= stopDistance * stopDistance);
        agent.isStopped = close;

        if (animator && lockAnimatorUntilFull)
        {
            if (!string.IsNullOrEmpty(speedParam)) animator.SetFloat(speedParam, agent.velocity.magnitude);
            if (!string.IsNullOrEmpty(attackBool)) animator.SetBool(attackBool, false);
        }
    }

    private void StartChase()
    {
        if (agent)
        {
            agent.enabled = true;
            agent.isStopped = false;
            agent.ResetPath();
        }

        if (disableUntilFullAlarm != null)
            foreach (var mb in disableUntilFullAlarm) if (mb) mb.enabled = true;

        if (disableCollidersUntilFull != null)
            foreach (var c in disableCollidersUntilFull) if (c) c.enabled = true;

        if (animator && lockAnimatorUntilFull)
        {
            animator.enabled = true;
            animator.applyRootMotion = false; // keep navmesh in charge
            if (!string.IsNullOrEmpty(attackBool)) animator.SetBool(attackBool, false);
        }

        chasing = true;
        lastPath = -999f;
    }

    private void SetIdleHard()
    {
        if (agent)
        {
            if (agent.enabled)
            {
                agent.isStopped = true;
                agent.ResetPath();
            }
            agent.enabled = false; // blocks any movement
        }

        if (disableUntilFullAlarm != null)
            foreach (var mb in disableUntilFullAlarm) if (mb) mb.enabled = false;

        if (disableCollidersUntilFull != null)
            foreach (var c in disableCollidersUntilFull) if (c) c.enabled = false;

        var rb = GetComponent<Rigidbody>();
        if (rb) rb.linearVelocity = Vector3.zero;

        if (animator && lockAnimatorUntilFull)
        {
            animator.enabled = true; // hold pose
            animator.applyRootMotion = false;
            if (!string.IsNullOrEmpty(speedParam)) animator.SetFloat(speedParam, 0f);
            if (!string.IsNullOrEmpty(attackBool)) animator.SetBool(attackBool, false);
        }

        chasing = false;
    }
}
