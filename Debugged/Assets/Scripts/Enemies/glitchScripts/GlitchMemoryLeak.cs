using UnityEngine;
using System.Collections;

public class GlitchMemoryLeak : EnemyBase
{
    [Header("Puddle")]
    public GameObject puddlePrefab;  // has PuddleDamage
    public float puddleLife = 6f;
    public float dps = 8f;
    public float castWindup = 0.25f;

    [Header("VFX / SFX")]
    public GameObject castVfx;
    public AudioClip  castSfx;

    protected override void Attack()
    {
        if (!puddlePrefab) return;
        StartCoroutine(CastRoutine());
    }

    IEnumerator CastRoutine()
    {
        // Use the SAME Attack anim as your cast
        GetComponent<GlitchAnimatorDriver>()?.PlayAttack();

        if (castVfx) Destroy(Instantiate(castVfx, transform.position, Quaternion.identity), 2f);
        if (castSfx) AudioSource.PlayClipAtPoint(castSfx, transform.position, 1f);

        yield return new WaitForSeconds(castWindup);

        Vector3 pos = transform.position; // or slight forward offset
        var p = Instantiate(puddlePrefab, pos, Quaternion.identity);
        var dmg = p.GetComponent<PuddleDamage>() ?? p.AddComponent<PuddleDamage>();
        dmg.damagePerSecond = dps;
        Destroy(p, puddleLife);
    }
}
