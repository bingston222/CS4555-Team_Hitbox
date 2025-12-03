using UnityEngine;
using UnityEngine.AI;

public class GlitchPhantom : EnemyBase
{
    [Header("Blink")]
    public float interval = 2.0f;
    public float stepDistance = 6f;
    public float minClearance = 1.5f;

    [Header("VFX / SFX")]
    public GameObject blinkOutVfx;
    public GameObject blinkInVfx;
    public AudioClip blinkSfx;

    float t;

    protected override void Update()
    {
        base.Update();
        if (!target || !agent) return;

        t += Time.deltaTime;
        if (t >= interval)
        {
            t = 0f;
            BlinkToward(target.position);
        }

        // Debug key (optional)
      //  if (Input.GetKeyDown(KeyCode.B))
      //  {
       //     BlinkToward(transform.position + transform.forward * 3f);
       // }
    }

    // 🔧 FIXED: must be protected, not public
    protected override void Attack()
    {
        BlinkToward(target.position);
    }

    void BlinkToward(Vector3 goal)
    {
        Vector3 to = goal - transform.position;
        if (to.sqrMagnitude < 0.01f) return;

        float maxStep = Mathf.Max(0f, to.magnitude - minClearance);
        Vector3 dir = to.normalized;
        Vector3 desired = transform.position + dir * Mathf.Min(stepDistance, maxStep);

        if (NavMesh.SamplePosition(desired, out var hit, 2f, NavMesh.AllAreas))
        {
            GetComponent<GlitchAnimatorDriver>()?.PlayAttack();

            if (blinkOutVfx)
                Destroy(Instantiate(blinkOutVfx, transform.position, Quaternion.identity), 2f);

            if (blinkSfx)
                AudioSource.PlayClipAtPoint(blinkSfx, transform.position, 1f);

            agent.Warp(hit.position);

            if (blinkInVfx)
                Destroy(Instantiate(blinkInVfx, transform.position, Quaternion.identity), 2f);
        }
    }
}
