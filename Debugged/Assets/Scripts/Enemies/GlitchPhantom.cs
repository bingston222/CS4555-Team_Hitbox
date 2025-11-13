using UnityEngine;
using UnityEngine.AI;

public class GlitchPhantom : EnemyBase
{
    [Header("Teleport")]
    public float teleportInterval = 2.5f;
    public float teleportStep = 6f;
    float t;

    protected override void Update()
    {
        base.Update();
        if (!target) return;

        t += Time.deltaTime;
        if (t >= teleportInterval)
        {
            t = 0f;
            Vector3 dir = (target.position - transform.position).normalized;
            Vector3 dest = transform.position + dir * teleportStep;

            if (NavMesh.SamplePosition(dest, out var hit, 2f, NavMesh.AllAreas))
                transform.position = hit.position;
        }
    }
}
