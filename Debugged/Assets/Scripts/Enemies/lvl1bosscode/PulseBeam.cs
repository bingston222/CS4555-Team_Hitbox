using UnityEngine;
using System.Collections;

public class PulseBeam : MonoBehaviour
{
    public bool isFiring = false;
    public float damagePerSecond = 10f;

    [Header("Spawn Settings")]
    public Transform firePoint;
    public float duration = 2f;   // how long the flame lives
    public float cooldown = 5f;   // delay before next shot

    [Header("VFX")]
    public HomingFlameBeam flameBeamPrefab;

    // Called by FirewallOverseer with the current target
    public void TryFire(Transform target)
    {
        if (!isFiring && target != null && flameBeamPrefab != null && firePoint != null)
        {
            StartCoroutine(FireRoutine(target));
        }
    }

    private IEnumerator FireRoutine(Transform target)
    {
        isFiring = true;

        // Spawn the flame projectile
        HomingFlameBeam beam = Instantiate(
            flameBeamPrefab,
            firePoint.position,
            firePoint.rotation
        );

        beam.Init(target, damagePerSecond, duration);

        // Wait until beam lifetime + cooldown
        yield return new WaitForSeconds(duration + cooldown);

        isFiring = false;
    }
}
