using UnityEngine;
using System.Collections;

public class PulseBeam : MonoBehaviour
{
    public bool isFiring = false;
    public float damagePerSecond = 10f;
    public LineRenderer beam;
    public Transform firePoint;
    public float duration = 2f;

    public void TryFire()
    {
        if (!isFiring)
            StartCoroutine(FireBeam());
    }

    IEnumerator FireBeam()
    {
        isFiring = true;
        beam.enabled = true;

        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;

            if (Physics.Raycast(firePoint.position, firePoint.forward, out RaycastHit hit))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    Health hp = hit.collider.GetComponent<Health>();
                    PlayerStatus status = hit.collider.GetComponent<PlayerStatus>();

                    if (hp) hp.TakeDamage(damagePerSecond * Time.deltaTime);
                    if (status) status.ApplyLag(3f);
                }

                beam.SetPosition(0, firePoint.position);
                beam.SetPosition(1, hit.point);
            }

            yield return null;
        }

        beam.enabled = false;
        isFiring = false;
    }
}
