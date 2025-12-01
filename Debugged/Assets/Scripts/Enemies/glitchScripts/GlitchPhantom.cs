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

        // Regular timed blink
        t += Time.deltaTime;
        if (t >= interval)
        {
            t = 0f;
            BlinkToward(target.position);
        }

        // ===================================================
        // TEMPORARY TESTING KEY
        // Press B to test blink VFX manually
        // Remove this block once testing is done.
        // ===================================================
        if (Input.GetKeyDown(KeyCode.B))
        {
            Vector3 testPos = transform.position + transform.forward * 3f;
            BlinkToward(testPos);
        }
    }

    void BlinkToward(Vector3 goal)
    {
        Vector3 to = goal - transform.position;
        if (to.sqrMagnitude < 0.01f) return;

        float maxStep = Mathf.Max(0f, to.magnitude - minClearance);
        Vector3 dir = to.normalized;
        Vector3 desired = transform.position + dir * Mathf.Min(stepDistance, maxStep);

        // Validate teleport location on NavMesh
        if (NavMesh.SamplePosition(desired, out var hit, 2f, NavMesh.AllAreas))
        {
            // Blink animation trigger
            GetComponent<GlitchAnimatorDriver>()?.PlayAttack();

            // Blink OUT effect
            if (blinkOutVfx)
                Destroy(Instantiate(blinkOutVfx,
                    transform.position,
                    Quaternion.identity),
                    2f);

            // Blink sound
            if (blinkSfx)
                AudioSource.PlayClipAtPoint(blinkSfx,
                    transform.position,
                    1f);

            // TELEPORT
            agent.Warp(hit.position);

            // Blink IN effect
            if (blinkInVfx)
                Destroy(Instantiate(blinkInVfx,
                    transform.position,
                    Quaternion.identity),
                    2f);
        }
    }
}
