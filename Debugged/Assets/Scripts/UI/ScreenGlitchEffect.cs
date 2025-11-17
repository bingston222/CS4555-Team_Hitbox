using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenGlitchEffect : MonoBehaviour
{
    public RectTransform target;
    public Image colorOverlay;

    public float glitchInterval = 3f;
    public float glitchDuration = 0.3f;
    public float shakeIntensity = 20f;
    public float colorIntensity = 0.7f;

    private Vector2 originalPos;
    private Vector3 originalScale;

    void Start()
    {
        if (target == null)
            target = GetComponent<RectTransform>();

        originalPos = target.anchoredPosition;
        originalScale = target.localScale;

        StartCoroutine(GlitchLoop());
    }

    IEnumerator GlitchLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(glitchInterval);

            float timer = glitchDuration;

            while (timer > 0f)
            {
                timer -= Time.deltaTime;

                // position shake
                Vector2 shakeOffset = new Vector2(
                    Random.Range(-shakeIntensity, shakeIntensity),
                    Random.Range(-shakeIntensity, shakeIntensity)
                );
                target.anchoredPosition = originalPos + shakeOffset;

                // scale shake
                float scaleMod = Random.Range(0.97f, 1.03f);
                target.localScale = originalScale * scaleMod;

                // COLOR FLICKER ⭐️
                colorOverlay.color = new Color(
                    Random.Range(0f, 1f),
                    Random.Range(0f, 1f),
                    Random.Range(0f, 1f),
                    Random.Range(0f, colorIntensity) // alpha
                );

                yield return null;
            }

            // reset everything
            target.anchoredPosition = originalPos;
            target.localScale = originalScale;
            colorOverlay.color = new Color(0, 0, 0, 0);
        }
    }
}
