using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StripeSpawnerUI : MonoBehaviour
{
    [Header("Timing")]
    public Vector2 waitRange = new Vector2(0.8f, 2.2f);
    public float life = 0.25f;

    [Header("Stripe")]
    public float minHeight = 6f;
    public float maxHeight = 28f;
    public float minSpeed = 800f;
    public float maxSpeed = 1700f;
    public Color[] palette; // assign magenta/cyan/blue/violet

    RectTransform rt;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        if (palette == null || palette.Length == 0)
        {
            palette = new [] {
                new Color(1f, 0.2f, 0.9f, 0.6f),   // neon magenta
                new Color(0.1f, 1f, 1f, 0.6f),     // cyan
                new Color(0.4f, 0.6f, 1f, 0.55f),  // blue
                new Color(0.9f, 0.2f, 0.4f, 0.55f) // pink-red
            };
        }
    }

    void OnEnable() { StartCoroutine(Loop()); }

    IEnumerator Loop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(waitRange.x, waitRange.y));
            StartCoroutine(SpawnStripe());
        }
    }

    IEnumerator SpawnStripe()
    {
        float h = Random.Range(minHeight, maxHeight);
        float y = Random.Range(0, rt.rect.height - h);
        float speed = Random.Range(minSpeed, maxSpeed);
        Color c = palette[Random.Range(0, palette.Length)];

        var go = new GameObject("stripe", typeof(RectTransform), typeof(Image));
        var srt = go.GetComponent<RectTransform>();
        srt.SetParent(rt, false);
        srt.anchorMin = new Vector2(0, 0);
        srt.anchorMax = new Vector2(0, 0);
        srt.sizeDelta = new Vector2(rt.rect.width * 0.35f, h); // short block
        srt.anchoredPosition = new Vector2(-srt.sizeDelta.x, y);

        var img = go.GetComponent<Image>();
        img.color = c;

        float t = 0f;
        float maxX = rt.rect.width + srt.sizeDelta.x;
        while (t < life)
        {
            t += Time.unscaledDeltaTime;
            srt.anchoredPosition += new Vector2(speed * Time.unscaledDeltaTime, 0);
            // fade
            float a = Mathf.Clamp01(1f - (t / life));
            img.color = new Color(c.r, c.g, c.b, c.a * a);
            if (srt.anchoredPosition.x > maxX) break;
            yield return null;
        }
        Destroy(go);
    }
}
