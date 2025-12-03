using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
public class Lvl1BossController : MonoBehaviour
{
    private Animator anim;
    private NavMeshAgent agent;
    private Rigidbody rb;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        rb    = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        float speed = 0f;

        if (agent != null && agent.enabled)
        {
            speed = agent.velocity.magnitude;
        }
        else if (rb != null)
        {
            Vector3 vel = rb.linearVelocity;
            vel.y = 0f;
            speed = vel.magnitude;
        }

        anim.SetFloat("Speed", speed);
    }
}
