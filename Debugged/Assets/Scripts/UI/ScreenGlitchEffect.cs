using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenGlitchEffect : MonoBehaviour
{
    [Header("UI Targets")]
    public RectTransform target;
    public Image colorOverlay;

    [Header("Glitch Settings")]
    public float glitchInterval = 2f;
    public float glitchDuration = 0.2f;
    public float shakeIntensity = 10f;
    public float colorIntensity = 0.3f;

    [Header("Timing")]
    public float totalGlitchTime = 12f;
    public bool glitchActive = true;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip[] glitchSounds;

    private Vector2 originalPos;
    private Vector3 originalScale;

    void Start()
    {
        if (target == null)
            target = GetComponent<RectTransform>();

        originalPos = target.anchoredPosition;
        originalScale = target.localScale;

        StartCoroutine(GlitchLoop());
        StartCoroutine(StopGlitchAfterTime());
    }

    IEnumerator GlitchLoop()
    {
        while (glitchActive)
        {
            // Wait for the next glitch moment
            yield return new WaitForSeconds(glitchInterval);

            // ⭐ Play ONE sound per glitch event
            PlayGlitchSound();

            float timer = glitchDuration;

            while (timer > 0f && glitchActive)
            {
                timer -= Time.deltaTime;

                // position shake
                Vector2 shakeOffset = new Vector2(
                    Random.Range(-shakeIntensity, shakeIntensity),
                    Random.Range(-shakeIntensity, shakeIntensity)
                );
                target.anchoredPosition = originalPos + shakeOffset;

                // scale shake
                float scaleMod = Random.Range(0.9f, 1.1f);
                target.localScale = originalScale * scaleMod;

                // color flicker
                colorOverlay.color = new Color(
                    Random.Range(0f, 1f),
                    Random.Range(0f, 1f),
                    Random.Range(0f, 1f),
                    Random.Range(0f, colorIntensity)
                );

                yield return null;
            }

            // Reset UI after glitch burst
            target.anchoredPosition = originalPos;
            target.localScale = originalScale;
            colorOverlay.color = new Color(0, 0, 0, 0);
        }

        // final reset
        target.anchoredPosition = originalPos;
        target.localScale = originalScale;
        colorOverlay.color = new Color(0, 0, 0, 0);
    }

    IEnumerator StopGlitchAfterTime()
    {
        yield return new WaitForSeconds(totalGlitchTime);
        glitchActive = false;
    }

    void PlayGlitchSound()
    {
        if (audioSource != null && glitchSounds.Length > 0)
        {
            int index = Random.Range(0, glitchSounds.Length);
            audioSource.PlayOneShot(glitchSounds[index]);
        }
    }
}
