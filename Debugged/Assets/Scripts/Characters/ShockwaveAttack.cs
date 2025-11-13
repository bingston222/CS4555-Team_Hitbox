using UnityEngine;
using System.Collections.Generic;

public class ShockwaveAttack : MonoBehaviour
{
    public float startRadius = 0.5f, endRadius = 6f, expandTime = 0.35f, damage = 25f, cooldown = 0.4f;
    public LayerMask enemyMask;

    bool ready = true;

    void Update(){ if (Input.GetMouseButtonDown(0) && ready) StartCoroutine(Fire()); }

    System.Collections.IEnumerator Fire()
    {
        ready = false;
        float t = 0f; var hitOnce = new HashSet<Collider>();
        while (t < expandTime)
        {
            t += Time.deltaTime;
            float r = Mathf.Lerp(startRadius, endRadius, t/expandTime);
            var cols = Physics.OverlapSphere(transform.position, r, enemyMask, QueryTriggerInteraction.Collide);
            foreach (var c in cols)
            {
                if (hitOnce.Add(c))
                {
                    var h = c.GetComponentInParent<Health>() ?? c.GetComponent<Health>();
                    if (h && c.CompareTag("Enemy")) h.TakeDamage(damage);
                }
            }
            yield return null;
        }
        yield return new WaitForSeconds(cooldown);
        ready = true;
    }
}
