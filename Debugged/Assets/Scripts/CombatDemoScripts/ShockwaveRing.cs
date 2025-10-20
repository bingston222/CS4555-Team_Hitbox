using UnityEngine;
using System.Collections.Generic;
using System.Collections;


public class ShockwaveRing : MonoBehaviour
{
    [Header("Visual")]
    public float duration = 0.35f;
    public float startRadius = 0.25f;
    public float endRadius = 4.0f;   // how far the ring grows
    public AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0,0,1,1);
    public AnimationCurve alphaCurve = AnimationCurve.EaseInOut(0,1,1,0);

    [Header("Damage")]
    public int damage = 15;
    public LayerMask enemyMask = ~0;
    public bool damageOncePerEnemy = true;

    HashSet<EnemyHealth> alreadyHit = new HashSet<EnemyHealth>();
    Transform tf;
    Material mat;
    Color baseColor;

    void Awake()
    {
        tf = transform;
        var rend = GetComponent<Renderer>();
        if (rend != null)
        {
            mat = rend.material; // instance
            baseColor = mat.color;
        }
        // keep flat on ground
        var p = tf.position; p.y = 0.01f; tf.position = p;
    }

    public void Play()
    {
        StopAllCoroutines();
        StartCoroutine(Co());
    }

    IEnumerator Co()
    {
        float t = 0f;
        while (t < duration)
        {
            float k = t / duration;
            float r = Mathf.Lerp(startRadius, endRadius, scaleCurve.Evaluate(k));
            // scale X & Z to diameter (radius*2)
            tf.localScale = new Vector3(r * 2f, 0.02f, r * 2f);

            // fade alpha
            if (mat != null)
            {
                var c = baseColor; c.a *= alphaCurve.Evaluate(k);
                mat.color = c;
            }

            // apply damage to enemies inside current radius
            var hits = Physics.OverlapSphere(tf.position, r, enemyMask, QueryTriggerInteraction.Collide);
            foreach (var h in hits)
            {
                var eh = h.GetComponentInParent<EnemyHealth>();
                if (eh != null && eh.IsAlive)
                {
                    if (damageOncePerEnemy)
                    {
                        if (alreadyHit.Add(eh)) eh.TakeDamage(damage);
                    }
                    else
                    {
                        eh.TakeDamage(damage);
                    }
                }
            }

            t += Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }
}
