using UnityEngine;
using System.Collections;

public class HitboxAura : MonoBehaviour
{
    Transform tf;
    void Awake() { tf = transform; }

    public void Play(Transform follow, float duration, float targetScale)
    {
        StartCoroutine(Co(follow, duration, targetScale));
    }

    // HitboxAura.cs
IEnumerator Co(Transform follow, float duration, float targetScale)
{
    float t = 0f;
    tf.localScale = Vector3.one;

    var rend = GetComponent<Renderer>();
    var startColor = rend ? rend.material.color : Color.white;
    var faded = new Color(startColor.r, startColor.g, startColor.b, 0f);

    while (t < duration)
    {
        // ✅ if enemy died, bail out and destroy the aura
        if (follow == null) break;

        tf.position = follow.position;
        float k = Mathf.Clamp01(t / 0.25f);
        float s = Mathf.Lerp(1f, targetScale, k);
        tf.localScale = new Vector3(s, s, s);

        // fade near the end
        float fadeT = Mathf.InverseLerp(duration * 0.8f, duration, t);
        if (rend) rend.material.color = Color.Lerp(startColor, faded, fadeT);

        t += Time.deltaTime;
        yield return null;
    }
    Destroy(gameObject);
}

}
