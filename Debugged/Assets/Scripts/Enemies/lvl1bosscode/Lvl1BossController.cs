using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
public class Lvl1BossController : MonoBehaviour
{
    public PulseBeam pulseBeam; // drag your PulseBeam component here in Inspector

    private Animator anim;
    private NavMeshAgent agent;   // optional if you use NavMesh

    void Awake()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();   // can be null if you don't use it
    }

    void Update()
    {
        // ----- locomotion (idle / walk) -----
        float speed = 0f;

        if (agent != null)
        {
            speed = agent.velocity.magnitude;
        }
        else
        {
            // if you're moving with Rigidbody or manually:
            Vector3 vel = GetComponent<Rigidbody>()?.linearVelocity ?? Vector3.zero;
            vel.y = 0;
            speed = vel.magnitude;
        }

        anim.SetFloat("Speed", speed);
    }

    // Call this from your AI when you want to fire the beam
    public void FirePulseBeam()
    {
        if (pulseBeam != null && !pulseBeam.isFiring)
        {
            anim.SetTrigger("FirePulse");  // goes to pulseBeam state
            // actual beam start is triggered by an animation event below
        }
    }

    // This is called by an Animation Event on the pulseBeam clip
    public void StartPulseFromAnim()
    {
        if (pulseBeam != null)
            pulseBeam.StartBeam();
    }
}
