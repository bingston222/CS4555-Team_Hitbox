using UnityEngine;
using System.Collections;

public class CorruptedPatchAttack : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform firePoint;
    public KeyCode key = KeyCode.O;
    public float cooldown = 0.3f;

    bool ready = true;

    void Update()
    {
        if (Input.GetKeyDown(key) && ready)
            StartCoroutine(Fire());
    }

    IEnumerator Fire()
    {
        ready = false;

        GameObject patch = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Projectile proj = patch.GetComponent<Projectile>();

        // Correct delegate type → GameObject
        proj.onHit += (GameObject hit) =>
        {
            // Only give charge if it hit an enemy with Health
            Health enemy = hit.GetComponent<Health>();
            if (enemy != null)
            {
                var ult = GetComponent<UltimateCharge>();
                if (ult != null) ult.AddCharge(8f);
            }
        };

        yield return new WaitForSeconds(cooldown);
        ready = true;
    }
}
