using UnityEngine;
using UnityEngine.AI;

[DisallowMultipleComponent]
public class GlitchAnimatorDriver : MonoBehaviour
{
    public Animator animator;          // auto-found if left empty
    public NavMeshAgent agent;         // auto-found if left empty

    [Header("Animator params")]
    public string speedParam    = "Speed";   // float
    public string attackTrigger = "Attack";  // trigger
    public string isMovingParam = "";        // optional bool; leave empty if unused

    [Header("Tuning")]
    public float speedScale   = 0.3f;   // maps agent speed (~3.5) -> ~1.0
    public float dampTime     = 0.15f;  // smoothing
    public float moveThreshold= 0.05f;  // tiny deadzone

    void Awake()
    {
        if (!animator) animator = GetComponentInChildren<Animator>();
        if (!agent)    agent    = GetComponent<NavMeshAgent>();
        if (animator)  animator.applyRootMotion = false;
    }

    void OnEnable()
    {
        // ensure we start in idle
        if (animator && !string.IsNullOrEmpty(speedParam))
            animator.SetFloat(speedParam, 0f);
        if (animator && !string.IsNullOrEmpty(isMovingParam))
            animator.SetBool(isMovingParam, false);
    }

    void Update()
    {
        if (!animator || !agent) return;

        // Consider the agent "moving" only if it's on navmesh AND actively traveling.
        bool valid = agent.enabled && agent.isOnNavMesh;
        bool traveling =
            valid &&
            !agent.isStopped &&
            agent.hasPath &&
            agent.remainingDistance > agent.stoppingDistance + 0.05f &&
            agent.desiredVelocity.sqrMagnitude > 0.0001f;

        float spd = 0f;
        if (traveling)
            spd = Mathf.Max(agent.velocity.magnitude, agent.desiredVelocity.magnitude) * speedScale;

        if (!string.IsNullOrEmpty(speedParam))
            animator.SetFloat(speedParam, spd, dampTime, Time.deltaTime);

        if (!string.IsNullOrEmpty(isMovingParam))
            animator.SetBool(isMovingParam, spd > moveThreshold);
    }

    public void PlayAttack()
    {
        if (animator && !string.IsNullOrEmpty(attackTrigger))
            animator.SetTrigger(attackTrigger);
    }
}
