using UnityEngine;

public class GlitchMemoryLeak : EnemyBase
{
    [Header("Puddle")]
    public GameObject puddlePrefab;
    public float puddleLife = 6f;
    public float dps = 8f;

    protected override void Attack()
    {
        if (!puddlePrefab) return;
        var p = Instantiate(puddlePrefab, transform.position, Quaternion.identity);
        var dmg = p.GetComponent<PuddleDamage>() ?? p.AddComponent<PuddleDamage>();
        dmg.damagePerSecond = dps;
        Destroy(p, puddleLife);
    }
}
